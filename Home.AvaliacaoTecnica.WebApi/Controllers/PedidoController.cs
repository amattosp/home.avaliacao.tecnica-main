using AutoMapper;
using Home.AvaliacaoTecnica.Application.Pedido.EnviarPedido;
using Home.AvaliacaoTecnica.Application.Pedido.ListarPorStatus;
using Home.AvaliacaoTecnica.Contracts.Contratos;
using Home.AvaliacaoTecnica.Infra.Data.Repositories;
using Home.AvaliacaoTecnica.WebApi.Features.Pedido.EnviarPedido;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Home.AvalicaoTecnica.WebApi.Controllers;

/// <summary>
/// Controller responsável pelo envio e consulta de pedidos.
/// </summary>
[ApiController]
[Route("api/pedidos")]
public class PedidoController : ControllerBase
{
    private readonly PedidoRepository _pedidoRepository;

    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public PedidoController(Serilog.ILogger logger,
                            PedidoRepository pedidoRepository,
                            IMediator mediator,
                            IMapper mapper)
    {
        _pedidoRepository = pedidoRepository;
        _mediator = mediator;
        _mapper = mapper;
    }

    /// <summary>
    /// Lista pedidos por Status
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ListarPedidosResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<IActionResult> ListarPorStatus([FromQuery] string status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return BadRequest("Status é obrigatório.");

        try
        {
            var query = new ListarPorStatusQuery(status);
            var pedidos = await _mediator.Send(query);
            return Ok(pedidos);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
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
        //todo: refatprar o codigo para levar a logica para a camada de application
        //todo: a camada de controler nao poder se comunicar com a camada de dominio, tem que passar pela appication
        var pedido = await _pedidoRepository.ObterPorIdAsync(id);
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
    /// Envia informacoes do pedido para servico de gerenciamento de pedidos
    /// </summary>
    /// <param name="request">O request do pedido</param>
    /// <returns>Pedido enviado</returns>
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<IActionResult> EnviarPedido([FromBody] EnviarPedidoRequest request)
    {
        try
        {
            var validator = new EnviarPedidoValidator();
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var enviarPedidoCommand = _mapper.Map<EnviarPedidoCommand>(request);

            var result = await _mediator.Send(enviarPedidoCommand);

            return CreatedAtAction(nameof(EnviarPedido), new { id = result.PedidoId }, new
            {
                pedidoId = result.PedidoId,
                Status = "Criado"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}