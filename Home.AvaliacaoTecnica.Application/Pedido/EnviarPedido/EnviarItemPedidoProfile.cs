using AutoMapper;
using Home.AvaliacaoTecnica.Application.Pedido.EnviarPedido;
using Home.AvaliacaoTecnica.Domain.Entities;

namespace Home.AvaliacaoTecnica.Domain.Mappers
{
    public class EnviarItemPedidoProfile : Profile
    {
        public EnviarItemPedidoProfile()
        {
            CreateMap<EnviarItemPedidoCommand, PedidoItemEnviado>()
                .ForMember(dest => dest.ProdutoId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.Quantidade, opt => opt.MapFrom(src => src.Quantidade))
                .ForMember(dest => dest.Valor, opt => opt.MapFrom(src => src.Valor))
                .ForMember(dest => dest.PedidoEnviadoId, opt => opt.Ignore()) 
                .ForMember(dest => dest.PedidoEnviado, opt => opt.Ignore()); 
        }
    }
}
