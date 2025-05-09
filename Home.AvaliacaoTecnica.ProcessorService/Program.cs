using Home.AvaliacaoTecnica.Domain.Interfaces;
using Home.AvaliacaoTecnica.Domain.Services;
using Home.AvaliacaoTecnica.Domain.Strategies;
using Home.AvaliacaoTecnica.ProcessorService.Messaging;
using Home.AvaliacaoTecnica.ProcessorService.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;

// Configuração do Host para o Worker Service
Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Configuração de Feature Management
        services.AddFeatureManagement();

        // Registro de estratégias de imposto
        services.AddSingleton<ImpostoReformaTributariaStrategy>();
        services.AddSingleton<ImpostoVigenteStrategy>();

        // Registro da factory de estratégias
        services.AddSingleton<ImpostoStrategyFactory>();

        // Registro da interface de estratégia com base na factory
        services.AddSingleton<IImpostoStrategy>(provider =>
        {
            var featureManager = provider.GetRequiredService<IFeatureManager>();
            var factory = provider.GetRequiredService<ImpostoStrategyFactory>();

            var usarReforma = featureManager
                .IsEnabledAsync("UsarReformaTributaria")
                .GetAwaiter().GetResult();

            return factory.GetStrategy(usarReforma);
        });

        var config = context.Configuration;

        // Registro do consumidor do Azure Service Bus
        services.AddSingleton<IServiceBusConsumer, AzureServiceBusConsumer>();

        // Registro do Hosted Service para processamento em background
        services.AddHostedService<PedidoBackgroundService>();
        services.AddScoped<IPedidoService, PedidoService>();
    })
    .Build()
    .Run();