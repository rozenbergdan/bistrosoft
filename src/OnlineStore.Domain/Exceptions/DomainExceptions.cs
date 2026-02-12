namespace OnlineStore.Domain.Exceptions;

/// <summary>
/// Excepción base para el dominio
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }

    public DomainException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}

/// <summary>
/// Excepción cuando no se encuentra una entidad
/// </summary>
public class NotFoundException : DomainException
{
    public NotFoundException(string entityName, object key)
        : base($"{entityName} con ID '{key}' no fue encontrado")
    {
    }
}

/// <summary>
/// Excepción de validación
/// </summary>
public class ValidationException : DomainException
{
    public ValidationException(string message) : base(message)
    {
    }
}

/// <summary>
/// Excepción de stock insuficiente
/// </summary>
public class InsufficientStockException : DomainException
{
    public InsufficientStockException(string productName, int available, int requested)
        : base($"Stock insuficiente para '{productName}'. Disponible: {available}, Solicitado: {requested}")
    {
    }
}
