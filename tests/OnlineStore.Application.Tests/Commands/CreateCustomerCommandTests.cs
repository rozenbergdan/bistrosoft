using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OnlineStore.Application.Commands;
using OnlineStore.Application.Mappings;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Exceptions;
using OnlineStore.Domain.Interfaces;
using Xunit;

namespace OnlineStore.Application.Tests.Commands;

public class CreateCustomerCommandTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<CreateCustomerCommandHandler>> _loggerMock;
    private readonly CreateCustomerCommandHandler _handler;

    public CreateCustomerCommandTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        _mapper = mapperConfig.CreateMapper();
        
        _loggerMock = new Mock<ILogger<CreateCustomerCommandHandler>>();
        
        _handler = new CreateCustomerCommandHandler(
            _unitOfWorkMock.Object,
            _mapper,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateCustomer()
    {
        // Arrange
        var command = new CreateCustomerCommand(
            "Test Customer",
            "test@example.com",
            "123456789");

        var customerRepositoryMock = new Mock<ICustomerRepository>();
        customerRepositoryMock
            .Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        _unitOfWorkMock.Setup(x => x.Customers).Returns(customerRepositoryMock.Object);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(command.Name);
        result.Email.Should().Be(command.Email);
        result.PhoneNumber.Should().Be(command.PhoneNumber);

        customerRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ShouldThrowValidationException()
    {
        // Arrange
        var command = new CreateCustomerCommand(
            "Test Customer",
            "existing@example.com",
            "123456789");

        var existingCustomer = new Customer("Existing", "existing@example.com", "");

        var customerRepositoryMock = new Mock<ICustomerRepository>();
        customerRepositoryMock
            .Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCustomer);

        _unitOfWorkMock.Setup(x => x.Customers).Returns(customerRepositoryMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(
            () => _handler.Handle(command, CancellationToken.None));

        customerRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
