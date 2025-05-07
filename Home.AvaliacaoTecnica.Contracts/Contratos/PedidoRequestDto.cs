using Home.AvaliacaoTecnica.Contracts.Contratos;

namespace Pedido.Contracts.Contratos
{
    public class PedidoRequestDto
    {
        public int PedidoId { get; set; }
        public int ClienteId { get; set; }
        public required List<ItemPedidoDto> Itens { get; set; }
    }
}
