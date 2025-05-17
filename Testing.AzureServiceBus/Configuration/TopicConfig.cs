namespace Testing.AzureServiceBus.Configuration;

public class TopicConfig
{
    public string? Name { get; set; }
    public TopicProperties Properties { get; set; } = new();
    public List<SubscriptionConfig> Subscriptions { get; set; } = [];
}
