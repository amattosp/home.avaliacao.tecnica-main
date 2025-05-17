namespace Testing.AzureServiceBus.Configuration;

public class UserConfig
{
    public List<NamespaceConfig> Namespaces { get; set; } = [];
    public LoggingConfig Logging { get; set; } = new();
}