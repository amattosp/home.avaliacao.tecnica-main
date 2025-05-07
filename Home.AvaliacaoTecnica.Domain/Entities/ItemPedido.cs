namespace Home.AvaliacaoTecnica.Domain.Entities;

public class ItemPedido
{
    public int ProdutoId { get; }
    public int Quantidade { get; }
    public decimal Valor { get; }

    public ItemPedido(int produtoId, int quantidade, decimal valor)
    {
        ProdutoId = produtoId;
        Quantidade = quantidade;
        Valor = valor;
    }
}
