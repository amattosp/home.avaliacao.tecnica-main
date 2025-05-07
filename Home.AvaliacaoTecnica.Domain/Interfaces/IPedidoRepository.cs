using Home.AvaliacaoTecnica.Domain.Entities;

namespace Home.AvaliacaoTecnica.Domain.Interfaces;

public interface IPedidoRepository
{
    Task AdicionarAsync(PedidoEnviado pedido);
    Task<List<PedidoEnviado>> ObterPorStatusAsync(string status);
    Task<PedidoEnviado?> ObterPorIdAsync(int id);
}