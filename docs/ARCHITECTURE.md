# 🏛️ Documentación de Arquitectura

## Visión General

Este proyecto implementa una **API REST** para gestión de pedidos siguiendo los principios de **Clean Architecture** y patrones modernos de desarrollo.

## Capas de la Arquitectura

### 1. Domain Layer (Núcleo)

**Responsabilidad**: Contiene las entidades de negocio, interfaces y reglas de dominio.

**Componentes**:
- `Entities/`: Modelos de dominio (Customer, Order, Product, OrderItem)
- `Enums/`: Enumeraciones (OrderStatus)
- `Interfaces/`: Contratos de repositorios y servicios
- `Exceptions/`: Excepciones de dominio personalizadas

**Características**:
- Sin dependencias externas
- Contiene la lógica de negocio crítica
- Entidades con comportamiento (no anémicas)

**Ejemplo**:
```csharp
public class Order
{
    public void UpdateStatus(OrderStatus newStatus)
    {
        if (!IsValidStatusTransition(Status, newStatus))
        {
            throw new InvalidOperationException(
                $"No se puede cambiar el estado de {Status} a {newStatus}");
        }
        Status = newStatus;
    }
}
```

### 2. Application Layer

**Responsabilidad**: Orquesta el flujo de datos entre capas, implementa casos de uso.

**Componentes**:
- `Commands/`: Operaciones de escritura (CQRS)
- `Queries/`: Operaciones de lectura (CQRS)
- `DTOs/`: Data Transfer Objects
- `Mappings/`: Perfiles de AutoMapper
- `Validators/`: Validaciones con FluentValidation
- `Behaviors/`: Pipelines de MediatR

**Patrones Implementados**:

#### CQRS (Command Query Responsibility Segregation)
```csharp
// Command - Escritura
public record CreateOrderCommand(Guid CustomerId, List<OrderItemRequestDto> Items) 
    : IRequest<OrderDto>;

// Query - Lectura
public record GetCustomerByIdQuery(Guid CustomerId) 
    : IRequest<CustomerWithOrdersDto>;
```

#### MediatR Pipelines
```csharp
// Validación automática
public class ValidationBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse>

// Logging automático
public class LoggingBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse>
```

### 3. Infrastructure Layer

**Responsabilidad**: Implementa acceso a datos, servicios externos, logging.

**Componentes**:
- `Persistence/`: DbContext de Entity Framework
- `Repositories/`: Implementaciones concretas
- `Logging/`: Configuración de Serilog

**Patrones Implementados**:

#### Repository Pattern
```csharp
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    // ... más métodos
}
```

#### Unit of Work
```csharp
public interface IUnitOfWork : IDisposable
{
    ICustomerRepository Customers { get; }
    IOrderRepository Orders { get; }
    IProductRepository Products { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

### 4. API Layer (Presentación)

**Responsabilidad**: Expone endpoints HTTP, maneja requests/responses.

**Componentes**:
- `Controllers/`: Endpoints REST
- `Middleware/`: Manejo global de excepciones
- `Extensions/`: Métodos de extensión
- `Program.cs`: Configuración y DI

**Características**:
- Documentación Swagger/OpenAPI
- Manejo centralizado de errores
- CORS configurado
- Logging de requests

## Flujo de Datos

```
Request HTTP
    ↓
Controller (API Layer)
    ↓
Command/Query (MediatR)
    ↓
Validation Behavior (FluentValidation)
    ↓
Logging Behavior
    ↓
Handler (Application Layer)
    ↓
Repository (Infrastructure Layer)
    ↓
DbContext (EF Core)
    ↓
Database (In-Memory/SQL Server)
```

## Inyección de Dependencias

### Configuración en Program.cs

```csharp
// Repositorios
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// MediatR con behaviors
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.Load("OnlineStore.Application"));
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

