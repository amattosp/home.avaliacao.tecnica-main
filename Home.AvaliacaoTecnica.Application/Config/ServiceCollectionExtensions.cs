using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Home.AvaliacaoTecnica.Application.Config;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAzureServiceBusConfiguration(
        this IServiceCollection services,
        Action<ServiceBusConfigurationBuilder> builder)
    {
        services.RemoveAll(typeof(ServiceBusProcessor));
        services.RemoveAll(typeof(ServiceBusSender));
        services.AddAzureClients(
            clientBuilder =>
            {
                clientBuilder.AddServiceBusClient(ServiceBusConstants.ConnectionString)
                    .WithName(ServiceBusConstants.LocalServiceBusClientName);
            });

        builder(new ServiceBusConfigurationBuilder(services));

        return services;
    }
}