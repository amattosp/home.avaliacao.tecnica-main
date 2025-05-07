using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace Home.AvaliacaoTecnica.Application.Services
{
    public class ServiceBusSenderFactory : IServiceBusSenderFactory
    {
        private readonly ServiceBusClient _client;

        public ServiceBusSenderFactory(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("ServiceBus")!;
            _client = new ServiceBusClient(connectionString);
        }

        public ServiceBusSender CreateSender(string topicName)
        {
            return _client.CreateSender(topicName);
        }
    }
}
