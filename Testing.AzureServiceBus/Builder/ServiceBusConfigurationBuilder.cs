using Testing.AzureServiceBus.Configuration;

namespace Testing.AzureServiceBus.Builder;

public class ServiceBusConfigurationBuilder()
{
    public static ServiceBusConfigurationBuilder Create() => new();
    private readonly ServiceBusConfiguration _serviceBusConfiguration = new();

    public ServiceBusConfigurationBuilder AddDefaultNamespace(Action<NamespaceConfigBuilder> configureNamespace)
    {
        NamespaceConfig namespaceConfig = new();
        configureNamespace(new NamespaceConfigBuilder(namespaceConfig));
        _serviceBusConfiguration.UserConfig.Namespaces.Add(namespaceConfig);
        return this;
    }

    public ServiceBusConfiguration Build()
        => _serviceBusConfiguration;
}
