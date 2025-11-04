# Shipment Tracking Backend

ASP.NET Core 8 backend for end-to-end shipment tracking with Clean Architecture, PostgreSQL, JWT authentication, and CQRS.

## Projects

```
src/
  ShipmentTracking.Domain          # Pure domain model
  ShipmentTracking.Application     # CQRS, validators, DTOs
  ShipmentTracking.Infrastructure  # EF Core, Identity, services
  ShipmentTracking.WebApi          # HTTP API host
tests/
  ShipmentTracking.Domain.Tests
  ShipmentTracking.Application.Tests
  ShipmentTracking.Integration.Tests
docker/
  Dockerfile
  docker-compose.yml
  .env.example
```

## Prerequisites

- .NET 8 SDK
- Docker Desktop (for PostgreSQL, MailHog, PgAdmin)

## Getting Started

1. Copy `docker/.env.example` to `docker/.env` and update secrets (`POSTGRES_PASSWORD`, `JWT_SECRET`, etc.).
2. Start the infrastructure stack:
   ```bash
   cd docker
   docker-compose up -d
   ```
3. Run migrations and seed data:
   ```bash
   cd ..
   dotnet ef database update --project src/ShipmentTracking.Infrastructure --startup-project src/ShipmentTracking.WebApi
   ```
4. Launch the API:
   ```bash
   dotnet run --project src/ShipmentTracking.WebApi
   ```

## Development Notes

- Default admin credentials are configured via `AdminUser` section in `appsettings.json`.
- MailHog captures outbound email at http://localhost:8025.
- Swagger UI is available at `/swagger` when running in the Development environment.

## Testing

```
dotnet test           # unit tests
dotnet test tests/ShipmentTracking.Integration.Tests --filter Category!=Docker # when Docker unavailable
```

Integration tests rely on Testcontainers; ensure Docker is running before executing the full suite.
