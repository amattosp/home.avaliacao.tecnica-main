using Testing.AzureServiceBus.Configuration;

namespace Testing.AzureServiceBus.Builder;

public class SubscriptionConfigBuilder(SubscriptionConfig subscriptionConfig)
{
    public SubscriptionConfigBuilder AddRule(
        string name,
        Action<RuleConfigBuilder> configureRule)
    {
        RuleConfig rule = new() { Name = name };
        configureRule(new RuleConfigBuilder(rule));
        subscriptionConfig.Rules.Add(rule);
        return this;
    }
}