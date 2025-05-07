using Azure.Messaging.ServiceBus;
using Home.AvaliacaoTecnica.Contracts.Contratos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Home.AvaliacaoTecnica.Consumer
{
    public class ConsumerWorker : BackgroundService
    {
        private readonly ILogger<ConsumerWorker> _logger;
        private readonly IConfiguration _configuration;
        private ServiceBusProcessor _processor;
        private ServiceBusClient _client;

        public ConsumerWorker(ILogger<ConsumerWorker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            var connectionString = _configuration.GetConnectionString("ServiceBus");
            _client = new ServiceBusClient(connectionString);
            _processor = _client.CreateProcessor("pedidos-processados", "sistemaB", new ServiceBusProcessorOptions());

            _processor.ProcessMessageAsync += ProcessarMensagemAsync;
            _processor.ProcessErrorAsync += TratarErroAsync;

            await _processor.StartProcessingAsync(cancellationToken);

            _logger.LogInformation("ConsumerWorker iniciado e escutando 'pedidos-processados'.");
        }

        private async Task ProcessarMensagemAsync(ProcessMessageEventArgs args)
        {
            var json = args.Message.Body.ToString();
            var pedido = JsonSerializer.Deserialize<PedidoProcessadoDto>(json);

            _logger.LogInformation($"Pedido recebido: {pedido.Id}, Status: {pedido.Status}, Total: {pedido.ValorTotal} Imposto: {pedido.Imposto}");

            await args.CompleteMessageAsync(args.Message);
        }

        private Task TratarErroAsync(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception, "Erro ao consumir pedido processado.");
            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _processor.StopProcessingAsync(cancellationToken);
            await _processor.DisposeAsync();
            await _client.DisposeAsync();

            _logger.LogInformation("ConsumerWorker finalizado.");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.CompletedTask;
    }

}
