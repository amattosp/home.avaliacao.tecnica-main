namespace Testing.AzureServiceBus.Builder;

public enum RuleFilterType
{
    Correlation,
    [Obsolete("Not available for the moment. https://github.com/Azure/azure-service-bus-emulator-installer/issues/54")]
    Sql
}