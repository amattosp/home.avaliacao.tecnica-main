namespace Testing.AzureServiceBus.Configuration;

public class SubscriptionConfig
{
    public string? Name { get; set; }
    public SubscriptionProperties Properties { get; set; } = new();
    public List<RuleConfig> Rules { get; set; } = [];
}