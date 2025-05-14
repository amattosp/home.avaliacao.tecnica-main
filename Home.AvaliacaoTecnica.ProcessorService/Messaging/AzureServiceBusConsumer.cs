using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Home.AvaliacaoTecnica.Domain.Interfaces;
using Home.AvaliacaoTecnica.Domain.Entities;
using Home.AvaliacaoTecnica.Domain.Services;
using Home.AvaliacaoTecnica.Contracts.Contratos;
using Pedido.Contracts.Contratos;
using System;
using System.Collections.Generic;

namespace Home.AvaliacaoTecnica.ProcessorService.Messaging;

public class AzureServiceBusConsumer : IServiceBusConsumer, IAsyncDisposable
{
    private ServiceBusClient _client;
    private ServiceBusProcessor _processor;
    private ServiceBusSender _sender;
    private readonly ILogger<AzureServiceBusConsumer> _logger;
    private readonly string _connectionString;
    private readonly IPedidoService _pedidoService;
    private readonly string _topicName = "pedidos";
    private readonly string _subscriptionName = "processador";
    private bool _disposed;

    public AzureServiceBusConsumer(
        ILogger<AzureServiceBusConsumer> logger,
        string connectionString,
        IImpostoStrategy impostoStrategy,
        IPedidoService pedidoService)
    {
        _logger = logger;
        _connectionString = connectionString;
        _pedidoService = pedidoService;
    }

    public async Task StartProcessingAsync(Func<string, Task> onMessageReceived, CancellationToken cancellationToken)
    {
        try
        {
            _client = new ServiceBusClient(_connectionString);
            _processor = _client.CreateProcessor(_topicName, _subscriptionName, new ServiceBusProcessorOptions());
            _sender = _client.CreateSender("pedidos-processados");

            _processor.ProcessMessageAsync += async args =>
            {
                var messageBody = args.Message.Body.ToString();
                _logger.LogInformation($"Mensagem recebida: {messageBody}");

                try
                {
                    // Processa o pedido
                    await ProcessarPedidoAsync(args);
                    await onMessageReceived(messageBody);
                    await args.CompleteMessageAsync(args.Message);
                }
                catch (Exception ex)
                {
                    await HandleMessageFailureAsync(args, messageBody, ex);
                }
            };

            _processor.ProcessErrorAsync += args =>
            {
                _logger.LogError($"Erro no Service Bus: {args.Exception.Message}");
                return Task.CompletedTask;
            };

            await _processor.StartProcessingAsync(cancellationToken);

            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000, cancellationToken);
            }

            await _processor.StopProcessingAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao iniciar o processador: {ex.Message}");
            throw;
        }
    }

    private async Task HandleMessageFailureAsync(ProcessMessageEventArgs args, string messageBody, Exception ex)
    {
        int failureCount = 0;
        if (args.Message.ApplicationProperties.TryGetValue("FailureCount", out var failureCountObj))
        {
            failureCount = Convert.ToInt32(failureCountObj);
        }
        failureCount++;

        if (failureCount >= 3)
        {
            _logger.LogWarning($"Mensagem descartada após 3 falhas: {messageBody}");
            await args.DeadLetterMessageAsync(args.Message, "Falhas consecutivas no processamento.");
        }
        else
        {
            _logger.LogError(ex, $"Erro ao processar mensagem. Tentativa {failureCount}/3.");
            var abandonProperties = new Dictionary<string, object>(args.Message.ApplicationProperties)
            {
                ["FailureCount"] = failureCount
            };

            await args.AbandonMessageAsync(args.Message, abandonProperties);
        }
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

        if (_pedidoService is PedidoService pedidoService)
        {
            var strategyName = pedidoService.GetImpostoStrategyName();
            _logger.LogInformation($"Política de imposto utilizada: {strategyName}");
            System.Diagnostics.Debug.WriteLine($"Política de imposto utilizada: {strategyName}");
        }

        var imposto = _pedidoService.CalcularImposto(pedido);

        var pedidoProcessado = new PedidoProcessadoDto
        {
            Id = pedido.PedidoId,
            ClientId = pedido.ClienteId,
            Status = "Criado",
            ValorTotal = pedido.Itens.Sum(i => i.Valor),
            Imposto = imposto
        };

        var msg = new ServiceBusMessage(JsonSerializer.Serialize(pedidoProcessado));
        await _sender.SendMessageAsync(msg);
        _logger.LogInformation($"Pedido processado com sucesso: ID {pedido.PedidoId}, CientId: {pedido.ClienteId} Valor Total R${pedidoProcessado.ValorTotal:F2} Imposto R${pedidoProcessado.Imposto:F2}");
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;

        if (_processor != null)
        {
            await _processor.DisposeAsync();
        }
        if (_sender != null)
        {
            await _sender.DisposeAsync();
        }
        if (_client != null)
        {
            await _client.DisposeAsync();
        }
    }
}
