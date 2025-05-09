using Bogus;
using Home.AvaliacaoTecnica.Domain.Entities;

namespace Home.AvaliacaoTecnica.Tests.Integrations.DataBuilders;

public class PedidoEnviadoDataBuilder
{
    private readonly Faker<PedidoEnviado> _pedidoEnviadoFaker;
    private readonly Faker<PedidoItemEnviado> _pedidoItemEnviadoFaker;
    private int? _pedidoId;
    private int? _id; 
    private string? _status;

    public PedidoEnviadoDataBuilder()
    {
        _pedidoItemEnviadoFaker = new Faker<PedidoItemEnviado>()
            .RuleFor(i => i.Id, f => f.UniqueIndex + 1)
            .RuleFor(i => i.ProdutoId, f => f.Random.Int(1, 1000))
            .RuleFor(i => i.Quantidade, f => f.Random.Int(1, 10))
            .RuleFor(i => i.Valor, f => f.Finance.Amount(10, 100));

        _pedidoEnviadoFaker = new Faker<PedidoEnviado>()
            .RuleFor(p => p.Id, f => _id ?? f.IndexFaker + 1) 
            .RuleFor(p => p.PedidoId, f => _pedidoId ?? f.Random.Int(1, 1000))
            .RuleFor(p => p.ClienteId, f => f.Random.Int(1, 500))
            .RuleFor(p => p.Status, f => _status ?? f.PickRandom(new[] { "Criado", "Enviado", "Cancelado" }))
            .RuleFor(p => p.EnviadoEm, f => f.Date.Past(1))
            .RuleFor(p => p.Itens, f => _pedidoItemEnviadoFaker.Generate(f.Random.Int(1, 5)));
    }

    public PedidoEnviadoDataBuilder ComPedidoId(int pedidoId)
    {
        _pedidoId = pedidoId;
        return this;
    }

    public PedidoEnviadoDataBuilder ComId(int id)
    {
        _id = id; 
        return this;
    }

    public PedidoEnviado Build()
    {
        var pedidoEnviado = _pedidoEnviadoFaker.Generate();

        foreach (var item in pedidoEnviado.Itens)
        {
            item.PedidoEnviado = pedidoEnviado;
        }

        return pedidoEnviado;
    }

    public List<PedidoEnviado> BuildList(int quantidade)
    {
        return Enumerable.Range(1, quantidade).Select(_ => Build()).ToList();
    }

    public PedidoEnviadoDataBuilder ComStatus(string status)
    {
        _status = status;
        return this;
    }
}