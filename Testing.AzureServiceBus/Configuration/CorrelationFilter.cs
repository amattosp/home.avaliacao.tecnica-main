namespace Testing.AzureServiceBus.Configuration;

public class CorrelationFilter
{
    public string ContentType { get; set; } = "application/json";
    public Dictionary<string, string> Properties { get; set; } = new();
}
