using FluentAssertions;
using Home.AvaliacaoTecnica.Domain.Core.Exceptions;
using Home.AvaliacaoTecnica.Domain.Entities;

namespace Home.AvaliacaoTecnica.Domain.Unit.Aggregates;

public class ItemPedidoTests
{
    [Fact]
    public void Deve_Criar_ItemPedido_Com_Valores_Validos()
    {
        // Arrange
        int produtoId = 1;
        int quantidade = 5;
        decimal valor = 10.50m;

        // Act
        var itemPedido = new ItemPedido(produtoId, quantidade, valor);

        // Assert
        itemPedido.ProdutoId.Should().Be(produtoId);
        itemPedido.Quantidade.Should().Be(quantidade);
        itemPedido.Valor.Should().Be(valor);
    }

    [Theory]
    [InlineData(0, 5, 10.50, "ProdutoId deve ser maior que zero.")]
    [InlineData(-1, 5, 10.50, "ProdutoId deve ser maior que zero.")]
    [InlineData(1, 0, 10.50, "Quantidade deve ser maior que zero.")]
    [InlineData(1, -5, 10.50, "Quantidade deve ser maior que zero.")]
    [InlineData(1, 5, 0, "Valor deve ser maior que zero.")]
    [InlineData(1, 5, -10.50, "Valor deve ser maior que zero.")]
    public void Deve_Lancar_BusinessRuleException_Para_Valores_Invalidos(
        int produtoId, int quantidade, decimal valor, string mensagemEsperada)
    {
        // Act
        var act = () => new ItemPedido(produtoId, quantidade, valor);

        // Assert
        act.Should().Throw<BusinessRuleException>()
            .WithMessage(mensagemEsperada);
    }
}