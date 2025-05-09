using System;
using System.Threading;
using System.Threading.Tasks;

namespace Home.AvaliacaoTecnica.ProcessorService.Messaging;

public interface IServiceBusConsumer
{
    Task StartProcessingAsync(Func<string, Task> onMessageReceived, CancellationToken cancellationToken);
}
