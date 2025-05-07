using Home.AvaliacaoTecnica.Infra.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Home.AvaliacaoTecnica.Infra.Data.Context;

public class PedidoDbContext : DbContext
{
    public PedidoDbContext(DbContextOptions<PedidoDbContext> options) : base(options) { }

    public DbSet<PedidoEnviado> Pedidos { get; set; }
    public DbSet<PedidoItemEnviado> Itens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PedidoEnviado>()
            .HasMany(p => p.Itens)
            .WithOne(i => i.PedidoEnviado)
            .HasForeignKey(i => i.PedidoEnviadoId);
    }
}
