using Azure.Messaging.ServiceBus;
using Azure.Messaging;
using Microsoft.Extensions.Azure;
namespace Home.AvaliacaoTecnica.WebApi.MessagingServices;

public class TopicSender
{
    private readonly ServiceBusSender _messageSender;
    private readonly ILogger _logger;

    public TopicSender(
        IAzureClientFactory<ServiceBusSender> serviceBusFactory,
        ILogger<TopicSender> logger)
    {
        ArgumentNullException.ThrowIfNull(serviceBusFactory);

        _messageSender = serviceBusFactory.CreateClient("TopicSender");
        _logger = logger;
    }

    public async Task SendMessageAsync()
    {
        CloudEvent cloudEvent = new(
            source: "https://testapi.com/events",
            type: "sl.testapi.topic.message",
            jsonSerializableData: new MessageArgs("Hello World!"));

        ServiceBusMessage message = new(new BinaryData(cloudEvent))
        {
            ContentType = "application/cloudevents+json",
            ApplicationProperties =
        {
            { "MyProperty", "MyValue" }
        }
        };

        await _messageSender.SendMessageAsync(message);
        _logger.LogInformation("Message sent to Azure Service Bus.");
    }
}