// FluentValidation
builder.Services.AddValidatorsFromAssembly(Assembly.Load("OnlineStore.Application"));
```

## Manejo de Errores

### Middleware Global

```csharp
public class ExceptionHandlingMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }
}
```

### Tipos de Excepciones

- `NotFoundException`: Entidad no encontrada (404)
- `ValidationException`: Validación fallida (400)
- `InsufficientStockException`: Stock insuficiente (400)
- `DomainException`: Error de negocio (400)
- `Exception`: Error interno (500)

## Validaciones

### FluentValidation

```csharp
public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("El ID del cliente es requerido");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("El pedido debe tener al menos un producto");
    }
}
```

### Validaciones de Dominio

```csharp
public void ReduceStock(int quantity)
{
    if (quantity > StockQuantity)
    {
        throw new InvalidOperationException(
            $"Stock insuficiente para el producto {Name}");
    }
    StockQuantity -= quantity;
}
```

## Logging

### Configuración Serilog

```csharp
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/onlinestore-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
```

### Uso en Handlers

```csharp
public async Task<OrderDto> Handle(CreateOrderCommand request, ...)
{
    _logger.LogInformation("Creating order for customer: {CustomerId}", 
        request.CustomerId);
    
    // ... lógica
    
    _logger.LogInformation("Order created successfully with ID: {OrderId}", 
        order.Id);
}
```

## Transacciones

### Uso de Unit of Work

```csharp
await _unitOfWork.BeginTransactionAsync(cancellationToken);

try
{
    // Múltiples operaciones
    await _unitOfWork.Orders.AddAsync(order, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);
    await _unitOfWork.CommitTransactionAsync(cancellationToken);
}
catch
{
    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
    throw;
}
```

## Testing

### Estrategia

- **Unit Tests**: Lógica de handlers con mocks
- **Integration Tests**: Posible extensión futura
- **Mocking**: Moq para repositorios y dependencias

### Ejemplo de Test

```csharp
[Fact]
public async Task Handle_ValidCommand_ShouldCreateCustomer()
{
    // Arrange
    var command = new CreateCustomerCommand("Test", "test@email.com", "123");
    
    // Act
    var result = await _handler.Handle(command, CancellationToken.None);
    
    // Assert
    result.Should().NotBeNull();
    result.Email.Should().Be(command.Email);
}
```

## Principios SOLID

### Single Responsibility
Cada clase tiene una única responsabilidad:
- Controllers: Manejar HTTP
- Handlers: Ejecutar casos de uso
- Repositories: Acceso a datos

### Open/Closed
Extensible mediante behaviors de MediatR sin modificar código existente.

### Liskov Substitution
Interfaces permiten intercambiar implementaciones.

### Interface Segregation
Interfaces específicas por dominio (ICustomerRepository, IOrderRepository).

### Dependency Inversion
Dependencias siempre hacia abstracciones (interfaces).

## Escalabilidad

### Horizontal
- API stateless permite múltiples instancias
- Database puede escalar independientemente

### Vertical
- Arquitectura en capas permite optimizaciones por capa
- Caching puede agregarse sin cambiar lógica

## Seguridad

### Implementado
- Validación de entrada
- Manejo seguro de excepciones
- CORS configurado

### Por Implementar (Extras)
- JWT Authentication
- Rate limiting
- HTTPS enforcement
- Input sanitization adicional

## Performance

### Optimizaciones
- Async/await en toda la aplicación
- Lazy loading deshabilitado
- Queries optimizadas con Include
- In-Memory database para desarrollo

### Métricas
- Logging de tiempo de ejecución en behaviors
- Posibilidad de agregar Application Insights

## Mantenibilidad

### Ventajas
- Código limpio y organizado
- Alta cohesión, bajo acoplamiento
- Tests unitarios
- Documentación XML en código
- Swagger para API

### Convenciones
- Nombres descriptivos
- Estructura de folders clara
- Separación de concerns
- DRY (Don't Repeat Yourself)

---

Esta arquitectura permite:
- ✅ Fácil testing
- ✅ Mantenimiento simplificado
- ✅ Escalabilidad
- ✅ Extensibilidad
- ✅ Independencia de frameworks
- ✅ Reglas de negocio centralizadas
