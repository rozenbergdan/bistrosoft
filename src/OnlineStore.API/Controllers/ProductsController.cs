using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.DTOs;
using OnlineStore.Application.Queries;

namespace OnlineStore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IMediator mediator, ILogger<ProductsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los productos disponibles
    /// </summary>
    /// <returns>Lista de productos</returns>
    /// <response code="200">Lista de productos</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<ProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllProducts()
    {
        _logger.LogInformation("GET /api/products - Getting all products");
        
        var query = new GetAllProductsQuery();
        var result = await _mediator.Send(query);
        
        return Ok(result);
    }
}
