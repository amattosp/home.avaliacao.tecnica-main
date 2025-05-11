using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using Testing.AzureServiceBus;

namespace TestBenchApi.Tests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class
{
    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
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
                            "QueueSender",
                            "testqueue")
                        .AddSender(
                            "TopicSender",
                            "testtopic")
                        .AddProcessor(
                            "QueueProcessor",
                            "testqueue")
                        .AddProcessor(
                            "TopicProcessor",
                            "testtopic",
                            "testsubscription"));
            });
    }
}