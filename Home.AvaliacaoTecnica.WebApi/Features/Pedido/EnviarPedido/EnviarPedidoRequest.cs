namespace Home.AvaliacaoTecnica.WebApi.Features.Pedido.EnviarPedido;

public class EnviarPedidoRequest
{
    

    public int PedidoId { get; set; }
    public int ClienteId { get; set; }
    public required List<EnviarItemPedidoRequest> Itens { get; set; }
}
