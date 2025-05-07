using MediatR;

namespace Home.AvaliacaoTecnica.Application.Pedido.EnviarPedido;

public class EnviarPedidoCommand : IRequest<EnviarPedidoResult>
{

    public EnviarPedidoCommand() {}

    public EnviarPedidoCommand(int pedidoId, int clientId, List<EnviarItemPedidoCommand> items)
    {
        PedidoId = pedidoId;
        ClientId = clientId;
        Items = items;
    }

    public int PedidoId { get; set; }
    public int ClientId { get; set; }
    public List<EnviarItemPedidoCommand> Items { get; set; } = [];
}
