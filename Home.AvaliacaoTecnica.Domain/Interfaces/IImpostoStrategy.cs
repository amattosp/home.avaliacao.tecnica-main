namespace Home.AvaliacaoTecnica.Domain.Interfaces;

public interface IImpostoStrategy
{
    decimal CalcularImposto(decimal totalItens);
}
