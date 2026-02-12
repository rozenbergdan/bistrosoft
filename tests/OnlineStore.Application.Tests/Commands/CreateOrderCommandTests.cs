using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OnlineStore.Application.Commands;
using OnlineStore.Application.DTOs;
using OnlineStore.Application.Mappings;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Exceptions;
using OnlineStore.Domain.Interfaces;
using Xunit;

namespace OnlineStore.Application.Tests.Commands;

public class CreateOrderCommandTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<CreateOrderCommandHandler>> _loggerMock;
    private readonly CreateOrderCommandHandler _handler;

    public CreateOrderCommandTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        _mapper = mapperConfig.CreateMapper();
        
        _loggerMock = new Mock<ILogger<CreateOrderCommandHandler>>();
        
        _handler = new CreateOrderCommandHandler(
            _unitOfWorkMock.Object,
            _mapper,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidOrder_ShouldCreateOrderAndReduceStock()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        
        var customer = new Customer("Test Customer", "test@example.com", "123")
        {
            Id = customerId
        };

        var product = new Product("Test Product", 100m, 10)
        {
            Id = productId
        };

        var command = new CreateOrderCommand(
            customerId,
            new List<OrderItemRequestDto>
            {
                new(productId, 2)
            });

        var order = new Order(customerId);
        order.AddItem(new OrderItem(productId, 2, product.Price));

        // Setup mocks
        var customerRepoMock = new Mock<ICustomerRepository>();
        customerRepoMock.Setup(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        var productRepoMock = new Mock<IProductRepository>();
        productRepoMock.Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product> { product });

        var orderRepoMock = new Mock<IOrderRepository>();
        orderRepoMock.Setup(x => x.GetWithItemsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _unitOfWorkMock.Setup(x => x.Customers).Returns(customerRepoMock.Object);
        _unitOfWorkMock.Setup(x => x.Products).Returns(productRepoMock.Object);
        _unitOfWorkMock.Setup(x => x.Orders).Returns(orderRepoMock.Object);
        _unitOfWorkMock.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        product.StockQuantity.Should().Be(8); // 10 - 2

        orderRepoMock.Verify(
            x => x.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(
            x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_InsufficientStock_ShouldThrowException()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        
        var customer = new Customer("Test Customer", "test@example.com", "123")
        {
            Id = customerId
        };

        var product = new Product("Test Product", 100m, 5)
        {
            Id = productId
        };

        var command = new CreateOrderCommand(
            customerId,
            new List<OrderItemRequestDto>
            {
                new(productId, 10) // Requesting more than available
            });

        // Setup mocks
        var customerRepoMock = new Mock<ICustomerRepository>();
        customerRepoMock.Setup(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        var productRepoMock = new Mock<IProductRepository>();
        productRepoMock.Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product> { product });

        _unitOfWorkMock.Setup(x => x.Customers).Returns(customerRepoMock.Object);
        _unitOfWorkMock.Setup(x => x.Products).Returns(productRepoMock.Object);
        _unitOfWorkMock.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act & Assert
        await Assert.ThrowsAsync<InsufficientStockException>(
            () => _handler.Handle(command, CancellationToken.None));

        _unitOfWorkMock.Verify(
            x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_CustomerNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var command = new CreateOrderCommand(
            customerId,
            new List<OrderItemRequestDto>
            {
                new(productId, 2)
            });

        var customerRepoMock = new Mock<ICustomerRepository>();
        customerRepoMock.Setup(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        _unitOfWorkMock.Setup(x => x.Customers).Returns(customerRepoMock.Object);
        _unitOfWorkMock.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(command, CancellationToken.None));
    }
}
