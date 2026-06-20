# Ticket System API

ASP.NET Core 8 Web API built with Clean Architecture and CQRS.

## Projects

| Project | Responsibility |
|---------|----------------|
| `TicketSystem.Domain` | Entities and enums. No dependencies. |
| `TicketSystem.Application` | CQRS handlers (MediatR), DTOs, validators, interfaces, pipeline behaviours. |
| `TicketSystem.Infrastructure` | EF Core `DbContext` + configurations, repositories / unit of work, JWT, password hashing, file storage, report export. |
| `TicketSystem.Api` | Controllers, auth, CORS, Swagger, exception-handling middleware, composition root. |

## Run

```bash
cd src/TicketSystem.Api
dotnet run
```

Set `ConnectionStrings__DefaultConnection` and `Jwt__Key` first (see [`../docs/setup-guide.md`](../docs/setup-guide.md)). The database is migrated and seeded on startup. Swagger is at `/swagger`.

## Migrations

```bash
dotnet ef migrations add <Name> \
  --project src/TicketSystem.Infrastructure \
  --startup-project src/TicketSystem.Api \
  --output-dir Persistence/Migrations
```
