using FluentAssertions;
using Home.AvaliacaoTecnica.Tests.Integrations.Factory;
using Home.AvaliacaoTecnica.WebApi;
using Home.AvaliacaoTecnica.WebApi.Features.Pedido.EnviarPedido;
using Pedido.Contracts.Contratos;
using System.Text.Json;
using Testing.AzureServiceBus;
using Testing.AzureServiceBus.Builder;
using Xunit.Abstractions;
using ServiceBusConfiguration = Testing.AzureServiceBus.Configuration.ServiceBusConfiguration;

namespace Home.AvaliacaoTecnica.Tests.Integrations.Tests
{
    [Trait("Test", "IntegrationTest")]
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

        /// <summary>
        /// Testa e2e do envio de um pedido para o tópico e a leitura desse pedido pelo background service.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ShouldProcessMessageFromTopic()
        {

            // Arrange
            EnviarPedidoRequest pedidoEnviadoRequest = CriarPedidoEnviado();
            string pedidoJson = JsonSerializer.Serialize(pedidoEnviadoRequest);

            HttpClient httpClient = _webApplicationFactory.CreateClient();
            StringContent content = new StringContent(pedidoJson, System.Text.Encoding.UTF8, "application/json");

            // Act
            // envio pedido para o topico
            HttpResponseMessage responseMessage = await httpClient.PostAsync("api/pedidos", content);

            // Assert
            responseMessage.Should().NotBeNull();
            responseMessage.IsSuccessStatusCode.Should().BeTrue();

            // Captura o pedido recebido do topico
            var pedidoRecebido = await _serviceBusResource.ConsumeMessageAsync<PedidoRequestDto>(
                topicName: topico,
                subscriptionName: subScription);

            // Assert
            pedidoRecebido.Should().NotBeNull();
            AssertPedidos(pedidoEnviadoRequest, pedidoRecebido!);
        }

        private static void AssertPedidos(EnviarPedidoRequest pedidoEnviadoRequest, PedidoRequestDto pedidoRecebido)
        {
            pedidoRecebido.PedidoId.Should().Be(pedidoEnviadoRequest.PedidoId);
            pedidoRecebido.ClienteId.Should().Be(pedidoEnviadoRequest.ClienteId);
            pedidoRecebido.Itens.Should().NotBeNull();
            pedidoRecebido.Itens.Should().HaveCount(pedidoEnviadoRequest.Itens.Count);
            AssertItensPedido(pedidoEnviadoRequest, pedidoRecebido);
        }

        private static void AssertItensPedido(EnviarPedidoRequest pedidoEnviadoRequest, PedidoRequestDto pedidoRecebido)
        {
            for (int i = 0; i < pedidoEnviadoRequest.Itens.Count; i++)
            {
                var expectedItem = pedidoEnviadoRequest.Itens[i];
                var actualItem = pedidoRecebido.Itens[i];

                actualItem.Should().NotBeNull();
                actualItem.ProdutoId.Should().Be(expectedItem.ProdutoId);
                actualItem.Quantidade.Should().Be(expectedItem.Quantidade);
                actualItem.Valor.Should().Be(expectedItem.Valor);
            }
        }

        private static EnviarPedidoRequest CriarPedidoEnviado()
        {
            return new EnviarPedidoRequest
            {
                PedidoId = 1,
                ClienteId = 1,
                Itens = new List<EnviarItemPedidoRequest>
                {
                    new EnviarItemPedidoRequest
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
}