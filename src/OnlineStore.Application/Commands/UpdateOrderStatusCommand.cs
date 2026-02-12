using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OnlineStore.Application.DTOs;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Enums;
using OnlineStore.Domain.Exceptions;
using OnlineStore.Domain.Interfaces;

namespace OnlineStore.Application.Commands;

public record UpdateOrderStatusCommand(Guid OrderId, OrderStatus Status) 
    : IRequest<OrderDto>;

public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, OrderDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateOrderStatusCommandHandler> _logger;

    public UpdateOrderStatusCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<UpdateOrderStatusCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<OrderDto> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Updating order {OrderId} status to {Status}", 
            request.OrderId, 
            request.Status);

        var order = await _unitOfWork.Orders.GetWithItemsAsync(request.OrderId, cancellationToken);
        if (order == null)
        {
            throw new NotFoundException(nameof(Order), request.OrderId);
        }

        var previousStatus = order.Status;
        
        try
        {
            order.UpdateStatus(request.Status);
            
            await _unitOfWork.Orders.UpdateAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Order {OrderId} status updated from {PreviousStatus} to {NewStatus}",
                order.Id,
                previousStatus,
                order.Status);

            return _mapper.Map<OrderDto>(order);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(
                ex,
                "Invalid status transition for order {OrderId} from {PreviousStatus} to {NewStatus}",
                order.Id,
                previousStatus,
                request.Status);
            throw;
        }
    }
}
