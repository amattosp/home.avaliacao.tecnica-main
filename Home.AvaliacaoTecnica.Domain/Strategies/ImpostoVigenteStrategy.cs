
using Home.AvaliacaoTecnica.Domain.Interfaces;

namespace Pedido.Processor.Domain.Strategies;

public class ImpostoVigenteStrategy : IImpostoStrategy
{
    private const decimal Fator = 0.3m;
    private const int quantidadeCasasDecimais = 2;

    public decimal CalcularImposto(decimal totalItens)
    {
        return decimal.Round(totalItens * Fator, quantidadeCasasDecimais, MidpointRounding.AwayFromZero);
    }
}
