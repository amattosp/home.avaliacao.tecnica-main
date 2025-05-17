using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Testing.AzureServiceBus;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAzureServiceBusEmulator(
        this IServiceCollection services,
        Action<ServiceBusEmulatorBuilder> builder)
    {
        services.RemoveAll(typeof(ServiceBusProcessor));
        services.RemoveAll(typeof(ServiceBusSender));
        services.AddAzureClients(
            clientBuilder =>
            {
                clientBuilder.AddServiceBusClient(ServiceBusConstants.ConnectionString)
                    .WithName(ServiceBusConstants.LocalServiceBusClientName);
            });

        builder(new ServiceBusEmulatorBuilder(services));

        return services;
    }
}