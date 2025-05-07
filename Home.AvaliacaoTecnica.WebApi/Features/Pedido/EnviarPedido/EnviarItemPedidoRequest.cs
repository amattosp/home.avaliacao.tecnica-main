namespace Home.AvaliacaoTecnica.WebApi.Features.Pedido.EnviarPedido;

public class EnviarItemPedidoRequest
{
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public decimal Valor { get; set; }
}
