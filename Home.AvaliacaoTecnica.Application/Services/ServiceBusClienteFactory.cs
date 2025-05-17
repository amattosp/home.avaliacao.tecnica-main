using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;

namespace Home.AvaliacaoTecnica.Application.Services;

public class ServiceBusClienteFactory : IServiceBusSenderFactory
{
    private readonly IAzureClientFactory<ServiceBusClient> _clientFactory;

    public ServiceBusClienteFactory(IAzureClientFactory<ServiceBusClient> clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public ServiceBusSender CreateSender(string topicName)
    {
        // Obtém o cliente configurado
        var client = _clientFactory.CreateClient("topicSender");
        return client.CreateSender(topicName);
    }
}