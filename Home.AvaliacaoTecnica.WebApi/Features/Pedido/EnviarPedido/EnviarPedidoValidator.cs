using FluentValidation;

namespace Home.AvaliacaoTecnica.WebApi.Features.Pedido.EnviarPedido;

public class EnviarPedidoValidator : AbstractValidator<EnviarPedidoRequest>
{
    public EnviarPedidoValidator()
    {
        RuleFor(x => x.PedidoId).NotEmpty();
        RuleFor(x => x.ClienteId).NotEmpty();
        RuleFor(x => x.Itens).NotEmpty();
    }
}
