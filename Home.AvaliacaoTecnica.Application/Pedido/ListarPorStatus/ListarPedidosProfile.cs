using AutoMapper;
using Home.AvaliacaoTecnica.Domain.Entities;

namespace Home.AvaliacaoTecnica.Application.Pedido.ListarPorStatus
{
    public class ListarPedidosProfile : Profile
    {
        public ListarPedidosProfile()
        {
            CreateMap<PedidoEnviado, ListarPedidosResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PedidoId, opt => opt.MapFrom(src => src.PedidoId))
                .ForMember(dest => dest.ClienteId, opt => opt.MapFrom(src => src.ClienteId))
                .ForMember(dest => dest.Itens, opt => opt.MapFrom(src => src.Itens))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

            CreateMap<PedidoItemEnviado, ItemPedidoResult>()
                .ForMember(dest => dest.ProdutoId, opt => opt.MapFrom(src => src.ProdutoId))
                .ForMember(dest => dest.Quantidade, opt => opt.MapFrom(src => src.Quantidade))
                .ForMember(dest => dest.Valor, opt => opt.MapFrom(src => src.Valor));
        }
    }
}
