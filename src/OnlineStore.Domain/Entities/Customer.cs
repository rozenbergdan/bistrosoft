namespace OnlineStore.Domain.Entities;

/// <summary>
/// Entidad Cliente
/// </summary>
public class Customer
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public ICollection<Order> Orders { get; set; } = new List<Order>();

    public Customer()
    {
        Id = Guid.NewGuid();
    }

    public Customer(string name, string email, string phoneNumber)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        PhoneNumber = phoneNumber;
    }
}
