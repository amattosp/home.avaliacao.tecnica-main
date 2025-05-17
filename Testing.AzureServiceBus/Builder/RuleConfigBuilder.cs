using Testing.AzureServiceBus.Configuration;

namespace Testing.AzureServiceBus.Builder;

public class RuleConfigBuilder(RuleConfig ruleConfig)
{
    public RuleConfigBuilder WithFilterType(RuleFilterType ruleFilterType)
    {
        ruleConfig.Properties.FilterType = ruleFilterType.ToString();
        return this;
    }
    
    public RuleConfigBuilder WithCorrelationFilter(Action<CorrelationFilterBuilder> configureCorrelationFilter)
    {
        CorrelationFilter correlationFilter = new();
        configureCorrelationFilter(new CorrelationFilterBuilder(correlationFilter));
        ruleConfig.Properties.CorrelationFilter = correlationFilter;
        return this;
    }
}