using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Home.AvaliacaoTecnica.Tests.Integrations.Tests;

public class PedidoProcessorFactory : WebApplicationFactory<ProcessorService.Program>
{
    private readonly string _serviceBusConnectionString;

    public PedidoProcessorFactory(string serviceBusConnectionString)
    {
        _serviceBusConnectionString = serviceBusConnectionString;
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(config =>
        {
            var testSettings = new Dictionary<string, string>
            {
                ["ConnectionStrings:ServiceBus"] = _serviceBusConnectionString
            };

            config.AddInMemoryCollection(testSettings!);
        });

        return base.CreateHost(builder);
    }
}