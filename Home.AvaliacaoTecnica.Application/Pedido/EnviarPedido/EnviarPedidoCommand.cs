using MediatR;

namespace Home.AvaliacaoTecnica.Application.Pedido.EnviarPedido;

public class EnviarPedidoCommand : IRequest<EnviarPedidoResult>
{

    public EnviarPedidoCommand() {}

    public EnviarPedidoCommand(int pedidoId, int clienteId, List<EnviarItemPedidoCommand> itens)
    {
        PedidoId = pedidoId;
        ClienteId = clienteId;
        Itens = itens;
    }

    public int PedidoId { get; set; }
    public int ClienteId { get; set; }
    public List<EnviarItemPedidoCommand> Itens { get; set; } = [];
}
