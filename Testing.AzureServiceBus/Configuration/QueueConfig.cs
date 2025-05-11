namespace Testing.AzureServiceBus.Configuration;

public class QueueConfig
{
    public string? Name { get; set; }
    public QueueProperties Properties { get; set; } = new();
}