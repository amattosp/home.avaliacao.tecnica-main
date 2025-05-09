using Home.AvaliacaoTecnica.Domain.Core.Exceptions;

namespace Home.AvaliacaoTecnica.Domain.Entities;

public class Pedido
{
    public int PedidoId { get; }
    public int ClienteId { get; }
    public List<ItemPedido> Itens { get; }

    public Pedido(int pedidoId, int clienteId, List<ItemPedido> itens)
    {
        if (pedidoId <= 0)
            throw new BusinessRuleException("PedidoId deve ser maior que zero.");

        if (clienteId <= 0)
            throw new BusinessRuleException("ClienteId deve ser maior que zero.");

        if (itens == null || !itens.Any())
            throw new BusinessRuleException("O pedido deve conter pelo menos um item.");

        if (itens.Any(i => i.Quantidade <= 0))
            throw new BusinessRuleException("Todos os itens devem ter quantidade maior que zero.");

        if (itens.Any(i => i.Valor <= 0))
            throw new BusinessRuleException("Todos os itens devem ter valor maior que zero.");

        PedidoId = pedidoId;
        ClienteId = clienteId;
        Itens = itens;
    }

    public string ToLogString()
    {
        var itensLog = string.Join(", ", Itens.Select(i =>
            $"[ProdutoId: {i.ProdutoId}, Quantidade: {i.Quantidade}, Valor: {i.Valor}]"));

        return $"PedidoId: {PedidoId}, ClienteId: {ClienteId}, Itens: {itensLog}";
    }
}
