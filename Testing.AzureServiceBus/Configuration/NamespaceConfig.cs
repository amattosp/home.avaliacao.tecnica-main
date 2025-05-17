namespace Testing.AzureServiceBus.Configuration;

public class NamespaceConfig
{
    public string Name { get; set; } = "sbemulatorns";
    public List<QueueConfig> Queues { get; set; } = [];
    public List<TopicConfig> Topics { get; set; } = [];
}
