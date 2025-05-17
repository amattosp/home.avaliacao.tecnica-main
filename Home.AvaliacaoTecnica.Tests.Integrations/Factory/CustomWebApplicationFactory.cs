using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Testing.AzureServiceBus;

namespace Home.AvaliacaoTecnica.Tests.Integrations.Factory;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class
{
    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "ConnectionStrings:ServiceBus", ServiceBusConstants.ConnectionString }
                }!)
                .Build();

            config.AddConfiguration(configuration);
        });

        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Debug);
        });

        builder.ConfigureServices(
            services =>
            {
                services.AddAzureServiceBusEmulator(
                    sbBuilder => sbBuilder
                        .AddSender(
                            "TopicSender",
                            "pedidos")
                        .AddProcessor(
                            "TopicProcessor",
                            "pedidos",
                            "processador"));
            });
    }
}