using Azure.Messaging.ServiceBus;

namespace Home.AvalicaoTecnica.WebApi.Services
{
    
    public class ServiceBusSenderFactory
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
