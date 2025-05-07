using Azure.Messaging.ServiceBus;
using Home.AvaliacaoTecnica.Application.Services;
using MediatR;
using Serilog;
using System.Text.Json;

namespace Home.AvaliacaoTecnica.Application.Pedido.EnviarPedido;

public class EnviarPedidoCommandHandler : IRequestHandler<EnviarPedidoCommand, EnviarPedidoResult>
{
    private readonly IServiceBusSenderFactory _senderFactory;
    private readonly ILogger _logger;

    public EnviarPedidoCommandHandler(IServiceBusSenderFactory senderFactory, ILogger logger)
    {
        _senderFactory = senderFactory;
        _logger = logger;
    }

    public async Task<EnviarPedidoResult> Handle(EnviarPedidoCommand request, CancellationToken cancellationToken)
    {
        var sender = _senderFactory.CreateSender("pedidos");

        var json = JsonSerializer.Serialize(request);
        await sender.SendMessageAsync(new ServiceBusMessage(json));
        _logger.Information("Pedido {Id} enviado ao topico com sucesso", request.PedidoId);

        return new EnviarPedidoResult
        {
            PedidoId = request.PedidoId
        };
    }
}
