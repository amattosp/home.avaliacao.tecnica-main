using Azure.Messaging.ServiceBus;
using Home.AvaliacaoTecnica.Domain.Interfaces;
using Home.AvaliacaoTecnica.Domain.Strategies;
using Home.AvaliacaoTecnica.Infra.Data.Context;
using Home.AvaliacaoTecnica.Tests.Integrations.Fixtures;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;

namespace Home.AvaliacaoTecnica.Tests.Integrations.Tests
{
    public class PedidoBackgroundServiceTests : IClassFixture<ContainersFixture>
    {
        private readonly ContainersFixture _containersFixture;
        private WebApplicationFactory<Program>? _factory;

        public PedidoBackgroundServiceTests(ContainersFixture containersFixture)
        {
            _containersFixture = containersFixture;
        }

        [Fact(Skip = "Testes de integração ainda não concluídos.")]
        public async Task DeveProcessarMensagensDoServiceBus()
        {
            var factory = FactoryCreate();
            var client = factory!.CreateClient();

            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PedidoDbContext>();

            // Act
            var host = _factory.Services.GetRequiredService<IHost>();
            await host.StartAsync();

            // Envie uma mensagem para o Service Bus
            var clientServiceBus = new ServiceBusClient(_containersFixture.ConnectionString);
            var sender = clientServiceBus.CreateSender("pedidos");
            await sender.SendMessageAsync(new ServiceBusMessage("Novo Pedido"));

            // Aguarde o processamento
            await Task.Delay(1000);

            // Assert

            await host.StopAsync();
        }

        private WebApplicationFactory<Program> FactoryCreate()
        {
            // Arrange
            var factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureAppConfiguration((context, config) =>
                    {
                        // Adiciona o appsettings.json do projeto ao contexto de configuração
                        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    });

                    builder.ConfigureServices(services =>
                    {
                        // Configurações adicionais para o teste
                        services.AddFeatureManagement();
                        services.AddSingleton<ImpostoStrategyFactory>();
                        services.AddSingleton<IImpostoStrategy>(provider =>
                        {
                            var featureManager = provider.GetRequiredService<IFeatureManager>();
                            var factory = provider.GetRequiredService<ImpostoStrategyFactory>();

                            var usarReforma = featureManager
                                .IsEnabledAsync("UsarReformaTributaria")
                                .GetAwaiter().GetResult();

                            return factory.GetStrategy(usarReforma);
                        });

                        // Injeta a ConnectionString do Service Bus Emulator
                        services.AddSingleton(_ => _containersFixture.ConnectionString);
                    });
                });

            return factory;
        }
    }
}