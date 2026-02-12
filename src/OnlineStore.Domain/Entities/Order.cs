using OnlineStore.Domain.Enums;

namespace OnlineStore.Domain.Entities;

/// <summary>
/// Entidad Pedido
/// </summary>
public class Order
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public OrderStatus Status { get; set; }

    // Navigation properties
    public Customer Customer { get; set; } = null!;
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public Order()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        Status = OrderStatus.Pending;
    }

    public Order(Guid customerId)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        CreatedAt = DateTime.UtcNow;
        Status = OrderStatus.Pending;
        TotalAmount = 0;
    }

    /// <summary>
    /// Añade un item al pedido
    /// </summary>
    public void AddItem(OrderItem item)
    {
        item.OrderId = Id;
        OrderItems.Add(item);
        CalculateTotalAmount();
    }

    /// <summary>
    /// Calcula el monto total del pedido
    /// </summary>
    public void CalculateTotalAmount()
    {
        TotalAmount = OrderItems.Sum(item => item.GetSubtotal());
    }

    /// <summary>
    /// Actualiza el estado del pedido con validación
    /// </summary>
    public void UpdateStatus(OrderStatus newStatus)
    {
        // Validar transiciones de estado permitidas
        if (!IsValidStatusTransition(Status, newStatus))
        {
            throw new InvalidOperationException(
                $"No se puede cambiar el estado de {Status} a {newStatus}");
        }

        Status = newStatus;
    }

    /// <summary>
    /// Valida si la transición de estado es permitida
    /// </summary>
    private bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
    {
        return currentStatus switch
        {
            OrderStatus.Pending => newStatus is OrderStatus.Paid or OrderStatus.Cancelled,
            OrderStatus.Paid => newStatus is OrderStatus.Shipped or OrderStatus.Cancelled,
            OrderStatus.Shipped => newStatus is OrderStatus.Delivered,
            OrderStatus.Delivered => false,
            OrderStatus.Cancelled => false,
            _ => false
        };
    }

    /// <summary>
    /// Cancela el pedido
    /// </summary>
    public void Cancel()
    {
        if (Status == OrderStatus.Delivered)
        {
            throw new InvalidOperationException("No se puede cancelar un pedido ya entregado");
        }

        Status = OrderStatus.Cancelled;
    }
}
