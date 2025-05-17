using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;

namespace Home.AvaliacaoTecnica.Application.Config;

public class ServiceBusConfigurationBuilder(IServiceCollection serviceCollection)
{
    public static readonly string LocalServiceBusClientName = nameof(LocalServiceBusClientName);
    public ServiceBusConfigurationBuilder AddSender(
        string senderName,
        string queueOrTopicName)
    {
        serviceCollection.AddAzureClients(
            clientBuilder =>
            {
                clientBuilder.AddClient<ServiceBusSender, ServiceBusClientOptions>(
                        (
                            _,
                            _,
                            serviceProvider) =>
                        {
                            IAzureClientFactory<ServiceBusClient> factory = serviceProvider.GetRequiredService<IAzureClientFactory<ServiceBusClient>>();
                            ServiceBusClient client = factory.CreateClient(nameof(LocalServiceBusClientName));
                            return client.CreateSender(queueOrTopicName);
                        })
                    .WithName(senderName);
            });

        return this;
    }

    public ServiceBusConfigurationBuilder AddProcessor(
        string processorName,
        string topicName,
        string subscriptionName)
    {
        return AddProcessorInternal(
            processorName,
            topicName,
            subscriptionName);
    }

    private ServiceBusConfigurationBuilder AddProcessorInternal(
        string processorName,
        string queueOrTopicName,
        string? subscriptionName)
    {
        serviceCollection.AddAzureClients(
            clientBuilder =>
            {
                clientBuilder.AddClient<ServiceBusProcessor, ServiceBusClientOptions>(
                        (
                            _,
                            _,
                            serviceProvider) =>
                        {
                            ServiceBusProcessorOptions options = new()
                            {
                                AutoCompleteMessages = false,
                                MaxConcurrentCalls = 1,
                            };
                            IAzureClientFactory<ServiceBusClient> factory = serviceProvider.GetRequiredService<IAzureClientFactory<ServiceBusClient>>();
                            ServiceBusClient client = factory.CreateClient(ServiceBusConstants.LocalServiceBusClientName);

                            if (!string.IsNullOrWhiteSpace(subscriptionName))
                            {
                                return client.CreateProcessor(
                                    queueOrTopicName,
                                    subscriptionName,
                                    options);
                            }

                            return client.CreateProcessor(
                                queueOrTopicName,
                                options);
                        })
                    .WithName(processorName);
            });

        return this;
    }
}
