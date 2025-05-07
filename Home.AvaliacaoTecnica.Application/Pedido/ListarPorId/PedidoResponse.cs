namespace Home.AvaliacaoTecnica.Application.Pedido.ListarPorId;

public class PedidoResponse
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public int ClienteId { get; set; }
    public decimal Imposto { get; set; }
    public List<ItemPedidoResponse> Itens { get; set; } = new();
    public string Status { get; set; } = string.Empty;
}

public class ItemPedidoResponse
{
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public decimal Valor { get; set; }
}
