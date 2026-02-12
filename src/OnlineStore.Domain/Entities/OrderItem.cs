namespace OnlineStore.Domain.Entities;

/// <summary>
/// Entidad Item del Pedido
/// </summary>
public class OrderItem
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    // Navigation properties
    public Order Order { get; set; } = null!;
    public Product Product { get; set; } = null!;

    public OrderItem()
    {
        Id = Guid.NewGuid();
    }

    public OrderItem(Guid productId, int quantity, decimal unitPrice)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    /// <summary>
    /// Calcula el subtotal del item
    /// </summary>
    public decimal GetSubtotal()
    {
        return Quantity * UnitPrice;
    }
}
