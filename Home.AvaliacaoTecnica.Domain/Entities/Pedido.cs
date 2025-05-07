namespace Home.AvaliacaoTecnica.Domain.Entities;

public class Pedido
{
    public int PedidoId { get; }
    public int ClienteId { get; }
    public List<ItemPedido> Itens { get; }

    public Pedido(int pedidoId, int clienteId, List<ItemPedido> itens)
    {
        PedidoId = pedidoId;
        ClienteId = clienteId;
        Itens = itens;
    }

    public decimal CalcularValorVigente()
    {
        var total = Itens.Sum(i => i.Quantidade * i.Valor);
        return total * 0.3m; 
    }

    public string ToLogString()
    {
        var itensLog = string.Join(", ", Itens.Select(i =>
            $"[ProdutoId: {i.ProdutoId}, Quantidade: {i.Quantidade}, Valor: {i.Valor}]"));

        return $"PedidoId: {PedidoId}, ClienteId: {ClienteId}, Itens: {itensLog}";
    }
}
