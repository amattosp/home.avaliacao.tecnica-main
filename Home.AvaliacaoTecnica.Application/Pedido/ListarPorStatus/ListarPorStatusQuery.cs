using MediatR;

namespace Home.AvaliacaoTecnica.Application.Pedido.ListarPorStatus;

public class ListarPorStatusQuery : IRequest<IEnumerable<ListarPedidosResponse>>
{
    public string Status { get; set; }

    public ListarPorStatusQuery(string status)
    {
        Status = status;
    }
}

