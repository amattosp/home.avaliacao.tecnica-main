using AutoMapper;
using Azure.Messaging.ServiceBus;
using Bogus;
using FluentAssertions;
using Home.AvaliacaoTecnica.Application.Pedido.EnviarPedido;
using Home.AvaliacaoTecnica.Application.Services.Wrapper;
using Home.AvaliacaoTecnica.Domain.Entities;
using Home.AvaliacaoTecnica.Domain.Factory;
using Home.AvaliacaoTecnica.Domain.Interfaces;
using Home.AvaliacaoTecnica.Domain.Unit.Builders;
using NSubstitute;
using Serilog;

namespace Home.AvaliacaoTecnica.Domain.Unit.EnviarPedido;

public class EnviarPedidoCommandHandlerTests
{
    private const string TopicSender = "TopicSender";
    private readonly IServiceBusSenderWrapper _messageSender;
    private readonly ILogger _loggerMock;
    private readonly IPedidoRepository _pedidoRepositoryMock;
    private readonly IMapper _mapperMock;
    private readonly EnviarPedidoCommandHandler _handler;

    public EnviarPedidoCommandHandlerTests()
    {
        _messageSender = Substitute.For<IServiceBusSenderWrapper>();
        _loggerMock = Substitute.For<ILogger>();
        _pedidoRepositoryMock = Substitute.For<IPedidoRepository>();
        _mapperMock = Substitute.For<IMapper>();

        _handler = new EnviarPedidoCommandHandler(
            _messageSender,
            _loggerMock,
            _pedidoRepositoryMock,
            _mapperMock
        );
    }

    [Fact]
    public async Task Handle_Should_AddPedidoToRepository_And_SendMessageToServiceBus()
    {
        // Arrange
        var faker = new Faker();
        var command = new EnviarPedidoCommand(
            pedidoId: faker.Random.Int(1, 1000),
            clienteId: faker.Random.Int(1, 1000),
            itens: new PedidoEnviadoBuilder()
                .AddItems(1)
                .Build()
        );

        var pedidoEnviado = PedidoEnviadoFactory.CriarDeCommand(
            pedidoId: command.PedidoId,
            clienteId: command.ClienteId,
            status: "Criado",
            itensCommand: new List<PedidoItemEnviado>()
        );

        _mapperMock.Map<List<PedidoItemEnviado>>(command.Itens).Returns(pedidoEnviado.Itens);
        _messageSender.ConfigureSender(TopicSender);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _pedidoRepositoryMock.Received(1).AdicionarAsync(Arg.Is<PedidoEnviado>(p =>
            p.PedidoId == command.PedidoId &&
            p.ClienteId == command.ClienteId &&
            p.Status == "Criado"
        ));

        await _messageSender.Received(1).SendMessageAsync(Arg.Is<ServiceBusMessage>(m =>
            m.Body.ToString().Contains($"\"PedidoId\":{command.PedidoId}")
        ), Arg.Any<CancellationToken>());

        result.PedidoId.Should().Be(command.PedidoId);
        _loggerMock.Received(1).Information("Pedido {Id} registrado na base de dados com sucesso", command.PedidoId);
        _loggerMock.Received(1).Information("Pedido {Id} enviado ao tópico com sucesso", command.PedidoId);
    }


    [Fact]
    public async Task Handle_Should_LogError_When_ServiceBusFails()
    {
        // Arrange
        var faker = new Faker();
        var command = new EnviarPedidoCommand(
            pedidoId: faker.Random.Int(1, 1000),
            clienteId: faker.Random.Int(1, 1000),
            itens: new PedidoEnviadoBuilder()
                .AddItems(1)
                .Build()
        );

        var mappedItems = new List<PedidoItemEnviado>
    {
        new PedidoItemEnviado
        {
            Id = faker.Random.Int(1, 1000),
            ProdutoId = command.Itens[0].ProdutoId,
            Quantidade = command.Itens[0].Quantidade,
            Valor = command.Itens[0].Valor
        }
    };

        _mapperMock.Map<List<PedidoItemEnviado>>(command.Itens).Returns(mappedItems);
        _messageSender.ConfigureSender(TopicSender);

        // Simula uma falha ao enviar a mensagem
        _messageSender.When(x => x.SendMessageAsync(Arg.Any<ServiceBusMessage>(), Arg.Any<CancellationToken>()))
                      .Do(x => throw new Exception("Service Bus Error"));

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Service Bus Error");
        _loggerMock.Received(1).Error(Arg.Any<Exception>(), "Erro ao enviar o pedido {Id} para o tópico", command.PedidoId);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_When_CommandIsInvalid()
    {
        // Arrange
        var command = new EnviarPedidoCommand(0, 0, new List<EnviarItemPedidoCommand>());

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task Handle_Should_LogError_When_RepositoryFails()
    {
        // Arrange
        var faker = new Faker();
        var command = new EnviarPedidoCommand(
            pedidoId: faker.Random.Int(1, 1000),
            clienteId: faker.Random.Int(1, 1000),
            itens: new PedidoEnviadoBuilder().AddItems(1).Build()
        );

        var mappedItems = new List<PedidoItemEnviado>
        {
            new PedidoItemEnviado
            {
                Id = faker.Random.Int(1, 1000),
                ProdutoId = command.Itens[0].ProdutoId,
                Quantidade = command.Itens[0].Quantidade,
                Valor = command.Itens[0].Valor
            }
        };

        _mapperMock.Map<List<PedidoItemEnviado>>(command.Itens).Returns(mappedItems);
        _pedidoRepositoryMock.When(x => x.AdicionarAsync(Arg.Any<PedidoEnviado>()))
                             .Do(x => throw new Exception("Repository Error"));

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Repository Error");
    }
}