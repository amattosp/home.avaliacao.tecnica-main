using Home.AvaliacaoTecnica.ProcessorService.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Home.AvaliacaoTecnica.Tests.Integrations.Factory;

public class HostApplicationFactory
{
    public IHost CreateHost(Action<IServiceCollection>? configureServices = null)
    {
        var builder = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                // Adiciona o worker real
                services.AddHostedService<PedidoBackgroundService>();

                // Configuração customizada para testes
                configureServices?.Invoke(services);
            });

        return builder.Build();
    }
}
