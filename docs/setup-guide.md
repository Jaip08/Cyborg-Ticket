# Setup Guide

Two ways to run the project locally: with Docker (one command) or manually.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/) and npm
- A PostgreSQL database — a local install or a free [Supabase](https://supabase.com) project
- (Optional) Docker Desktop

---

## Option A — Docker (recommended for a quick look)

From the repository root:

```bash
docker compose up --build
```

That starts PostgreSQL, the API, and the web app. The API applies the schema and seeds demo data on first boot.

- Web: http://localhost:3000
- Swagger: http://localhost:5080/swagger

Stop with `Ctrl+C`; add `-v` to `docker compose down` to wipe the database volume.

---

## Option B — Manual

### 1. Database

**Local PostgreSQL** — create an empty database:

```sql
CREATE DATABASE ticketsystem;
```

**Supabase** — create a project, then grab the connection string from *Project Settings → Database*. It looks like:

```
Host=db.<ref>.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=<your-password>;SSL Mode=Require;Trust Server Certificate=true
```

You don't need to create tables by hand — the API does it on startup. If you'd rather set the schema up yourself (e.g. in the Supabase SQL editor), run [`database/schema.sql`](../database/schema.sql) then [`database/seed.sql`](../database/seed.sql).

### 2. Backend

Point the API at your database. Either edit `backend/src/TicketSystem.Api/appsettings.json` or set an environment variable:

```bash
# from backend/src/TicketSystem.Api
export ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=ticketsystem;Username=postgres;Password=postgres"
export Jwt__Key="any-long-random-string-at-least-32-characters"

dotnet run
```

On Windows PowerShell:

```powershell
$env:ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=ticketsystem;Username=postgres;Password=postgres"
$env:Jwt__Key="any-long-random-string-at-least-32-characters"
dotnet run
```

The API starts on **http://localhost:5080** with Swagger at `/swagger`. On first run it applies migrations and seeds roles, categories, demo users and sample tickets.

### 3. Frontend

```bash
cd frontend
cp .env.example .env.local      # NEXT_PUBLIC_API_URL=http://localhost:5080/api
npm install
npm run dev
```

Open http://localhost:3000 and sign in with one of the [demo accounts](../README.md#demo-accounts).

---

## Working with EF Core migrations

Migrations live in `backend/src/TicketSystem.Infrastructure/Persistence/Migrations`. To add one after changing an entity:

```bash
dotnet tool install --global dotnet-ef          # once
cd backend
dotnet ef migrations add <Name> \
  --project src/TicketSystem.Infrastructure \
  --startup-project src/TicketSystem.Api \
  --output-dir Persistence/Migrations
```

Regenerate the SQL schema file with:

```bash
dotnet ef migrations script --idempotent \
  --project src/TicketSystem.Infrastructure \
  --startup-project src/TicketSystem.Api \
  -o ../database/schema.sql
```

---

## Troubleshooting

- **CORS errors in the browser** — make sure `Cors__AllowedOrigins__0` matches the frontend URL exactly (scheme + host + port).
- **401 right after logging in** — check `Jwt__Key` is set and identical to the value the API started with.
- **Port already in use** — change the API port with `ASPNETCORE_URLS=http://localhost:5090`, and update `NEXT_PUBLIC_API_URL` to match.
- **Can't connect to Supabase** — ensure `SSL Mode=Require;Trust Server Certificate=true` is on the connection string.
