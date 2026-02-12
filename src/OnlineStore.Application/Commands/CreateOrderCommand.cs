using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OnlineStore.Application.DTOs;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Exceptions;
using OnlineStore.Domain.Interfaces;

namespace OnlineStore.Application.Commands;

public record CreateOrderCommand(Guid CustomerId, List<OrderItemRequestDto> Items) 
    : IRequest<OrderDto>;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateOrderCommandHandler> _logger;

    public CreateOrderCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CreateOrderCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating order for customer: {CustomerId}", request.CustomerId);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Verificar que el cliente existe
            var customer = await _unitOfWork.Customers.GetByIdAsync(request.CustomerId, cancellationToken);
            if (customer == null)
            {
                throw new NotFoundException(nameof(Customer), request.CustomerId);
            }

            // Obtener todos los productos
            var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();
            var products = (await _unitOfWork.Products.GetByIdsAsync(productIds, cancellationToken)).ToList();

            // Verificar que todos los productos existen
            var missingProductIds = productIds.Except(products.Select(p => p.Id)).ToList();
            if (missingProductIds.Any())
            {
                throw new NotFoundException("Producto(s)", string.Join(", ", missingProductIds));
            }

            // Crear el pedido
            var order = new Order(request.CustomerId);

            // Agregar items y validar stock
            foreach (var itemRequest in request.Items)
            {
                var product = products.First(p => p.Id == itemRequest.ProductId);

                // Validar stock
                if (!product.HasStock(itemRequest.Quantity))
                {
                    throw new InsufficientStockException(
                        product.Name, 
                        product.StockQuantity, 
                        itemRequest.Quantity);
                }

                // Reducir stock
                product.ReduceStock(itemRequest.Quantity);

                // Crear item del pedido
                var orderItem = new OrderItem(product.Id, itemRequest.Quantity, product.Price);
                order.AddItem(orderItem);
            }

            // Guardar cambios
            await _unitOfWork.Orders.AddAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation(
                "Order created successfully with ID: {OrderId}, Total: {TotalAmount}", 
                order.Id, 
                order.TotalAmount);

            // Obtener el pedido completo con relaciones
            var orderWithItems = await _unitOfWork.Orders.GetWithItemsAsync(order.Id, cancellationToken);
            
            return _mapper.Map<OrderDto>(orderWithItems);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
