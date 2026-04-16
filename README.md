# BookStoreAPI

API REST para gestionar libros y autores. Instrucciones para correrla con Docker.

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
