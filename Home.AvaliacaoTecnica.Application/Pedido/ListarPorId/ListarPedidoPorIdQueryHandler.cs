using MediatR;
using AutoMapper;
using Home.AvaliacaoTecnica.Domain.Interfaces;
using Home.AvaliacaoTecnica.Domain.Entities;

namespace Home.AvaliacaoTecnica.Application.Pedido.ListarPorId;

public class ListarPedidoPorIdQueryHandler : IRequestHandler<ListarPedidoPorIdQuery, PedidoResponse>
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IMapper _mapper;

    public ListarPedidoPorIdQueryHandler(IPedidoRepository pedidoRepository, IMapper mapper)
    {
        _pedidoRepository = pedidoRepository;
        _mapper = mapper;
    }

    public async Task<PedidoResponse> Handle(ListarPedidoPorIdQuery request, CancellationToken cancellationToken)
    {
        var pedido = await _pedidoRepository.ObterPorIdAsync(request.PedidoId);

        //todo: retirar depois que conseguir gravar no banco de dados
        pedido = PedidoEnviadoFakeData.GetFakePedido();

        if (pedido == null)
        {
            throw new KeyNotFoundException($"Pedido com ID {request.PedidoId} não foi encontrado.");
        }

        var pedidoResponse = _mapper.Map<PedidoResponse>(pedido);

        return pedidoResponse;
    }
}

public static class PedidoEnviadoFakeData
{
    public static PedidoEnviado GetFakePedido()
    {
        return new PedidoEnviado
        {
            Id = 1,
            PedidoId = 1,
            ClienteId = 1,
            Status = "Criado",
            EnviadoEm = DateTime.UtcNow,
            Itens = new List<PedidoItemEnviado>
                {
                    new PedidoItemEnviado
                    {
                        Id = 1,
                        ProdutoId = 1001,
                        Quantidade = 2,
                        Valor = 52.70m,
                        PedidoEnviadoId = 1
                    }
                }
        };
    }
}