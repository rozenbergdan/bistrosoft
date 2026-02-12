namespace OnlineStore.Domain.Entities;

/// <summary>
/// Entidad Producto
/// </summary>
public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }

    public Product()
    {
        Id = Guid.NewGuid();
    }

    public Product(string name, decimal price, int stockQuantity)
    {
        Id = Guid.NewGuid();
        Name = name;
        Price = price;
        StockQuantity = stockQuantity;
    }

    /// <summary>
    /// Reduce el stock del producto
    /// </summary>
    public void ReduceStock(int quantity)
    {
        if (quantity > StockQuantity)
        {
            throw new InvalidOperationException($"Stock insuficiente para el producto {Name}. Disponible: {StockQuantity}, Solicitado: {quantity}");
        }
        
        StockQuantity -= quantity;
    }

    /// <summary>
    /// Aumenta el stock del producto
    /// </summary>
    public void IncreaseStock(int quantity)
    {
        StockQuantity += quantity;
    }

    /// <summary>
    /// Verifica si hay stock disponible
    /// </summary>
    public bool HasStock(int quantity)
    {
        return StockQuantity >= quantity;
    }
}
