using System;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.Messaging;
using Azure.Messaging.ServiceBus;
using Home.AvaliacaoTecnica.WebApi;
using Testing.AzureServiceBus;
using Testing.AzureServiceBus.Builder;
using Testing.AzureServiceBus.Configuration;
using Xunit;
using Xunit.Abstractions;

namespace TestBenchApi.Tests;

public class ApiTests : IClassFixture<CustomWebApplicationFactory<IApiAssemblyMarker>>, IClassFixture<ServiceBusResource>
{
    private readonly ServiceBusResource _serviceBusResource;
    private readonly CustomWebApplicationFactory<IApiAssemblyMarker> _webApplicationFactory;

    public ApiTests(
        CustomWebApplicationFactory<IApiAssemblyMarker> webApplicationFactory,
        ServiceBusResource serviceBusResource,
        ITestOutputHelper testOutputHelper)
    {
        ArgumentNullException.ThrowIfNull(serviceBusResource);

        serviceBusResource.Initialize(
            ServiceBusConfig(),
            testOutputHelper);

        _webApplicationFactory = webApplicationFactory;
        _serviceBusResource = serviceBusResource;
    }


    [Fact]
    public async Task TestTopicSendMessage()
    {
        // Arrange
        HttpClient httpClient = _webApplicationFactory.CreateClient();

        // Act
        //todo: Modificar a url para representar o endpoint de envio de pedidos
        HttpResponseMessage responseMessage = await httpClient.PostAsync(
            "api/send/topic",
            null);

        // Assert
        responseMessage.EnsureSuccessStatusCode();

        CloudEvent message = await _serviceBusResource.ConsumeMessageAsync<CloudEvent>(
            "testtopic",
            "testsubscription");
        Assert.NotNull(message);
        Assert.NotNull(message.Data);

        MessageArgs messageArgs = message.Data.ToObjectFromJson<MessageArgs>();
        Assert.NotNull(messageArgs);

        Assert.Equal(
            "Hello World!",
            messageArgs.Message);
    }


    [Fact]
    public async Task TestProcessTopicMessage()
    {
        // Arrange
        HttpClient httpClient = _webApplicationFactory.CreateClient();

        // Act
        HttpResponseMessage responseMessage = await httpClient.PostAsync(
            "api/send/topic",
            null);

        // Assert
        responseMessage.EnsureSuccessStatusCode();

        CloudEvent message = await _serviceBusResource.ConsumeMessageAsync<CloudEvent>(
            "testtopic",
            "testsubscription");
        Assert.NotNull(message);
        Assert.NotNull(message.Data);

        MessageArgs messageArgs = message.Data.ToObjectFromJson<MessageArgs>();
        Assert.NotNull(messageArgs);

        Assert.Equal(
            "Hello World!",
            messageArgs.Message);
    }

    private static ServiceBusConfiguration ServiceBusConfig()
        => ServiceBusConfigurationBuilder
            .Create()
            .AddDefaultNamespace(
                namespaceConfigBuilder => namespaceConfigBuilder
                    .AddQueue("testqueue")
                    .AddTopic(
                        "testtopic",
                        topicBuilder => topicBuilder
                            .AddSubscription(
                                "testsubscription")
                            .AddSubscription(
                                "testsubscriptionwithrule",
                                subscriptionBuilder => subscriptionBuilder
                                    .AddRule(
                                        "testrule",
                                        ruleBuilder => ruleBuilder
                                            .WithFilterType(RuleFilterType.Correlation)
                                            .WithCorrelationFilter(
                                                correlationFilterBuilder => correlationFilterBuilder
                                                    .WithCloudEventsContentType()
                                                    .WithProperties(
                                                        "MyProperty",
                                                        "MyValue"))))))
            .Build();
}