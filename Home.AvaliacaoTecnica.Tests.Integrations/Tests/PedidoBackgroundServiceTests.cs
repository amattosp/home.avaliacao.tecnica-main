using Home.AvaliacaoTecnica.Tests.Integrations.Factory;
using Home.AvaliacaoTecnica.WebApi;
using Pedido.Contracts.Contratos;
using System.Text.Json;
using Testing.AzureServiceBus;
using Testing.AzureServiceBus.Builder;
using Xunit.Abstractions;
using ServiceBusConfiguration = Testing.AzureServiceBus.Configuration.ServiceBusConfiguration;

namespace Home.AvaliacaoTecnica.Tests.Integrations.Tests
{
    public class PedidoBackgroundServiceIntegrationTests : IClassFixture<CustomWebApplicationFactory<IApiAssemblyMarker>>, IClassFixture<ServiceBusResource>
    {
        private readonly ServiceBusResource _serviceBusResource;
        private readonly CustomWebApplicationFactory<IApiAssemblyMarker> _webApplicationFactory;
        private const string topico = "pedidos";
        private const string subScription = "processador";

        public PedidoBackgroundServiceIntegrationTests(
            CustomWebApplicationFactory<IApiAssemblyMarker> webApplicationFactory,
            ServiceBusResource serviceBusResource,
            ITestOutputHelper testOutputHelper)
        {
            ArgumentNullException.ThrowIfNull(serviceBusResource);

            serviceBusResource.Initialize(
                ServiceBusConfig(),
            testOutputHelper);

            _webApplicationFactory = webApplicationFactory;
            _serviceBusResource = serviceBusResource;
        }

        [Fact]
        public async Task ShouldProcessMessageFromTopic()
        {
            try
            {
                // Arrange
                PedidoEnviadoRequest pedidoEnviadoRequest = CriarPedidoEnviado();
                string pedidoJson = JsonSerializer.Serialize(pedidoEnviadoRequest);

                HttpClient httpClient = _webApplicationFactory.CreateClient();
                StringContent content = new StringContent(pedidoJson, System.Text.Encoding.UTF8, "application/json");

                // Act
                // envio pedido para o topico
                HttpResponseMessage responseMessage = await httpClient.PostAsync("api/pedidos", content);

                // Assert
                responseMessage.EnsureSuccessStatusCode();
                Assert.NotNull(responseMessage);

                // Captura o pedido recebido do topico
                var pedidoRecebido = await _serviceBusResource.ConsumeMessageAsync<PedidoRequestDto>(
                    topicName: topico,
                    subscriptionName: subScription);

                // Assert
                Assert.NotNull(pedidoRecebido);
                AssertPedidos(pedidoEnviadoRequest, pedidoRecebido);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao conectar ao Service Bus: {ex.Message}");
            }
        }

        private static void AssertPedidos(PedidoEnviadoRequest pedidoEnviadoRequest, PedidoRequestDto pedidoRecebido)
        {
            Assert.Equal(pedidoEnviadoRequest.PedidoId, pedidoRecebido.PedidoId);
            Assert.Equal(pedidoEnviadoRequest.ClienteId, pedidoRecebido.ClienteId);
            Assert.NotNull(pedidoRecebido.Itens);
            Assert.Equal(pedidoEnviadoRequest.Itens.Count, pedidoRecebido.Itens.Count);
            for (int i = 0; i < pedidoEnviadoRequest.Itens.Count; i++)
            {
                var expectedItem = pedidoEnviadoRequest.Itens[i];
                var actualItem = pedidoRecebido.Itens[i];

                Assert.NotNull(actualItem);
                Assert.Equal(expectedItem.ProdutoId, actualItem.ProdutoId);
                Assert.Equal(expectedItem.Quantidade, actualItem.Quantidade);
                Assert.Equal(expectedItem.Valor, actualItem.Valor);
            }
        }

        private static PedidoEnviadoRequest CriarPedidoEnviado()
        {
            return new PedidoEnviadoRequest
            {
                PedidoId = 1,
                ClienteId = 1,
                Itens = new List<ItemPedido>
                {
                    new ItemPedido
                    {
                        ProdutoId = 1001,
                        Quantidade = 2,
                        Valor = 52.70m
                    }
                }
            };
        }

        private static ServiceBusConfiguration ServiceBusConfig()
            => ServiceBusConfigurationBuilder
                .Create()
                .AddDefaultNamespace(
                    namespaceConfigBuilder => namespaceConfigBuilder
                        .AddTopic(
                            "pedidos",
                            topicBuilder => topicBuilder
                                .AddSubscription(
                                    "processador")))
                .Build();
    }

    //todo: classe utilizadas apenas para teste, depois podem ser substituidas pela classe original do projeto da API
    public class PedidoEnviadoRequest
    {
        public int PedidoId { get; set; }
        public int ClienteId { get; set; }
        public List<ItemPedido> Itens { get; set; } = new List<ItemPedido>();
    }

    public class ItemPedido
    {
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal Valor { get; set; }
    }
}
