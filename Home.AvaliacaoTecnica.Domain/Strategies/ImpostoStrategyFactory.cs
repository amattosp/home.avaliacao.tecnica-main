using Home.AvaliacaoTecnica.Domain.Interfaces;
using Pedido.Processor.Domain.Strategies;
using Microsoft.Extensions.DependencyInjection;

namespace Home.AvaliacaoTecnica.Domain.Strategies
{
    public class ImpostoStrategyFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ImpostoStrategyFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IImpostoStrategy GetStrategy(bool usarReformaTributaria)
        {
            return usarReformaTributaria
                ? _serviceProvider.GetRequiredService<ImpostoReformaTributariaStrategy>()
                : _serviceProvider.GetRequiredService<ImpostoVigenteStrategy>();
        }
    }

}
