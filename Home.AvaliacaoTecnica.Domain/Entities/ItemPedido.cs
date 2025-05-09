using Home.AvaliacaoTecnica.Domain.Core.Exceptions;

namespace Home.AvaliacaoTecnica.Domain.Entities;

public class ItemPedido
{
    public int ProdutoId { get; }
    public int Quantidade { get; }
    public decimal Valor { get; }

    public ItemPedido(int produtoId, int quantidade, decimal valor)
    {
        if (produtoId <= 0)
            throw new BusinessRuleException("ProdutoId deve ser maior que zero.");

        if (quantidade <= 0)
            throw new BusinessRuleException("Quantidade deve ser maior que zero.");

        if (valor <= 0)
            throw new BusinessRuleException("Valor deve ser maior que zero.");

        ProdutoId = produtoId;
        Quantidade = quantidade;
        Valor = valor;
    }
}