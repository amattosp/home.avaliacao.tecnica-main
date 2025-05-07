namespace Home.AvaliacaoTecnica.Contracts.Contratos
{
    public class PedidoProcessadoDto
    {
        public int Id { get; set; }
        public required string Status { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal Imposto { get; set; }
        public int ClientId { get; set; }
    }
}
