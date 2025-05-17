using Azure.Messaging.ServiceBus;

namespace Home.AvaliacaoTecnica.Application.Services.Wrapper;

public interface IServiceBusSenderWrapper
{
    /// <summary>
    /// Configura o sender para um tópico específico.
    /// </summary>
    /// <param name="topicName">Nome do tópico.</param>
    void ConfigureSender(string name);

    /// <summary>
    /// Envia uma mensagem para o Service Bus.
    /// </summary>
    /// <param name="message">Mensagem a ser enviada.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    Task SendMessageAsync(ServiceBusMessage message, CancellationToken cancellationToken = default);
}
