
using Home.AvaliacaoTecnica.Domain.Interfaces;

namespace Home.AvaliacaoTecnica.Domain.Services;

public class PedidoService : IPedidoService
{
    private readonly IImpostoStrategy _estrategiaImposto;

    public PedidoService(IImpostoStrategy estrategiaImposto)
    {
        _estrategiaImposto = estrategiaImposto;
    }

    public decimal CalcularImposto(Entities.Pedido pedido)
    {
        var totalItens = pedido.Itens.Sum(i => i.Valor);
        return _estrategiaImposto.CalcularImposto(totalItens);
    }

    public string GetImpostoStrategyName()
    {
        return _estrategiaImposto.GetType().Name;
    }
}
