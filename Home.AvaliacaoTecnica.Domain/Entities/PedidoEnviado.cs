namespace Home.AvaliacaoTecnica.Domain.Entities;
public class PedidoEnviado
{
    public int Id { get; set; } 
    public int PedidoId { get; set; }
    public int ClienteId { get; set; }
    public string Status { get; set; }
    public required DateTime EnviadoEm
    {
        get; set;
    }

    public required List<PedidoItemEnviado> Itens
    {
        get; set;
    }
}

public class PedidoItemEnviado
{
    public int Id { get; set; } // PK
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public decimal Valor { get; set; }

    public int PedidoEnviadoId { get; set; } // FK
    public PedidoEnviado PedidoEnviado { get; set; }
}