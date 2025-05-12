using AutoMapper;
using Azure.Messaging.ServiceBus;
using Home.AvaliacaoTecnica.Application.Services;
using Home.AvaliacaoTecnica.Domain.Entities;
using Home.AvaliacaoTecnica.Domain.Factory;
using Home.AvaliacaoTecnica.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Azure;
using Serilog;
using System.Text.Json;

namespace Home.AvaliacaoTecnica.Application.Pedido.EnviarPedido;

public class EnviarPedidoCommandHandler : IRequestHandler<EnviarPedidoCommand, EnviarPedidoResult>
{
    private readonly ILogger _logger;
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IMapper _mapper;
    private readonly ServiceBusSender _messageSender;

    public EnviarPedidoCommandHandler(IServiceBusSenderFactory senderFactory,
                                      IAzureClientFactory<ServiceBusSender> serviceBusFactory,
                                      ILogger logger,
                                      IPedidoRepository pedidoRepository,
                                      IMapper mapper)
    {
        ArgumentNullException.ThrowIfNull(serviceBusFactory);

        _logger = logger;
        _pedidoRepository = pedidoRepository;
        _mapper = mapper;

        _messageSender = serviceBusFactory.CreateClient("TopicSender");
    }

    public async Task<EnviarPedidoResult> Handle(EnviarPedidoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var json = JsonSerializer.Serialize(request);

            var itens = _mapper.Map<List<PedidoItemEnviado>>(request.Itens);
            var pedidoEnviado = PedidoEnviadoFactory.CriarDeCommand(
                pedidoId: request.PedidoId,
                clienteId: request.ClienteId,
                status: "Criado",
                itensCommand: itens
            );

            await _pedidoRepository.AdicionarAsync(pedidoEnviado);
            _logger.Information("Pedido {Id} registrado na base de dados com sucesso", request.PedidoId);

            await _messageSender.SendMessageAsync(new ServiceBusMessage(json));
            _logger.Information("Pedido {Id} enviado ao tópico com sucesso", request.PedidoId);

            return new EnviarPedidoResult
            {
                PedidoId = request.PedidoId
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao enviar o pedido {Id} para o tópico", request.PedidoId);
            throw;
        }
    }
}