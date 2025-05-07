using AutoMapper;
using Home.AvaliacaoTecnica.Domain.Interfaces;
using MediatR;
using Serilog;

namespace Home.AvaliacaoTecnica.Application.Pedido.ListarPorId;

public class ListarPedidoPorIdQueryHandler : IRequestHandler<ListarPedidoPorIdQuery, PedidoResponse>
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public ListarPedidoPorIdQueryHandler(IPedidoRepository pedidoRepository, IMapper mapper, ILogger logger)
    {
        _pedidoRepository = pedidoRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PedidoResponse> Handle(ListarPedidoPorIdQuery request, CancellationToken cancellationToken)
    {
        var pedidos = await _pedidoRepository.ObterPorIdAsync(request.PedidoId);

        // Verificar se há pedidos duplicados com o mesmo ID
        if (pedidos.Count > 1)
        {
            _logger.Error("Erro: Mais de um pedido encontrado com o ID {PedidoId}.", request.PedidoId);
            throw new InvalidOperationException($"Não pode haver pedidos duplicados com o ID {request.PedidoId}.");
        }

        // Verificar se nenhum pedido foi encontrado
        if (!pedidos.Any())
        {
            _logger.Warning("Aviso: Nenhum pedido encontrado com o ID {PedidoId}.", request.PedidoId);
            throw new KeyNotFoundException($"Pedido com ID {request.PedidoId} não foi encontrado.");
        }

        var pedidoResponse = _mapper.Map<PedidoResponse>(pedidos.First());

        return pedidoResponse;
    }
}
