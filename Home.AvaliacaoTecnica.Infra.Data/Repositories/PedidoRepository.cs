﻿using Home.AvaliacaoTecnica.Domain.Entities;
using Home.AvaliacaoTecnica.Domain.Interfaces;
using Home.AvaliacaoTecnica.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;


namespace Home.AvaliacaoTecnica.Infra.Data.Repositories;

public class PedidoRepository : IPedidoRepository
{
    private readonly PedidoDbContext _context;

    public PedidoRepository(PedidoDbContext context)
    {
        _context = context;
    }

    public async Task AdicionarAsync(PedidoEnviado pedido)
    {
        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync();
    }

    public async Task<List<PedidoEnviado>> ObterPorStatusAsync(string status)
    {
        return await _context.Pedidos
            .Include(p => p.Itens)
            .Where(p => p.Status.Equals(status, StringComparison.OrdinalIgnoreCase))
            .ToListAsync();
    }

    public async Task<List<PedidoEnviado>> ObterPorIdAsync(int id)
    {
        return await _context.Pedidos
            .Include(p => p.Itens)
            .Where(p => p.PedidoId == id)
            .ToListAsync();
    }
}
