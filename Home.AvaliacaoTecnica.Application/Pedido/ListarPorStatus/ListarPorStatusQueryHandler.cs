using MediatR;
using AutoMapper;
using Home.AvaliacaoTecnica.Domain.Interfaces;

namespace Home.AvaliacaoTecnica.Application.Pedido.ListarPorStatus;

public class ListarPorStatusQueryHandler : IRequestHandler<ListarPorStatusQuery, IEnumerable<ListarPedidosResponse>>
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IMapper _mapper;

    public ListarPorStatusQueryHandler(IPedidoRepository pedidoRepository,
                                       IMapper mapper)
    {
        _pedidoRepository = pedidoRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ListarPedidosResponse>> Handle(ListarPorStatusQuery request, CancellationToken cancellationToken)
    {
        var pedidos = await _pedidoRepository.ObterPorStatusAsync(request.Status);
        
        if (pedidos == null || !pedidos.Any())
            throw new ArgumentException("Nenhum pedido encontrado com o status informado.");

        var pedidosResponse = _mapper.Map<IEnumerable<ListarPedidosResponse>>(pedidos);

        return pedidosResponse;
    }
}