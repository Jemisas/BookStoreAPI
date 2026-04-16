# BookStoreAPI

API REST para gestionar libros y autores.

## Resumen del proyecto

- **Framework**: ASP.NET Core Web API sobre .NET 8.
- **Persistencia**: Entity Framework Core con SQLite en memoria (`DataSource=:memory:`).
- **Autenticación**: JWT Bearer, contraseñas con BCrypt.
- **Validación**: Data Annotations + FluentValidation.
- **Integraciones externas**: Open Library (REST, vía `IHttpClientFactory`) para portadas y daehosting (SOAP, cliente generado con `dotnet-svcutil`) para validar ISBN.
- **CSV**: CsvHelper para la carga masiva de libros.
- **Pruebas**: xUnit con Moq y FluentAssertions (tests de servicio y de controlador con `WebApplicationFactory`).
- **Documentación**: Swagger/OpenAPI en la raíz.
- **Arquitectura**: Clean Architecture en 3 capas — `Controller → Service → Repository`, comunicadas siempre por interfaces y registradas en DI. Los controllers solo manejan HTTP, los services concentran la lógica de negocio (normalización, orquestación SOAP/REST, emisión de JWT) y los repositories son el único punto de acceso a `AppDbContext`. Un middleware central traduce las excepciones a `ProblemDetails` (RFC 7807).

## Docker

El `Dockerfile` está en `BookStoreAPI/BookStoreAPI/` (junto al `.csproj`). Es multi-stage: `mcr.microsoft.com/dotnet/sdk:8.0` para compilar y `mcr.microsoft.com/dotnet/aspnet:8.0` como runtime.

## 1. Construir la imagen

Desde `BookStoreAPI/BookStoreAPI/`:

```bash
docker build -t bookstoreapi .
```

## 2. Ejecutar el contenedor

El comando es el mismo en Linux (Docker nativo), Mac (Docker Desktop) y Windows 11 (Docker Desktop con WSL2):

```bash
docker run -d -p 8080:8080 --name bookstoreapi bookstoreapi
```

Una vez levantado, la API queda en `http://localhost:8080` (Swagger se abre en la raíz).

> Si el puerto 8080 ya está ocupado en tu máquina, cambia solo el lado del host en el mapeo (el contenedor sigue escuchando en 8080). Por ejemplo, para usar 5080:
>
> ```bash
> docker run -d -p 5080:8080 --name bookstoreapi bookstoreapi
> ```
>
> Y accede a `http://localhost:5080`.

Notas mínimas por plataforma:
- **Linux**: si tu usuario no está en el grupo `docker`, antepone `sudo`.
- **Mac (Apple Silicon)**: la imagen base ya tiene variante `arm64`, no necesitas `--platform`.
- **Windows 11**: ejecuta en PowerShell, CMD o Git Bash con Docker Desktop en modo Linux containers (el modo por defecto).

## Detener y limpiar

```bash
docker stop bookstoreapi && docker rm bookstoreapi
```

## Credenciales de prueba

| Usuario | Contraseña |
|---------|------------|
| `admin` | `admin123` |
