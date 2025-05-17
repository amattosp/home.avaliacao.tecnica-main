namespace Testing.AzureServiceBus.Configuration;

public class RuleProperties
{
    public string? FilterType { get; set; }
    public CorrelationFilter? CorrelationFilter { get; set; }
}