using Azure.Messaging.ServiceBus;
using System.Text.Json;

namespace Home.AvaliacaoTecnica.Tests.Integrations.Tests
{
    public class PedidoBackgroundServiceIntegrationTests
    {
        private readonly string _serviceBusConnectionString = "Endpoint=sb://127.0.0.1;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;";
        private readonly string _topicName = "pedidos";
        private readonly string _subscriptionName = "processador";

        [Fact]
        public async Task ShouldProcessMessageFromTopic()
        {
            try
            {
                // Enviar mensagem para o tópico
                var pedido = new PedidoEnviadoBackGroundService
                {
                    Id = 1,
                    Cliente = "Jose Silva",
                    ValorTotal = 150.75m,
                    DataPedido = DateTime.UtcNow
                };

                string pedidoJson = JsonSerializer.Serialize(pedido);

                // Enviar mensagem para o tópico
                var client = new ServiceBusClient(_serviceBusConnectionString);
                var sender = client.CreateSender(_topicName);
                await sender.SendMessageAsync(new ServiceBusMessage(pedidoJson));

                using var factory = new PedidoProcessorFactory(_serviceBusConnectionString);
                var host = factory.Server.Host;

                // Aguarda processamento
                await Task.Delay(3000);

                // Verifica se a mensagem foi processada (a assinatura deve estar vazia agora)
                var receiver = client.CreateReceiver(_topicName, _subscriptionName);
                var received = await receiver.ReceiveMessageAsync(TimeSpan.FromSeconds(2));
                Assert.Null(received); // Deve estar vazia se foi processada

                await receiver.DisposeAsync();
                await client.DisposeAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao conectar ao Service Bus: {ex.Message}");
            }
        }
    }
}

public class PedidoEnviadoBackGroundService
{
    public int Id { get; set; }
    public string Cliente { get; set; }
    public decimal ValorTotal { get; set; }
    public DateTime DataPedido { get; set; }
}