using Home.AvaliacaoTecnica.Domain.Entities;

namespace Home.AvaliacaoTecnica.Domain.Factory;

public static class PedidoEnviadoFactory
{
    private static PedidoEnviado Criar(int pedidoId, int clienteId, DateTime enviadoEm, List<PedidoItemEnviado> itens)
    {
        return new PedidoEnviado
        {
            PedidoId = pedidoId,
            ClienteId = clienteId,
            EnviadoEm = enviadoEm,
            Itens = itens
        };
    }

    public static PedidoEnviado CriarDeCommand(int pedidoId, int clienteId, string status, List<PedidoItemEnviado> itensCommand)
    {
        var itens = MapearItens(itensCommand);

        var pedido = Criar(pedidoId, clienteId, DateTime.UtcNow, itens);
        pedido.Status = status;

        return pedido;
    }

    private static List<PedidoItemEnviado> MapearItens(List<PedidoItemEnviado> itens)
    {
        return itens.Select(item => new PedidoItemEnviado
        {
            ProdutoId = item.ProdutoId,
            Quantidade = item.Quantidade,
            Valor = item.Valor
        }).ToList();
    }
}
