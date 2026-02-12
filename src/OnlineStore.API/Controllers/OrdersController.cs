using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.Commands;
using OnlineStore.Application.DTOs;

namespace OnlineStore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IMediator mediator, ILogger<OrdersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Crea un nuevo pedido
    /// </summary>
    /// <param name="command">Datos del pedido a crear</param>
    /// <returns>Pedido creado</returns>
    /// <response code="201">Pedido creado exitosamente</response>
    /// <response code="400">Datos inválidos o stock insuficiente</response>
    /// <response code="404">Cliente o producto no encontrado</response>
    [HttpPost]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
    {
        _logger.LogInformation("POST /api/orders - Creating order for customer {CustomerId}", command.CustomerId);
        
        var result = await _mediator.Send(command);
        
        return CreatedAtAction(
            nameof(CustomersController.GetCustomerOrders),
            "Customers",
            new { id = result.CustomerId },
            result);
    }

    /// <summary>
    /// Actualiza el estado de un pedido
    /// </summary>
    /// <param name="id">ID del pedido</param>
    /// <param name="dto">Nuevo estado</param>
    /// <returns>Pedido actualizado</returns>
    /// <response code="200">Estado actualizado exitosamente</response>
    /// <response code="400">Transición de estado inválida</response>
    /// <response code="404">Pedido no encontrado</response>
    [HttpPut("{id}/status")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] UpdateOrderStatusDto dto)
    {
        _logger.LogInformation("PUT /api/orders/{Id}/status - Updating to {Status}", id, dto.Status);
        
        var command = new UpdateOrderStatusCommand(id, dto.Status);
        var result = await _mediator.Send(command);
        
        return Ok(result);
    }
}
