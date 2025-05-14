namespace Home.AvaliacaoTecnica.Domain.Services;

public interface IPedidoService
{
    decimal CalcularImposto(Entities.Pedido pedido);

    string GetImpostoStrategyName();
}
