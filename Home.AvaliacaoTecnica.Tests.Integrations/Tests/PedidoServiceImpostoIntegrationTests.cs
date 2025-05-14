using FluentAssertions;
using Home.AvaliacaoTecnica.Contracts.Contratos;
using Home.AvaliacaoTecnica.Domain.Interfaces;
using Home.AvaliacaoTecnica.Domain.Services;
using Home.AvaliacaoTecnica.Domain.Strategies;
using Home.AvaliacaoTecnica.ProcessorService.Messaging;
using Home.AvaliacaoTecnica.ProcessorService.Services;
using Home.AvaliacaoTecnica.Tests.Integrations.Factory;
using Home.AvaliacaoTecnica.WebApi;
using Home.AvaliacaoTecnica.WebApi.Features.Pedido.EnviarPedido;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using System.Text.Json;
using Testing.AzureServiceBus;
using Testing.AzureServiceBus.Builder;
using Testing.AzureServiceBus.Configuration;
using Xunit.Abstractions;

namespace Home.AvaliacaoTecnica.Tests.Integrations.Tests;

/// <summary>
/// Integration test class for validating the tax calculation logic in the order processing flow,
/// based on the "UsarReformaTributaria" feature flag. 
/// 
/// This class sets up a test environment with Azure Service Bus topics and subscriptions, 
/// sends order requests through the API, and verifies that the correct tax strategy 
/// (current or reform) is applied by consuming the processed order message from the service bus.
/// 
/// The tests ensure that the system correctly switches between tax calculation strategies 
/// according to the feature flag, and that the integration between API, background services, 
/// and messaging infrastructure works as expected.
/// </summary>
public class PedidoServiceImpostoIntegrationTests : IClassFixture<CustomWebApplicationFactory<IApiAssemblyMarker>>, IClassFixture<ServiceBusResource>, IClassFixture<HostApplicationFactory>
{
    private readonly CustomWebApplicationFactory<IApiAssemblyMarker> _webApplicationFactory;
    private readonly HostApplicationFactory _factory;
    private readonly ServiceBusResource _serviceBusResource;

    public PedidoServiceImpostoIntegrationTests(CustomWebApplicationFactory<IApiAssemblyMarker> webApplicationFactory,
                                                HostApplicationFactory factory,
                                                ServiceBusResource serviceBusResource,
                                                ITestOutputHelper testOutputHelper)       
    {
        ArgumentNullException.ThrowIfNull(serviceBusResource);

        serviceBusResource.Initialize(
                    ServiceBusConfig(),
                testOutputHelper);

        _webApplicationFactory = webApplicationFactory;
        _factory = factory;
        _serviceBusResource = serviceBusResource;

    }

    private static ServiceBusConfiguration ServiceBusConfig()
     => ServiceBusConfigurationBuilder
         .Create()
         .AddDefaultNamespace(
             namespaceConfigBuilder => namespaceConfigBuilder
                 .AddTopic(
                     "pedidos",
                     topicBuilder => topicBuilder
                         .AddSubscription("processador"))
                 .AddTopic(
                     "pedidos-processados",
                     topicBuilder => topicBuilder
                         .AddSubscription("sistemaB"))
         )
         .Build();


    [Theory(Skip = "Desabilitado temporariamente pois estamos corrigindo o assert da regra de calculo")]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Deve_Calcular_Imposto_Conforme_FeatureFlag_ReformaTributaria(bool usarReformaTributaria)
    {
        // Arrange
        var pedidoRequest = new EnviarPedidoRequest
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

        string pedidoJson = JsonSerializer.Serialize(pedidoRequest);
        HttpClient httpClient = _webApplicationFactory.CreateClient();
        StringContent content = new StringContent(pedidoJson, System.Text.Encoding.UTF8, "application/json");

        // Envio do pedido para o tópico
        HttpResponseMessage responseMessage = await httpClient.PostAsync("api/pedidos", content);
        responseMessage.Should().NotBeNull();
        responseMessage.IsSuccessStatusCode.Should().BeTrue();

        var serviceBusConnectionString =
            "Endpoint=sb://127.0.0.1;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;";

        using var host = _factory.CreateHost(services =>
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:ServiceBus"] = serviceBusConnectionString,
                    ["FeatureManagement:UsarReformaTributaria"] = usarReformaTributaria.ToString().ToLowerInvariant()
                })
                .Build();

            services.AddFeatureManagement();
            services.AddSingleton<ImpostoReformaTributariaStrategy>();
            services.AddSingleton<ImpostoVigenteStrategy>();
            services.AddSingleton<ImpostoStrategyFactory>();
            services.AddSingleton<IImpostoStrategy>(provider =>
            {
                var featureManager = provider.GetRequiredService<IFeatureManager>();
                var factory = provider.GetRequiredService<ImpostoStrategyFactory>();
                var flag = featureManager.IsEnabledAsync("UsarReformaTributaria").GetAwaiter().GetResult();
                return factory.GetStrategy(flag);
            });
            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton<IServiceBusConsumer, AzureServiceBusConsumer>();
            services.AddSingleton<IPedidoService, PedidoService>();
            services.AddSingleton<PedidoBackgroundService>();
            services.AddSingleton<IServiceBusConsumer>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<AzureServiceBusConsumer>>();
                var connectionString = configuration.GetConnectionString("ServiceBus");
                var impostoStrategy = sp.GetRequiredService<IImpostoStrategy>();
                var pedidoService = sp.GetRequiredService<IPedidoService>();
                return new AzureServiceBusConsumer(logger, connectionString, impostoStrategy, pedidoService);
            });
        });

        await host.StartAsync();
        await Task.Delay(2500);
        await host.StopAsync();


        // Recebe o pedido que foi calculo o imposto no processor service do topico pedidos-processados
        var resultado = await _serviceBusResource.ConsumeMessageAsync<PedidoProcessadoDto>(
          "pedidos-processados", "sistemaB", 5);

        // Assert
        decimal valorEsperado = usarReformaTributaria ? 10.54m : 15.81m;
        resultado!.Imposto.Should().Be(valorEsperado);
    }
}