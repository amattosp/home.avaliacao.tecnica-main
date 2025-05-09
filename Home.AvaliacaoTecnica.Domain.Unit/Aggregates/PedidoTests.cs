using FluentAssertions;
using Home.AvaliacaoTecnica.Domain.Core.Exceptions;
using Home.AvaliacaoTecnica.Domain.Entities;

namespace Home.AvaliacaoTecnica.Domain.Unit.Aggregates;

public class PedidoTest
{
    [Fact]
    public void Pedido_DeveLancarExcecao_SePedidoIdForMenorOuIgualAZero()
    {
        // Arrange
        var clienteId = 1;
        var itens = new List<ItemPedido> { new ItemPedido(1, 1, 10.0m) };

        // Act
        var act = () => new Pedido(0, clienteId, itens);

        // Assert
        act.Should().Throw<BusinessRuleException>();
    }

    [Fact]
    public void Pedido_DeveLancarExcecao_SeClienteIdForMenorOuIgualAZero()
    {
        // Arrange
        var pedidoId = 1;
        var itens = new List<ItemPedido> { new ItemPedido(1, 1, 10.0m) };

        // Act
        var act = () => new Pedido(pedidoId, 0, itens);

        // Assert
        act.Should().Throw<BusinessRuleException>();
    }

    [Fact]
    public void Pedido_DeveLancarExcecao_SeItensForNulo()
    {
        // Arrange
        var pedidoId = 1;
        var clienteId = 1;

        // Act
        var act = () => new Pedido(pedidoId, clienteId, null);

        // Assert
        act.Should().Throw<BusinessRuleException>();
    }

    [Fact]
    public void Pedido_DeveLancarExcecao_SeItensForVazio()
    {
        // Arrange
        var pedidoId = 1;
        var clienteId = 1;
        var itens = new List<ItemPedido>();

        // Act
        var act = () => new Pedido(pedidoId, clienteId, itens);

        // Assert
        act.Should().Throw<BusinessRuleException>();
    }


    [Fact]
    public void Pedido_DeveSerCriadoComSucesso_SeTodosOsParametrosForemValidos()
    {
        // Arrange
        var pedidoId = 1;
        var clienteId = 1;
        var itens = new List<ItemPedido> { new ItemPedido(1, 1, 10.0m) };

        // Act
        var pedido = new Pedido(pedidoId, clienteId, itens);

        // Assert
        pedido.PedidoId.Should().Be(pedidoId);
        pedido.ClienteId.Should().Be(clienteId);
        pedido.Itens.Should().BeEquivalentTo(itens);
    }
}