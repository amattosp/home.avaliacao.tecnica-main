using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;

namespace Home.AvaliacaoTecnica.Application.Services.Wrapper;

public class ServiceBusSenderWrapper : IServiceBusSenderWrapper
{
    private readonly IAzureClientFactory<ServiceBusSender> _clientFactory;
    private ServiceBusSender? _sender;

    public ServiceBusSenderWrapper(IAzureClientFactory<ServiceBusSender> clientFactory)
    {
        _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
    }

    public void ConfigureSender(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("O nome do sender do tópico não pode ser nulo ou vazio.", nameof(name));
        }

        _sender = _clientFactory.CreateClient(name);
    }

    public async Task SendMessageAsync(ServiceBusMessage message, CancellationToken cancellationToken = default)
          => await _sender!.SendMessageAsync(message, cancellationToken);
}