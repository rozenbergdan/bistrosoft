# 🛒 Online Store - Sistema de Gestión de Pedidos

Sistema completo de gestión de pedidos para tienda online desarrollado con **.NET 8** y **Vue.js 3**.

## 📋 Características

- ✅ API REST con .NET 8
- ✅ Clean Architecture (Arquitectura Limpia)
- ✅ CQRS con MediatR
- ✅ Repository Pattern
- ✅ Unit of Work
- ✅ Entity Framework Core (In-Memory Database)
- ✅ AutoMapper para DTOs
- ✅ FluentValidation
- ✅ Logging con Serilog
- ✅ Manejo global de excepciones
- ✅ Documentación Swagger/OpenAPI
- ✅ Tests unitarios con xUnit y Moq
- ✅ Frontend Vue.js 3 con TypeScript
- ✅ Diseño responsive

## 🏗️ Arquitectura

El proyecto sigue los principios de **Clean Architecture**:

```
OnlineStore/
├── src/
│   ├── OnlineStore.Domain/          # Entidades, interfaces, excepciones
│   ├── OnlineStore.Application/     # Lógica de negocio, CQRS, DTOs
│   ├── OnlineStore.Infrastructure/  # EF Core, repositorios, logging
│   └── OnlineStore.API/             # Controllers, middleware, configuración
├── tests/
│   └── OnlineStore.Application.Tests/
├── frontend/                        # Aplicación Vue.js 3
└── docs/                           # Documentación adicional
```

## 🚀 Inicio Rápido

### Requisitos Previos

- .NET 8 SDK
- Node.js 18+ y npm
- Visual Studio Code, Cursor, o Windsurf (recomendado)

### Backend (.NET API)

1. Navegar al directorio del proyecto:
```bash
cd OnlineStore
```

2. Restaurar dependencias:
```bash
dotnet restore
```

3. Ejecutar la API:
```bash
cd src/OnlineStore.API
dotnet run
```

La API estará disponible en:
- Swagger UI: `https://localhost:5001` o `http://localhost:5000`
- API: `http://localhost:5000/api`

### Frontend (Vue.js)

1. Navegar al directorio frontend:
```bash
cd frontend
```

2. Instalar dependencias:
```bash
npm install
```

3. Ejecutar en modo desarrollo:
```bash
npm run dev
```

La aplicación estará disponible en: `http://localhost:5173`

## 🧪 Ejecutar Tests

```bash
cd tests/OnlineStore.Application.Tests
dotnet test
```

## 📚 Endpoints de la API

### Customers

- **POST** `/api/customers` - Crear cliente
- **GET** `/api/customers/{id}` - Obtener cliente por ID
- **GET** `/api/customers/{id}/orders` - Obtener pedidos de un cliente

### Orders

- **POST** `/api/orders` - Crear pedido
- **PUT** `/api/orders/{id}/status` - Actualizar estado del pedido

### Products

- **GET** `/api/products` - Listar todos los productos

## 🎯 Funcionalidades Principales

### Backend

1. **Gestión de Clientes**
   - Crear clientes con validación de email único
   - Consultar información del cliente
   - Ver historial de pedidos

2. **Gestión de Pedidos**
   - Crear pedidos con validación de stock
   - Actualizar estados (Pending → Paid → Shipped → Delivered)
   - Cálculo automático de totales
   - Transacciones para consistencia de datos

3. **Gestión de Productos**
   - Consultar catálogo de productos
   - Control de inventario automático

### Frontend

1. **Interfaz de Usuario**
   - Formulario para crear clientes
   - Catálogo de productos con stock disponible
   - Carrito de compras interactivo
   - Proceso de checkout
   - Confirmación de pedidos

## 🛠️ Tecnologías Utilizadas

### Backend
- .NET 8
- Entity Framework Core 8
- MediatR 12
- AutoMapper 13
- FluentValidation 11
- Serilog 8
- xUnit 2.6
- Moq 4.20

### Frontend
- Vue.js 3.4
- TypeScript 5.3
- Vite 5.0
- Axios 1.6

## 📖 Patrones y Principios

- **Clean Architecture**: Separación clara de responsabilidades
- **CQRS**: Separación de comandos y consultas
- **Repository Pattern**: Abstracción de acceso a datos
- **Unit of Work**: Gestión de transacciones
- **Dependency Injection**: Inyección de dependencias nativa de .NET
- **SOLID Principles**: Código mantenible y escalable

## 🔍 Validaciones

- Email único por cliente
- Validación de stock antes de crear pedidos
- Validación de transiciones de estado
- FluentValidation para reglas de negocio

## 📝 Logging

Todos los requests son logueados con:
- Nombre del request
- Tiempo de ejecución
- Errores con stack trace
- Logs guardados en archivos diarios

## ⚙️ Configuración

La aplicación usa **base de datos en memoria** por defecto. Para usar SQL Server:

1. Actualizar `Program.cs`:
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

2. Agregar connection string en `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=OnlineStoreDb;Trusted_Connection=True;"
  }
}
```

## 🎨 Capturas

La aplicación incluye:
- Dashboard principal con productos
- Formulario de cliente
- Carrito de compras
- Confirmación de pedidos

## 👨‍💻 Desarrollo

### Estructura de Código

- **Controllers**: Endpoints HTTP con documentación XML
- **Commands**: Operaciones de escritura (Create, Update)
- **Queries**: Operaciones de lectura (Get)
- **Handlers**: Lógica de negocio para cada comando/query
- **Validators**: Reglas de validación con FluentValidation
- **Behaviors**: Pipelines de MediatR (logging, validación)

### Manejo de Errores

- Middleware global para capturar excepciones
- Respuestas HTTP apropiadas (400, 404, 500)
- Mensajes de error descriptivos
- Logging de todos los errores

## 🤝 Contribuciones

Este proyecto fue desarrollado como parte del proceso de selección para Bistrosoft.

## 📄 Licencia

Proyecto de evaluación técnica - Bistrosoft

## 📧 Contacto

Para consultas sobre este proyecto, contactar al evaluador de Bistrosoft.

---

**Desarrollado con ❤️ usando .NET 8 y Vue.js 3**
