using Home.AvaliacaoTecnica.Consumer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<ConsumerWorker>();
    })
    .Build()
    .Run();
