using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace Home.AvaliacaoTecnica.Tests.Integrations.Fixtures;

public class ContainersFixture : IAsyncLifetime
{
    private readonly IContainer _sqlContainer;
    private readonly IContainer _serviceBusContainer;

    public ContainersFixture()
    {
        _sqlContainer = new ContainerBuilder()
          .WithImage("mcr.microsoft.com/azure-sql-edge:latest")
          .WithEnvironment("ACCEPT_EULA", "Y")
          .WithEnvironment("MSSQL_SA_PASSWORD", "StrongPassword!1")
          .WithName("sqledge") 
          .WithPortBinding(1433, true)
          .Build();

        _serviceBusContainer = new ContainerBuilder()
              .WithImage("mcr.microsoft.com/azure-messaging/servicebus-emulator:latest")
              .WithEnvironment("ACCEPT_EULA", "Y")
              .WithEnvironment("SQL_SERVER", "sqledge") 
              .WithEnvironment("MSSQL_SA_PASSWORD", "StrongPassword!1")
              .WithPortBinding(5672, true)
              .WithPortBinding(9354, true)
              .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Emulator Service is Successfully Up!"))
              .Build();

    }

    public string ConnectionString => $"Endpoint=sb://localhost:5672;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;";

    public async Task InitializeAsync()
    {
        await _sqlContainer.StartAsync();
        await _serviceBusContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _serviceBusContainer.StopAsync();
        await _sqlContainer.StopAsync();
    }
}