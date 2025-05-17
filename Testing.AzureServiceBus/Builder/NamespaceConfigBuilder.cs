using Testing.AzureServiceBus.Configuration;

namespace Testing.AzureServiceBus.Builder;

public class NamespaceConfigBuilder(NamespaceConfig namespaceConfig)
{
    public NamespaceConfigBuilder AddQueue(string name)
    {
        namespaceConfig.Queues.Add(new QueueConfig { Name = name });
        return this;
    }

    public NamespaceConfigBuilder AddTopic(
        string name,
        Action<TopicConfigBuilder> configureTopic)
    {
        TopicConfig topic = new() { Name = name };
        configureTopic(new TopicConfigBuilder(topic));
        namespaceConfig.Topics.Add(topic);
        return this;
    }
}