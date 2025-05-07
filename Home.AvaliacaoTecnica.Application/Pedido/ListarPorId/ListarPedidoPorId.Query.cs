using MediatR;

namespace Home.AvaliacaoTecnica.Application.Pedido.ListarPorId;

public class ListarPedidoPorIdQuery : IRequest<PedidoResponse>
{
    public int PedidoId { get; set; }
    public ListarPedidoPorIdQuery(int pedidoId)
    {
        PedidoId = pedidoId;
    }
}
