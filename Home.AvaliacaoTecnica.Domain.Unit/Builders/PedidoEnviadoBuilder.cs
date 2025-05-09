using Bogus;
using Home.AvaliacaoTecnica.Application.Pedido.EnviarPedido;

namespace Home.AvaliacaoTecnica.Domain.Unit.Builders;
public class PedidoEnviadoBuilder
{
    private readonly Faker _faker;
    private readonly List<EnviarItemPedidoCommand> _items;

    public PedidoEnviadoBuilder()
    {
        _faker = new Faker();
        _items = new List<EnviarItemPedidoCommand>();
    }

    public PedidoEnviadoBuilder AddItem(int? produtoId = null, int? quantidade = null, decimal? valor = null)
    {
        _items.Add(new EnviarItemPedidoCommand
        {
            ProdutoId = produtoId ?? _faker.Random.Int(1, 100),
            Quantidade = quantidade ?? _faker.Random.Int(1, 10),
            Valor = valor ?? _faker.Random.Decimal(1, 100)
        });

        return this;
    }

    public PedidoEnviadoBuilder AddItems(int count)
    {
        for (int i = 0; i < count; i++)
        {
            AddItem();
        }

        return this;
    }

    public List<EnviarItemPedidoCommand> Build()
    {
        return _items;
    }
}