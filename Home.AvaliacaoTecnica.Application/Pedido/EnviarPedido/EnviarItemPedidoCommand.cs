namespace Home.AvaliacaoTecnica.Application.Pedido.EnviarPedido;

public class EnviarItemPedidoCommand
{
    public int ProductId { get; set; }
    public int Quantidade { get; set; }
    public decimal Valor { get; set; }
}
