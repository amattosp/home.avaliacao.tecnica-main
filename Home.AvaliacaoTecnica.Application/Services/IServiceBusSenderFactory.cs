using Azure.Messaging.ServiceBus;

namespace Home.AvaliacaoTecnica.Application.Services;

public interface IServiceBusSenderFactory
{
    ServiceBusSender CreateSender(string topicName);
}
