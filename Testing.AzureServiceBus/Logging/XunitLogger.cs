using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Testing.AzureServiceBus.Logging;

public class XunitLogger<TCategory>(
    ITestOutputHelper outputHelper,
    LogLevel minimumLogLevel = LogLevel.Debug)
    : ILogger<TCategory>, IDisposable
{
    private readonly ITestOutputHelper _outputHelper =
        outputHelper ?? throw new ArgumentNullException(nameof(outputHelper));

    public IDisposable BeginScope<TState>(
        TState state)
        => this;

    public bool IsEnabled(
        LogLevel logLevel)
        => logLevel >= minimumLogLevel;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        string message = formatter(
            state,
            exception);
        try
        {
            _outputHelper.WriteLine($"[{logLevel}] {message}");
            if (exception != null)
            {
                _outputHelper.WriteLine(exception.ToString());
            }
        }
        catch (InvalidOperationException)
        {
            // Suppress exceptions if output is unavailable (e.g., test completed).
        }
    }

    public void Dispose()
    {
        // No resources to dispose
    }
}