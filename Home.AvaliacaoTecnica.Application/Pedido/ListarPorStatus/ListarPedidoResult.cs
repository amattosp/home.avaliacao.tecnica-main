namespace Home.AvaliacaoTecnica.Application.Pedido.ListarPorStatus;

public class ListarPedidosResponse
{
    public int PedidoId { get; set; }
    public int ClienteId { get; set; }
    public List<ItemPedidoResult> Itens { get; set; } = new();
}

public class ItemPedidoResult
{
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public decimal Valor { get; set; }
}
