namespace OnlineStore.Domain.Enums;

/// <summary>
/// Estados posibles de un pedido
/// </summary>
public enum OrderStatus
{
    Pending = 0,
    Paid = 1,
    Shipped = 2,
    Delivered = 3,
    Cancelled = 4
}
