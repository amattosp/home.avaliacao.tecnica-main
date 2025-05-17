using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Testing.AzureServiceBus.Logging;

public class XUnitLoggerProvider<T>(
    ITestOutputHelper outputHelper) : ILoggerProvider
{
    public ILogger CreateLogger(
        string categoryName)
        => new XunitLogger<T>(outputHelper);

    public void Dispose()
    {
        // No resources to dispose
    }
}