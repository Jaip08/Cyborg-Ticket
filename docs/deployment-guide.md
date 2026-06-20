# Deployment Guide

A free-tier friendly setup:

- **Supabase** — managed PostgreSQL
- **Render** — the ASP.NET Core API (Docker)
- **Vercel** — the Next.js frontend

Deploy in that order so each piece has the URL it depends on.

---

## 1. Database — Supabase

1. Create a project at [supabase.com](https://supabase.com) and choose a database password.
2. *Project Settings → Database → Connection string* gives you the host and credentials. The API will use:
   ```
   Host=db.<ref>.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=<password>;SSL Mode=Require;Trust Server Certificate=true
   ```
3. (Optional) Pre-create the schema: open the **SQL Editor**, run [`database/schema.sql`](../database/schema.sql), then [`database/seed.sql`](../database/seed.sql). Otherwise the API creates and seeds everything on first start.

---

## 2. API — Render

1. Push this repo to GitHub.
2. On [Render](https://render.com): **New → Web Service**, connect the repo.
3. Settings:
   - **Root Directory**: `backend`
   - **Runtime**: Docker (Render picks up `backend/Dockerfile`)
4. Environment variables:

   | Key | Value |
   |-----|-------|
   | `ConnectionStrings__DefaultConnection` | your Supabase connection string |
   | `Jwt__Key` | a long random secret (32+ chars) |
   | `Jwt__Issuer` | `TicketSystem` |
   | `Jwt__Audience` | `TicketSystemClient` |
   | `Cors__AllowedOrigins__0` | your Vercel URL (fill in after step 3) |
   | `ASPNETCORE_ENVIRONMENT` | `Production` |

   The app reads Render's `PORT` automatically, so no port config is needed.
5. Deploy. When it's live, note the URL, e.g. `https://ticket-api.onrender.com`. Check `…/swagger`.

> Free Render services sleep when idle, so the first request after a pause is slow. That's expected.

---

## 3. Frontend — Vercel

1. On [Vercel](https://vercel.com): **Add New → Project**, import the repo.
2. Settings:
   - **Root Directory**: `frontend`
   - **Framework Preset**: Next.js (auto-detected)
3. Environment variable:

   | Key | Value |
   |-----|-------|
   | `NEXT_PUBLIC_API_URL` | `https://ticket-api.onrender.com/api` |

4. Deploy. Vercel gives you a URL like `https://ticket-system.vercel.app`.

---

## 4. Connect the two

Go back to Render and set `Cors__AllowedOrigins__0` to your Vercel URL (no trailing slash), then redeploy the API. Without this the browser will block API calls.

## 5. Smoke test

1. Open the Vercel URL.
2. Log in with `admin@ticket.local` / `Admin@123`.
3. Confirm the dashboard loads and you can open and create a ticket.

## Notes

- **Secrets**: never commit a real `Jwt__Key` or database password — set them as platform environment variables.
- **Migrations**: the API runs them on startup, so deploys apply schema changes automatically.
- **File uploads**: attachments are stored on local disk by default, which is ephemeral on Render. For durable storage, swap `IFileStorage` for an S3 / Supabase Storage implementation.
