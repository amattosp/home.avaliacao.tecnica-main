using Azure.Messaging.ServiceBus;
using Home.AvaliacaoTecnica.Contracts.Contratos;
using Home.AvaliacaoTecnica.Infra.Data.Entities;
using Home.AvaliacaoTecnica.Infra.Data.Repositories;
using Home.AvalicaoTecnica.WebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Pedido.Contracts.Contratos;
using System.Text.Json;

namespace Home.AvalicaoTecnica.WebApi.Controllers;

/// <summary>
/// Controlador responsável pelo envio e consulta de pedidos.
/// </summary>
[ApiController]
[Route("api/pedidos")]
public class PedidoController : ControllerBase
{
    private readonly Serilog.ILogger _logger;
    private readonly ServiceBusSenderFactory _senderFactory;
    private readonly PedidoRepository _store;

    public PedidoController(Serilog.ILogger logger,
                            ServiceBusSenderFactory serviceBusSenderFactory,
                            PedidoRepository store)
    {
        _logger = logger;
        _senderFactory = serviceBusSenderFactory;
        _store = store;
    }

    /// <summary>
    /// Lista pedido por Status
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PedidoEnviado>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<IActionResult> ListarPorStatus([FromQuery] string status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return BadRequest("Status é obrigatório.");

        var pedidos = await _store.ObterPorStatusAsync(status);
        return Ok(pedidos);
    }

    /// <summary>
    /// Lista pedido por Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PedidoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var pedido = await _store.ObterPorIdAsync(id);
        if (pedido == null)
            return NotFound(new ProblemDetails
            {
                Title = "Pedido não encontrado",
                Status = 404,
                Detail = $"Nenhum pedido com o ID {id} foi localizado."
            });

        var response = new PedidoResponseDto
        {
            PedidoId = pedido.PedidoId,
            ClienteId = pedido.ClienteId,
            Status = pedido.Status,
            EnviadoEm = pedido.EnviadoEm,
            Itens = pedido.Itens.Select(i => new PedidoItemResponseDto
            {
                ProdutoId = i.ProdutoId,
                Quantidade = i.Quantidade,
                Valor = i.Valor
            }).ToList()
        };

        return Ok(response);
    }

    /// <summary>
    /// Cria um novo pedido e envia para o barramento de mensagens.
    /// </summary>
    /// <returns>Dados do pedido enviado</returns>
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<IActionResult> CriarPedido([FromBody] PedidoRequestDto pedidoRequest)
    {
        if (pedidoRequest == null || pedidoRequest.Itens == null || !pedidoRequest.Itens.Any())
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Pedido inválido",
                Detail = "O pedido e seus itens devem ser informados.",
                Status = 400
            });
        }

        try
        {
            var sender = _senderFactory.CreateSender("pedidos");

            var json = JsonSerializer.Serialize(pedidoRequest);
            await sender.SendMessageAsync(new ServiceBusMessage(json));
            _logger.Information("Pedido {Id} enviado ao topico com sucesso", pedidoRequest.PedidoId);

            //todo: Criar chamada para command para registrar o pedido no banco de dados em memoria
            var pedidoRegistrado = new PedidoEnviado
            {
                PedidoId = pedidoRequest.PedidoId,
                ClienteId = pedidoRequest.ClienteId,
                Status = "Criado",
                EnviadoEm = DateTime.UtcNow,
                Itens = pedidoRequest.Itens.Select(i => new PedidoItemEnviado
                {
                    ProdutoId = i.ProdutoId,
                    Quantidade = i.Quantidade,
                    Valor = i.Valor
                }).ToList()
            };

            await _store.AdicionarAsync(pedidoRegistrado);

            return CreatedAtAction(nameof(CriarPedido), new { id = pedidoRequest.PedidoId }, new
            {
                pedidoId = pedidoRequest.PedidoId,
                Status = "Criado"
            });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erro ao enviar pedido");

            return StatusCode(500, new ProblemDetails
            {
                Title = "Erro interno",
                Detail = "Ocorreu um erro ao tentar enviar o pedido.",
                Status = 500
            });
        }

    }
}