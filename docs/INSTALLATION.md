# 📦 Guía de Instalación Completa

## Requisitos del Sistema

### Software Necesario

1. **.NET 8 SDK**
   - Descargar de: https://dotnet.microsoft.com/download/dotnet/8.0
   - Verificar instalación: `dotnet --version`

2. **Node.js 18+ y npm**
   - Descargar de: https://nodejs.org/
   - Verificar instalación: `node --version` y `npm --version`

3. **IDE Recomendado** (elegir uno)
   - Visual Studio Code: https://code.visualstudio.com/
   - Cursor: https://cursor.sh/
   - Windsurf: https://windsurf.ai/
   - Visual Studio 2022: https://visualstudio.microsoft.com/

### Extensiones Recomendadas para VS Code

- C# Dev Kit
- Vue Language Features (Volar)
- TypeScript Vue Plugin (Volar)
- REST Client (para probar API)

## Instalación Paso a Paso

### 1. Clonar/Descargar el Proyecto

```bash
# Si tienes el zip, extraerlo
# Si está en git:
git clone [URL_DEL_REPOSITORIO]
cd OnlineStore
```

### 2. Configurar el Backend

```bash
# Navegar a la solución
cd OnlineStore

# Restaurar todos los paquetes NuGet
dotnet restore

# Compilar la solución
dotnet build

# Verificar que no hay errores
```

### 3. Ejecutar Tests (Opcional pero recomendado)

```bash
cd tests/OnlineStore.Application.Tests
dotnet test

# Deberías ver todos los tests en verde
```

### 4. Ejecutar la API

```bash
# Desde la raíz del proyecto
cd src/OnlineStore.API

# Ejecutar la API
dotnet run

# O con watch para desarrollo (recarga automática)
dotnet watch run
```

**Salida esperada:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
```

### 5. Verificar Swagger

1. Abrir navegador en: `http://localhost:5000`
2. Deberías ver la interfaz de Swagger UI
3. Probar algunos endpoints:
   - GET `/api/products` - Debería devolver 5 productos de ejemplo

### 6. Configurar el Frontend

```bash
# Abrir una nueva terminal
# Navegar al directorio frontend
cd frontend

# Instalar dependencias
npm install

# Ejecutar en modo desarrollo
npm run dev
```

**Salida esperada:**
```
VITE v5.0.0  ready in 500 ms

➜  Local:   http://localhost:5173/
➜  Network: use --host to expose
```

### 7. Probar la Aplicación Completa

1. Abrir navegador en: `http://localhost:5173`
2. Deberías ver la interfaz de la tienda
3. Crear un cliente
4. Agregar productos al carrito
5. Finalizar compra

## Estructura de Puertos

- **Backend API**: `http://localhost:5000` (HTTP) y `https://localhost:5001` (HTTPS)
- **Frontend Vue**: `http://localhost:5173`
- **Swagger UI**: `http://localhost:5000` (raíz)

## Solución de Problemas Comunes

### Error: "dotnet: command not found"

**Solución**: Instalar .NET 8 SDK desde el sitio oficial.

### Error: Puerto 5000 o 5173 en uso

**Solución**: 
```bash
# Para cambiar el puerto de la API, editar launchSettings.json
# Para cambiar el puerto de Vue, editar vite.config.ts
```

### Error: CORS al llamar API desde Vue

**Solución**: Verificar que la API esté corriendo y que en `Program.cs` esté configurado CORS:
```csharp
app.UseCors("AllowVueApp");
```

### Error: "Cannot find module" en Vue

**Solución**:
```bash
cd frontend
rm -rf node_modules
npm install
```

### Error: Falla al crear base de datos

**Solución**: La aplicación usa In-Memory Database por defecto, no requiere configuración adicional.

## Comandos Útiles

### Backend

```bash
# Restaurar dependencias
dotnet restore

# Compilar
dotnet build

# Ejecutar
dotnet run

# Ejecutar con watch
dotnet watch run

# Ejecutar tests
dotnet test

# Limpiar build
dotnet clean
```

### Frontend

```bash
# Instalar dependencias
npm install

# Modo desarrollo
npm run dev

# Build para producción
npm run build

# Preview de producción
npm run preview
```

## Configuración Avanzada

### Usar SQL Server en lugar de In-Memory

1. Instalar SQL Server o SQL Server Express
2. Actualizar `Program.cs`:
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
```

3. Agregar connection string en `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=OnlineStoreDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

4. Crear migración:
```bash
dotnet ef migrations add InitialCreate -p src/OnlineStore.Infrastructure -s src/OnlineStore.API
dotnet ef database update -s src/OnlineStore.API
```

### Configurar Logging

El logging está configurado en `appsettings.json`:
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning"
      }
    }
  }
}
```

Los logs se guardan en: `src/OnlineStore.API/logs/`

## Datos de Prueba

La aplicación viene con datos de ejemplo:

**Productos** (5 productos disponibles):
- Laptop HP - $899.99
- Mouse Logitech - $25.99
- Teclado Mecánico - $79.99
- Monitor 24" - $199.99
- Auriculares Bluetooth - $49.99

**Cliente de ejemplo**:
- Nombre: Juan Pérez
- Email: juan.perez@example.com
- ID: aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa

## Verificación de la Instalación

### Checklist

- [ ] .NET 8 SDK instalado (`dotnet --version`)
- [ ] Node.js instalado (`node --version`)
- [ ] Backend compila sin errores (`dotnet build`)
- [ ] Tests pasan (`dotnet test`)
- [ ] API corriendo en puerto 5000
- [ ] Swagger accesible en navegador
- [ ] Frontend instalado (`npm install`)
- [ ] Vue corriendo en puerto 5173
- [ ] Se puede crear cliente desde UI
- [ ] Se pueden agregar productos al carrito
- [ ] Se puede crear pedido

## Próximos Pasos

1. Explorar la documentación Swagger
2. Revisar el código fuente
3. Ejecutar y revisar los tests unitarios
4. Probar todos los flujos de la aplicación
5. Revisar logs generados

## Recursos Adicionales

- [Documentación .NET](https://docs.microsoft.com/dotnet/)
- [Documentación Vue.js](https://vuejs.org/)
- [Documentación MediatR](https://github.com/jbogard/MediatR)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

---

Si tienes problemas con la instalación, verifica que todos los requisitos previos estén correctamente instalados y que los puertos necesarios estén disponibles.
