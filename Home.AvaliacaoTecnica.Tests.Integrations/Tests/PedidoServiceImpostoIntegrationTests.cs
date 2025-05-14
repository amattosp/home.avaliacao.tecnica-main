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
using System.Diagnostics;
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



    [Trait("Category", "IntegrationTest")]
    [Fact(DisplayName = "Deve calcular imposto com a feature flag 'UsarReformaTributaria' DESABILITADA")]
    public async Task Deve_Calcular_Imposto_Com_FeatureFlag_ReformaTributaria_Desabilitada()
    {
        await ExecutarCenarioFeatureFlag(false, 15.81m);
    }

    [Trait("Category", "IntegrationTest")]
    [Fact(DisplayName = "Deve calcular imposto com a feature flag 'UsarReformaTributaria' HABILITADA")]
    public async Task Deve_Calcular_Imposto_Com_FeatureFlag_ReformaTributaria_Habilitada()
    {
        await ExecutarCenarioFeatureFlag(true, 10.54m);
    }

    private async Task ExecutarCenarioFeatureFlag(bool usarReformaTributaria, decimal valorEsperado)
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


        // Configuração do Service Bus
        var serviceBusConnectionString =
            "Endpoint=sb://127.0.0.1;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;";

        // Configuração do Host BackGroundService
        Debug.WriteLine($"Configurando o host para o Service Bus: {serviceBusConnectionString}");
        using Microsoft.Extensions.Hosting.IHost host = ConfigurarHost(usarReformaTributaria, serviceBusConnectionString);

        Debug.WriteLine($"Vou enviar um pedido: {pedidoJson}");
        HttpResponseMessage responseMessage = await httpClient.PostAsync("api/pedidos", content);
        responseMessage.Should().NotBeNull();
        responseMessage.IsSuccessStatusCode.Should().BeTrue();

        await host.StartAsync();
        Task.Delay(2500).Wait();
        await host.StopAsync();
        
        var resultado = await _serviceBusResource.ConsumeMessageAsync<PedidoProcessadoDto>(
            "pedidos-processados", "sistemaB", 10);

        Debug.WriteLine($"Resultado: {resultado}");
        resultado.Should().NotBeNull();

        // Assert
        resultado!.Imposto.Should().Be(valorEsperado);
    }

    private Microsoft.Extensions.Hosting.IHost ConfigurarHost(bool usarReformaTributaria, string serviceBusConnectionString)
    {
        return _factory.CreateHost(services =>
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:ServiceBus"] = serviceBusConnectionString,
                    ["FeatureManagement:UsarReformaTributaria"] = usarReformaTributaria.ToString().ToLowerInvariant()
                })
                .Build();

            services.AddFeatureManagement();
            services.AddTransient<ImpostoReformaTributariaStrategy>();
            services.AddTransient<ImpostoVigenteStrategy>();
            services.AddTransient<ImpostoStrategyFactory>();
            services.AddTransient<IImpostoStrategy>(provider =>
            {
                var featureManager = provider.GetRequiredService<IFeatureManager>();
                var factory = provider.GetRequiredService<ImpostoStrategyFactory>();
                var flag = featureManager.IsEnabledAsync("UsarReformaTributaria").GetAwaiter().GetResult();
                return factory.GetStrategy(flag);
            });
            services.AddSingleton<IConfiguration>(configuration);
            services.AddTransient<IServiceBusConsumer, AzureServiceBusConsumer>();
            services.AddTransient<IPedidoService, PedidoService>();
            services.AddSingleton<PedidoBackgroundService>();
            services.AddTransient<IServiceBusConsumer>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<AzureServiceBusConsumer>>();
                var connectionString = configuration.GetConnectionString("ServiceBus");
                var impostoStrategy = sp.GetRequiredService<IImpostoStrategy>();
                var pedidoService = sp.GetRequiredService<IPedidoService>();
                return new AzureServiceBusConsumer(logger, connectionString, impostoStrategy, pedidoService);
            });
        });
    }
}