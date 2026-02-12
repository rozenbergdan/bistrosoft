using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OnlineStore.Application.DTOs;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Exceptions;
using OnlineStore.Domain.Interfaces;

namespace OnlineStore.Application.Queries;

public record GetCustomerOrdersQuery(Guid CustomerId) : IRequest<List<OrderDto>>;

public class GetCustomerOrdersQueryHandler : IRequestHandler<GetCustomerOrdersQuery, List<OrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetCustomerOrdersQueryHandler> _logger;

    public GetCustomerOrdersQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetCustomerOrdersQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<OrderDto>> Handle(GetCustomerOrdersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting orders for customer: {CustomerId}", request.CustomerId);

        // Verificar que el cliente existe
        var customerExists = await _unitOfWork.Customers.ExistsAsync(request.CustomerId, cancellationToken);
        if (!customerExists)
        {
            throw new NotFoundException(nameof(Customer), request.CustomerId);
        }

        var orders = await _unitOfWork.Orders.GetByCustomerIdAsync(request.CustomerId, cancellationToken);

        return _mapper.Map<List<OrderDto>>(orders);
    }
}
