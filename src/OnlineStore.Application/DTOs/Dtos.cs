using OnlineStore.Domain.Enums;

namespace OnlineStore.Application.DTOs;

public record CustomerDto(
    Guid Id,
    string Name,
    string Email,
    string PhoneNumber);

public record CreateCustomerDto(
    string Name,
    string Email,
    string PhoneNumber);

public record ProductDto(
    Guid Id,
    string Name,
    decimal Price,
    int StockQuantity);

public record OrderItemDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal Subtotal);

public record OrderDto(
    Guid Id,
    Guid CustomerId,
    string CustomerName,
    decimal TotalAmount,
    DateTime CreatedAt,
    OrderStatus Status,
    List<OrderItemDto> OrderItems);

public record CreateOrderDto(
    Guid CustomerId,
    List<OrderItemRequestDto> Items);

public record OrderItemRequestDto(
    Guid ProductId,
    int Quantity);

public record UpdateOrderStatusDto(
    OrderStatus Status);

public record CustomerWithOrdersDto(
    Guid Id,
    string Name,
    string Email,
    string PhoneNumber,
    List<OrderDto> Orders);
