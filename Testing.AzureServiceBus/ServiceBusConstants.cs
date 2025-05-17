namespace Testing.AzureServiceBus;

public static class ServiceBusConstants
{
    public static readonly string ConnectionString =
        "Endpoint=sb://127.0.0.1;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;";

    public static readonly string LocalServiceBusClientName = nameof(LocalServiceBusClientName);
}