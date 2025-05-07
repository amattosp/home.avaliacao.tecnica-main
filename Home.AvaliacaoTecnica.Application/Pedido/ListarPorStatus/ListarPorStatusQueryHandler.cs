using MediatR;
using AutoMapper;
using Home.AvaliacaoTecnica.Domain.Interfaces;
using Home.AvaliacaoTecnica.Domain.Entities;

namespace Home.AvaliacaoTecnica.Application.Pedido.ListarPorStatus;

public class ListarPorStatusQueryHandler : IRequestHandler<ListarPorStatusQuery, IEnumerable<ListarPedidosResponse>>
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IMapper _mapper;

    public ListarPorStatusQueryHandler(IPedidoRepository pedidoRepository,
                                       IMapper mapper)
    {
        _pedidoRepository = pedidoRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ListarPedidosResponse>> Handle(ListarPorStatusQuery request, CancellationToken cancellationToken)
    {
        var pedidos = await _pedidoRepository.ObterPorStatusAsync(request.Status);
        
        // Simulando dados fake para teste pois ainda nao estamos gravando no banco de dados
        pedidos = PedidoEnviadoFakeData.GetFakePedidos();

        if (pedidos == null || !pedidos.Any())
            throw new ArgumentException("Nenhum pedido encontrado com o status informado.");


        var pedidosResponse = _mapper.Map<IEnumerable<ListarPedidosResponse>>(pedidos);

        return pedidosResponse;
    }
}

//todo: retirar depois que conseguir gravar no banco de dados
public static class PedidoEnviadoFakeData
{
    public static List<PedidoEnviado> GetFakePedidos()
    {
        return new List<PedidoEnviado>
        {
            new PedidoEnviado
            {
                Id = 1,
                PedidoId = 1001,
                ClienteId = 2001,
                Status = "Enviado",
                EnviadoEm = DateTime.UtcNow.AddDays(-1),
                Itens = new List<PedidoItemEnviado>
                {
                    new PedidoItemEnviado
                    {
                        Id = 1,
                        ProdutoId = 3001,
                        Quantidade = 2,
                        Valor = 50.00m,
                        PedidoEnviadoId = 1
                    },
                    new PedidoItemEnviado
                    {
                        Id = 2,
                        ProdutoId = 3002,
                        Quantidade = 1,
                        Valor = 30.00m,
                        PedidoEnviadoId = 1
                    }
                }
            },
            new PedidoEnviado
            {
                Id = 2,
                PedidoId = 1002,
                ClienteId = 2002,
                Status = "Pendente",
                EnviadoEm = DateTime.UtcNow.AddDays(-2),
                Itens = new List<PedidoItemEnviado>
                {
                    new PedidoItemEnviado
                    {
                        Id = 3,
                        ProdutoId = 3003,
                        Quantidade = 5,
                        Valor = 20.00m,
                        PedidoEnviadoId = 2
                    }
                }
            }
        };
    }
}

