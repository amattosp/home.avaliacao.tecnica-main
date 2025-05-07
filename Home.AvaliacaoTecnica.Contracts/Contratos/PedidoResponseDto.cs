namespace Home.AvaliacaoTecnica.Contracts.Contratos;

public class PedidoResponseDto
{
    public required int PedidoId { get; set; }
    public required int ClienteId { get; set; }
    public required string Status { get; set; }
    public required DateTime EnviadoEm { get; set; }
    public required List<PedidoItemResponseDto> Itens { get; set; }
}

public class PedidoItemResponseDto
{
    public required int ProdutoId { get; set; }
    public required int Quantidade { get; set; }
    public required decimal Valor { get; set; }
}