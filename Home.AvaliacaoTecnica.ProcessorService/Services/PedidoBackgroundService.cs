using Home.AvaliacaoTecnica.ProcessorService.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Home.AvaliacaoTecnica.ProcessorService.Services;

public class PedidoBackgroundService : BackgroundService
{
    private readonly IServiceBusConsumer _consumer;
    private readonly ILogger<PedidoBackgroundService> _logger;

    public PedidoBackgroundService(IServiceBusConsumer consumer, ILogger<PedidoBackgroundService> logger)
    {
        _consumer = consumer;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Aguardando recebimento da mensagem...");
        await _consumer.StartProcessingAsync(ProcessarPedidoAsync, stoppingToken);
    }

    private Task ProcessarPedidoAsync(string mensagem)
    {
        _logger.LogInformation($"Pedido recebido: {mensagem}");
        return Task.CompletedTask;
    }
}