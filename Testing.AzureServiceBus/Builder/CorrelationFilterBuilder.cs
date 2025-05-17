using Testing.AzureServiceBus.Configuration;

namespace Testing.AzureServiceBus.Builder;

public class CorrelationFilterBuilder(CorrelationFilter correlationFilter)
{
    public CorrelationFilterBuilder WithJsonContentType()
    {
        correlationFilter.ContentType = "application/json";
        return this;
    }
    
    public CorrelationFilterBuilder WithCloudEventsContentType()
    {
        correlationFilter.ContentType = "application/cloudevents+json";
        return this;
    }
    
    public CorrelationFilterBuilder WithProperties(string key, string value)
    {
        correlationFilter.Properties.Add(key, value);
        return this;
    }
}