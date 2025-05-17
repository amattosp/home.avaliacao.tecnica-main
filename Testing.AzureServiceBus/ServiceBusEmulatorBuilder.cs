using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;

namespace Testing.AzureServiceBus;

public class ServiceBusEmulatorBuilder(IServiceCollection serviceCollection)
{
    public ServiceBusEmulatorBuilder AddSender(
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
                            ServiceBusClient client = factory.CreateClient(ServiceBusConstants.LocalServiceBusClientName);
                            return client.CreateSender(queueOrTopicName);
                        })
                    .WithName(senderName);
            });

        return this;
    }

    public ServiceBusEmulatorBuilder AddProcessor(
        string processorName,
        string queueName)
    {
        return AddProcessorInternal(
            processorName,
            queueName,
            null);
    }

    public ServiceBusEmulatorBuilder AddProcessor(
        string processorName,
        string topicName,
        string subscriptionName)
    {
        return AddProcessorInternal(
            processorName,
            topicName,
            subscriptionName);
    }

    private ServiceBusEmulatorBuilder AddProcessorInternal(
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