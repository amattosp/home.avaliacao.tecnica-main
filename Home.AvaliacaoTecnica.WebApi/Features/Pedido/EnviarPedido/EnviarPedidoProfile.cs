using AutoMapper;
using Home.AvaliacaoTecnica.Application.Pedido.EnviarPedido;

namespace Home.AvaliacaoTecnica.WebApi.Features.Pedido.EnviarPedido;

public class EnviarPedidoProfile : Profile
{
    public EnviarPedidoProfile()
    {
        CreateMap<EnviarPedidoRequest, EnviarPedidoCommand>()
            .ForMember(dest => dest.ClienteId, opt => opt.MapFrom(src => src.ClienteId))
            .ForMember(dest => dest.Itens, opt => opt.MapFrom(src => src.Itens));
        CreateMap<EnviarItemPedidoRequest, EnviarItemPedidoCommand>()
            .ForMember(dest => dest.ProdutoId, opt => opt.MapFrom(src => src.ProdutoId))
            .ForMember(dest => dest.Quantidade, opt => opt.MapFrom(src => src.Quantidade))
            .ForMember(dest => dest.Valor, opt => opt.MapFrom(src => src.Valor));
        CreateMap<EnviarPedidoResult, EnviarPedidoResponse>();
    }
}