using System.Text.Json;
using Testing.AzureServiceBus.Configuration;

namespace Testing.AzureServiceBus;

internal static class FileUtils
{
    internal static void SaveUserConfig(ServiceBusConfiguration serviceBusConfiguration)
    {
        string json = JsonSerializer.Serialize(serviceBusConfiguration);
        File.WriteAllText(
            Path.Combine(
                AppContext.BaseDirectory,
                "Config.json"),
            json);
    }
}