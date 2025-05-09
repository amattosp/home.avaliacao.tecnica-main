namespace Home.AvaliacaoTecnica.Application.Pedido.ListarPorStatus;

public class ListarPedidosResponse
{
    public int Id { get; set; } 
    public int PedidoId { get; set; }
    public int ClienteId { get; set; }
    public List<ItemPedidoResult> Itens { get; set; } = new();
    public string Status { get; set; } = string.Empty;
    public decimal Imposto { get; set; } = 0;
}

public class ItemPedidoResult
{
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public decimal Valor { get; set; }
}
