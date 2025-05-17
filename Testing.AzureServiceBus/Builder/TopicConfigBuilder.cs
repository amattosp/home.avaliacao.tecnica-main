using Testing.AzureServiceBus.Configuration;

namespace Testing.AzureServiceBus.Builder;

public class TopicConfigBuilder(TopicConfig topicConfig)
{
    public TopicConfigBuilder AddSubscription(string name, Action<SubscriptionConfigBuilder>? configureSubscription = null)
    {
        SubscriptionConfig subscription = new() { Name = name };
        configureSubscription?.Invoke(new SubscriptionConfigBuilder(subscription));

        topicConfig.Subscriptions.Add(subscription);
        return this;
    }
}