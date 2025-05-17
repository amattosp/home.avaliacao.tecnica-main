namespace Testing.AzureServiceBus.Configuration;

public class QueueProperties
{
    public bool DeadLetteringOnMessageExpiration { get; set; } = false;
    public string DefaultMessageTimeToLive { get; set; } = "PT1H";
    public string DuplicateDetectionHistoryTimeWindow { get; set; } = "PT20S";
    public string ForwardDeadLetteredMessagesTo { get; set; } = "";
    public string ForwardTo { get; set; } = "";
    public string LockDuration { get; set; } = "PT1M";
    public int MaxDeliveryCount { get; set; } = 3;
    public bool RequiresDuplicateDetection { get; set; } = false;
    public bool RequiresSession { get; set; } = false;
}
