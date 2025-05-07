using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pedido.Contracts.Contratos;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Home.AvaliacaoTecnica.Domain.Interfaces;
using Home.AvaliacaoTecnica.Domain.Entities;
using Home.AvaliacaoTecnica.Domain.Services;
using Home.AvaliacaoTecnica.Contracts.Contratos;

namespace Home.AvaliacaoTecnica.Processor.Service;
public class PedidoWorker : BackgroundService
{
    private readonly ILogger<PedidoWorker> _logger;
    private readonly IConfiguration _configuration;
    private readonly IImpostoStrategy _impostoStrategy;
    private readonly IPedidoService _pedidoService;
    private ServiceBusProcessor _processor;
    private ServiceBusClient _client;

    public PedidoWorker(ILogger<PedidoWorker> logger, IConfiguration configuration, IImpostoStrategy impostoStrategy, IPedidoService pedidoService)
    {
        _logger = logger;
        _configuration = configuration;
        _impostoStrategy = impostoStrategy;
        _pedidoService = pedidoService;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        var connectionString = _configuration.GetConnectionString("ServiceBus");
        _client = new ServiceBusClient(connectionString);
        _processor = _client.CreateProcessor("pedidos", "processador", new ServiceBusProcessorOptions());

        _processor.ProcessMessageAsync += ProcessarPedidoAsync;
        _processor.ProcessErrorAsync += TratarErroAsync;

        await _processor.StartProcessingAsync(cancellationToken);

        _logger.LogInformation("Processador de pedidos iniciado.");
    }

    private async Task ProcessarPedidoAsync(ProcessMessageEventArgs args)
    {
        var json = args.Message.Body.ToString();
        var dto = JsonSerializer.Deserialize<PedidoRequestDto>(json);

        var pedido = new Domain.Entities.Pedido(
            dto.PedidoId,
            dto.ClienteId,
            [.. dto.Itens.Select(i => new ItemPedido(i.ProdutoId, i.Quantidade, i.Valor))]
        );

        _logger.LogInformation($"Processando pedido: {pedido.ToLogString()}");

        var imposto = _pedidoService.CalcularImposto(pedido);

        var pedidoProcessado = new PedidoProcessadoDto
        {
            Id = pedido.PedidoId,
            ClientId = pedido.ClienteId,
            Status = "Criado",
            ValorTotal = pedido.Itens.Sum(i => i.Valor),
            Imposto = imposto
        };

        var sender = _client.CreateSender("pedidos-processados");
        var msg = new ServiceBusMessage(JsonSerializer.Serialize(pedidoProcessado));

        await sender.SendMessageAsync(msg);
        await args.CompleteMessageAsync(args.Message);

        _logger.LogInformation($"Pedido processado com sucesso: ID {pedido.PedidoId}, CientId: {pedido.ClienteId} Valor Total R${pedidoProcessado.ValorTotal:F2} Imposto R${pedidoProcessado.Imposto:F2}");
    }

    private Task TratarErroAsync(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Erro ao processar pedido");
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _processor.StopProcessingAsync(cancellationToken);
        await _processor.DisposeAsync();
        await _client.DisposeAsync();

        _logger.LogInformation("Processador de pedidos finalizado.");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.CompletedTask;
}
