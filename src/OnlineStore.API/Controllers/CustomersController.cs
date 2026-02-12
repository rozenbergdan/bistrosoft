using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.Commands;
using OnlineStore.Application.DTOs;
using OnlineStore.Application.Queries;

namespace OnlineStore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(IMediator mediator, ILogger<CustomersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Crea un nuevo cliente
    /// </summary>
    /// <param name="command">Datos del cliente a crear</param>
    /// <returns>Cliente creado</returns>
    /// <response code="201">Cliente creado exitosamente</response>
    /// <response code="400">Datos inválidos o email duplicado</response>
    [HttpPost]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerCommand command)
    {
        _logger.LogInformation("POST /api/customers - Creating customer");
        
        var result = await _mediator.Send(command);
        
        return CreatedAtAction(
            nameof(GetCustomerById), 
            new { id = result.Id }, 
            result);
    }

    /// <summary>
    /// Obtiene un cliente por ID con sus pedidos
    /// </summary>
    /// <param name="id">ID del cliente</param>
    /// <returns>Cliente con sus pedidos</returns>
    /// <response code="200">Cliente encontrado</response>
    /// <response code="404">Cliente no encontrado</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomerWithOrdersDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCustomerById(Guid id)
    {
        _logger.LogInformation("GET /api/customers/{Id}", id);
        
        var query = new GetCustomerByIdQuery(id);
        var result = await _mediator.Send(query);
        
        return Ok(result);
    }

    /// <summary>
    /// Obtiene todos los pedidos de un cliente
    /// </summary>
    /// <param name="id">ID del cliente</param>
    /// <returns>Lista de pedidos del cliente</returns>
    /// <response code="200">Pedidos encontrados</response>
    /// <response code="404">Cliente no encontrado</response>
    [HttpGet("{id}/orders")]
    [ProducesResponseType(typeof(List<OrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCustomerOrders(Guid id)
    {
        _logger.LogInformation("GET /api/customers/{Id}/orders", id);
        
        var query = new GetCustomerOrdersQuery(id);
        var result = await _mediator.Send(query);
        
        return Ok(result);
    }
}
