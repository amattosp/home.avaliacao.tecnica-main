using FluentAssertions;
using Home.AvaliacaoTecnica.Domain.Strategies;

namespace Home.AvaliacaoTecnica.Domain.Unit;

public class ImpostoStrategyTests
{
    [Theory]
    [InlineData(100, 30)] // Exemplo: 100 * 0.3 = 30
    [InlineData(200, 60)] // Exemplo: 200 * 0.3 = 60
    [InlineData(0, 0)]    // Exemplo: 0 * 0.3 = 0
    public void ImpostoVigenteStrategy_CalcularImposto_DeveRetornarValorEsperado(decimal totalItens, decimal valorEsperado)
    {
        // Arrange
        var strategy = new ImpostoVigenteStrategy();

        // Act
        var resultado = strategy.CalcularImposto(totalItens);

        // Assert
        resultado.Should().Be(valorEsperado);
    }


    [Theory]
    [InlineData(-100, -30)] // Exemplo: -100 * 0.3 = -30
    [InlineData(-200, -60)] // Exemplo: -200 * 0.3 = -60
    public void ImpostoVigenteStrategy_CalcularImposto_DeveLidarComValoresNegativos(decimal totalItens, decimal valorEsperado)
    {
        // Arrange
        var strategy = new ImpostoVigenteStrategy();

        // Act
        var resultado = strategy.CalcularImposto(totalItens);

        // Assert
        resultado.Should().Be(valorEsperado);
    }


    [Theory]
    [InlineData(1000000000, 300000000)] // Exemplo: 1 bilhão * 0.3 = 300 milhões
    [InlineData(2000000000, 600000000)] // Exemplo: 2 bilhões * 0.3 = 600 milhões
    public void ImpostoVigenteStrategy_CalcularImposto_DeveLidarComValoresMuitoGrandes(decimal totalItens, decimal valorEsperado)
    {
        // Arrange
        var strategy = new ImpostoVigenteStrategy();

        // Act
        var resultado = strategy.CalcularImposto(totalItens);

        // Assert
        resultado.Should().Be(valorEsperado);
    }


    [Theory]
    [InlineData(100.12345, 30.04)] // Exemplo: 100.12345 * 0.3 = 30.037035 -> Arredondado para 30.04
    [InlineData(200.98765, 60.30)] // Exemplo: 200.98765 * 0.3 = 60.296295 -> Arredondado para 60.30
    public void ImpostoVigenteStrategy_CalcularImposto_DeveArredondarCorretamente(decimal totalItens, decimal valorEsperado)
    {
        // Arrange
        var strategy = new ImpostoVigenteStrategy();

        // Act
        var resultado = strategy.CalcularImposto(totalItens);

        // Assert
        resultado.Should().Be(valorEsperado);
    }

    [Theory]
    [InlineData(100.12345, 20.02)] // Exemplo: 100.12345 * 0.2 = 20.02469 -> Arredondado para 20.02
    [InlineData(200.98765, 40.20)] // Exemplo: 200.98765 * 0.2 = 40.19753 -> Arredondado para 40.20
    public void ImpostoReformaTributariaStrategy_CalcularImposto_DeveArredondarCorretamente(decimal totalItens, decimal valorEsperado)
    {
        // Arrange
        var strategy = new ImpostoReformaTributariaStrategy();

        // Act
        var resultado = strategy.CalcularImposto(totalItens);

        // Assert
        resultado.Should().Be(valorEsperado);
    }

    [Theory]
    [InlineData(100, 20)] // Exemplo: 100 * 0.2 = 20
    [InlineData(200, 40)] // Exemplo: 200 * 0.2 = 40
    [InlineData(0, 0)]    // Exemplo: 0 * 0.2 = 0
    public void ImpostoReformaTributariaStrategy_CalcularImposto_DeveRetornarValorEsperado(decimal totalItens, decimal valorEsperado)
    {
        // Arrange
        var strategy = new ImpostoReformaTributariaStrategy();

        // Act
        var resultado = strategy.CalcularImposto(totalItens);

        // Assert
        resultado.Should().Be(valorEsperado);
    }

    [Theory]
    [InlineData(-100, -20)] // Exemplo: -100 * 0.2 = -20
    [InlineData(-200, -40)] // Exemplo: -200 * 0.2 = -40
    public void ImpostoReformaTributariaStrategy_CalcularImposto_DeveLidarComValoresNegativos(decimal totalItens, decimal valorEsperado)
    {
        // Arrange
        var strategy = new ImpostoReformaTributariaStrategy();

        // Act
        var resultado = strategy.CalcularImposto(totalItens);

        // Assert
        resultado.Should().Be(valorEsperado);
    }

    [Theory]
    [InlineData(1000000000, 200000000)] // Exemplo: 1 bilhão * 0.2 = 200 milhões
    [InlineData(2000000000, 400000000)] // Exemplo: 2 bilhões * 0.2 = 400 milhões
    public void ImpostoReformaTributariaStrategy_CalcularImposto_DeveLidarComValoresMuitoGrandes(decimal totalItens, decimal valorEsperado)
    {
        // Arrange
        var strategy = new ImpostoReformaTributariaStrategy();

        // Act
        var resultado = strategy.CalcularImposto(totalItens);

        // Assert
        resultado.Should().Be(valorEsperado);
    }

}