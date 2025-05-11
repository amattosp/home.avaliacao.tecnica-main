namespace Testing.AzureServiceBus.Configuration;

public class RuleConfig
{
    public string? Name { get; set; }
    public RuleProperties Properties { get; set; } = new();
}