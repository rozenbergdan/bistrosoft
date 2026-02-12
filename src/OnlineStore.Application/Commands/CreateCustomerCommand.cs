using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OnlineStore.Application.DTOs;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Exceptions;
using OnlineStore.Domain.Interfaces;

namespace OnlineStore.Application.Commands;

public record CreateCustomerCommand(string Name, string Email, string PhoneNumber) 
    : IRequest<CustomerDto>;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, CustomerDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateCustomerCommandHandler> _logger;

    public CreateCustomerCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CreateCustomerCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CustomerDto> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating customer with email: {Email}", request.Email);

        // Verificar si el email ya existe
        var existingCustomer = await _unitOfWork.Customers.GetByEmailAsync(request.Email, cancellationToken);
        if (existingCustomer != null)
        {
            throw new ValidationException($"Ya existe un cliente con el email {request.Email}");
        }

        var customer = new Customer(request.Name, request.Email, request.PhoneNumber);

        await _unitOfWork.Customers.AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Customer created successfully with ID: {CustomerId}", customer.Id);

        return _mapper.Map<CustomerDto>(customer);
    }
}
