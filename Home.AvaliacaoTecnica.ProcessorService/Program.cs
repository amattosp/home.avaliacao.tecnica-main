namespace Home.AvaliacaoTecnica.ProcessorService;

using Home.AvaliacaoTecnica.Domain.Interfaces;
using Home.AvaliacaoTecnica.Domain.Services;
using Home.AvaliacaoTecnica.Domain.Strategies;
using Home.AvaliacaoTecnica.ProcessorService.Messaging;
using Home.AvaliacaoTecnica.ProcessorService.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;

public class Program
{
    public static void Main(string[] args)
    {
        // Configuração do Host para o Worker Service
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // Configuração de Feature Management
                services.AddFeatureManagement();

                // Registro de estratégias de imposto
                services.AddTransient<ImpostoReformaTributariaStrategy>();
                services.AddTransient<ImpostoVigenteStrategy>();

                // Registro da factory de estratégias
                services.AddTransient<ImpostoStrategyFactory>();

                // Registro da interface de estratégia com base na factory
                services.AddTransient<IImpostoStrategy>(provider =>
                {
                    var featureManager = provider.GetRequiredService<IFeatureManager>();
                    var factory = provider.GetRequiredService<ImpostoStrategyFactory>();

                    var usarReforma = featureManager
                        .IsEnabledAsync("UsarReformaTributaria")
                        .GetAwaiter().GetResult();

                    return factory.GetStrategy(usarReforma);
                });

                services.AddTransient<IPedidoService, PedidoService>();

                // Registro do consumidor do Azure Service Bus
                services.AddTransient<IServiceBusConsumer>(sp =>
                {
                    var configuration = sp.GetRequiredService<IConfiguration>();
                    var connectionString = configuration.GetConnectionString("ServiceBus");
                    var logger = sp.GetRequiredService<ILogger<AzureServiceBusConsumer>>();
                    var impostoStrategy = sp.GetRequiredService<IImpostoStrategy>();
                    var pedidoService = sp.GetRequiredService<IPedidoService>();
                    return new AzureServiceBusConsumer(logger, connectionString, impostoStrategy, pedidoService);
                });

                // Registro do Hosted Service para processamento em background
                services.AddHostedService<PedidoBackgroundService>();
            })
            .Build()
            .Run();
    }
}