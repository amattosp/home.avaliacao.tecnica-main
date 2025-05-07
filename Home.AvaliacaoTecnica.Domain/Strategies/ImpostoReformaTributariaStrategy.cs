
using Home.AvaliacaoTecnica.Domain.Interfaces;

namespace Home.AvaliacaoTecnica.Domain.Strategies;

public class ImpostoReformaTributariaStrategy : IImpostoStrategy
{
    private const decimal Fator = 0.2m;
    private const int QuantidadeCasasDecimais = 2;

    public decimal CalcularImposto(decimal totalItens)
    {
        return decimal.Round(totalItens * Fator, QuantidadeCasasDecimais, MidpointRounding.AwayFromZero);
    }
}
