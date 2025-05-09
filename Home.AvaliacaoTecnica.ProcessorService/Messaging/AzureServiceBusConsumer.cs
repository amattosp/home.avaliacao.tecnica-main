using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
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

public class AzureServiceBusConsumer : IServiceBusConsumer
{
    private ServiceBusClient _client;
    private ServiceBusProcessor _processor;
    private readonly ILogger<AzureServiceBusConsumer> _logger;
    private readonly IConfiguration _configuration;
    private readonly IPedidoService _pedidoService;

    public AzureServiceBusConsumer(
        ILogger<AzureServiceBusConsumer> logger,
        IConfiguration configuration,
        IImpostoStrategy impostoStrategy,
        IPedidoService pedidoService)
    {
        _logger = logger;
        _configuration = configuration;
        _pedidoService = pedidoService;
    }

    public async Task StartProcessingAsync(Func<string, Task> onMessageReceived, CancellationToken cancellationToken)
    {
        try
        {
            var connectionString = _configuration.GetConnectionString("ServiceBus");
            _client = new ServiceBusClient(connectionString);
            _processor = _client.CreateProcessor("pedidos", "processador", new ServiceBusProcessorOptions());

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
        // Incrementa o contador de falhas e apos 3 tentativas envia para a Dead Letter Queue
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
        _logger.LogInformation($"Pedido processado com sucesso: ID {pedido.PedidoId}, CientId: {pedido.ClienteId} Valor Total R${pedidoProcessado.ValorTotal:F2} Imposto R${pedidoProcessado.Imposto:F2}");
    }
}
