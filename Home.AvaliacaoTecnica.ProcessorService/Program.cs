using Home.AvaliacaoTecnica.Processor.Service;
using Home.AvaliacaoTecnica.Domain.Interfaces;
using Home.AvaliacaoTecnica.Domain.Strategies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;
using Pedido.Processor.Domain.Strategies;
using Home.AvaliacaoTecnica.Domain.Services;

Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddFeatureManagement();

        // Registra as implementações específicas
        services.AddSingleton<ImpostoReformaTributariaStrategy>();
        services.AddSingleton<ImpostoVigenteStrategy>();

        // Registra a factory
        services.AddSingleton<ImpostoStrategyFactory>();

        // Registra a interface com base na factory
        services.AddSingleton<IImpostoStrategy>(provider =>
        {
            var featureManager = provider.GetRequiredService<IFeatureManager>();
            var factory = provider.GetRequiredService<ImpostoStrategyFactory>();

            var usarReforma = featureManager
                .IsEnabledAsync("UsarReformaTributaria")
                .GetAwaiter().GetResult();

            return factory.GetStrategy(usarReforma);
        });


        services.AddHostedService<PedidoWorker>();
        services.AddScoped<IPedidoService, PedidoService>(); 
    })
    .Build()
    .Run();
