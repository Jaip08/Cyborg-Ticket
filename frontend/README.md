# Helpdesk ERP — Frontend

Next.js 15 (App Router) frontend for the Ticket Management ERP. TypeScript, Tailwind, React Query, and hand-rolled shadcn-style UI primitives.

## Stack

- **Next.js 15** (App Router, standalone output)
- **TypeScript** + **React 18**
- **Tailwind CSS v3** with shadcn slate theme tokens
- **@tanstack/react-query v5** for server state
- **axios** API client with JWT + 401 interceptors
- **react-hook-form** + **zod** for forms
- **Recharts** for dashboard/report charts
- **next-themes** for dark mode (class strategy)
- **sonner** for toasts, **lucide-react** for icons
- Radix UI primitives under `components/ui`

## Getting started

```bash
npm install
cp .env.example .env.local   # point NEXT_PUBLIC_API_URL at the backend
npm run dev
```

The app expects the backend at `http://localhost:5080/api` (override via `NEXT_PUBLIC_API_URL`).

## Project layout

```
app/
  (auth)/        login, register, forgot-password (shared centered layout)
  (app)/         authenticated shell: dashboard, tickets, reports, categories, users
  layout.tsx     providers (theme + react-query) and Toaster
components/
  ui/            shadcn-style primitives
  charts/        Recharts wrappers
  ticket/        ticket-detail building blocks
hooks/           React Query hooks per resource
lib/             api client, auth, types, constants, utils
```

## Auth & roles

The JWT is stored in `localStorage` and attached by an axios request interceptor. A 401 response clears the session and redirects to `/login`. Route protection is client-side in `app/(app)/layout.tsx`.

Role gating (`Admin` / `Manager` / `Employee`):

- **Employees** — create/track tickets, comment publicly. They only see internal notes on tickets they created or are assigned to; can edit/delete only their own tickets.
- **Managers** — assign tickets, post internal notes, manage categories, view reports.
- **Admins** — everything, plus user/role management.

## Build

```bash
npm run build && npm run start
```

A multi-stage `Dockerfile` (node:22-alpine, Next standalone) is included.
