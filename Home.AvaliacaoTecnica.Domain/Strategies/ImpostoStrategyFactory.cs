using Home.AvaliacaoTecnica.Domain.Interfaces;

namespace Home.AvaliacaoTecnica.Domain.Strategies;

public class ImpostoStrategyFactory
{
    private readonly ImpostoVigenteStrategy _vigente;
    private readonly ImpostoReformaTributariaStrategy _reforma;

    public ImpostoStrategyFactory(ImpostoVigenteStrategy vigente, ImpostoReformaTributariaStrategy reforma)
    {
        _vigente = vigente;
        _reforma = reforma;
    }

    public IImpostoStrategy GetStrategy(bool usarReformaTributaria)
        => usarReformaTributaria ? _reforma : _vigente;
}