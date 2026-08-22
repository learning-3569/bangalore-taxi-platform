# Bangalore Taxi Booking Platform

Advance taxi booking for a Bangalore fleet of about 20 cars. Customers discover the service through Google search, request trips in advance, and receive confirmation with driver details after an administrator accepts the request and assigns a vehicle.

**Current Phase: Phase 0 — Architecture & Project Setup**

This repository is a production product developed incrementally. Do not implement later phases unless explicitly requested.

## Business objectives

1. Generate customers through Google organic search.
2. Let customers request taxis in advance (airport, local, outstation, one-way, round-trip, corporate).
3. Let administrators manage booking requests.
4. Let administrators assign drivers and vehicles.
5. Send booking confirmation and driver information to customers.

V1 does **not** include online payment. Payment is a future phase only if the client requests it. See [ADR-004](docs/decisions/ADR-004-no-payment-v1.md).

## Technology stack

| Layer | Choice |
| --- | --- |
| Public website | Next.js 15 (App Router), TypeScript, Tailwind CSS |
| Admin portal | Next.js 15 (App Router), TypeScript, Tailwind CSS — separate app |
| Backend | ASP.NET Core 8 Web API, C# |
| Database | PostgreSQL (schema begins in Phase 1) |
| API docs | OpenAPI / Swagger |
| Tests | xUnit + `Microsoft.AspNetCore.Mvc.Testing` |

## Architecture

Modular monolith. One deployable API organized by business capability. Two frontend applications share that API.

- Public website: SEO-first, SSR/SSG, indexable marketing and booking surfaces.
- Admin portal: internal, noindex, operations UI.
- API: REST, thin controllers, business logic in application services.

Do not introduce microservices, Kubernetes, Kafka, or a service mesh. See [system architecture](docs/architecture/system-architecture.md) and [ADR-001](docs/decisions/ADR-001-modular-monolith.md).

## Repository structure

```text
apps/web          Public Next.js website
apps/admin        Next.js admin portal
apps/api          ASP.NET Core Web API
tests/unit        xUnit unit tests
tests/integration ASP.NET integration tests
docs/             Architecture, database, API, SEO, roadmap, ADRs
scripts/          Local helper scripts
.cursor/rules/    Cursor Agent project rules
.github/workflows CI builds and tests
```

## Development phases

| Phase | Name |
| --- | --- |
| 0 | Architecture & Project Setup (current) |
| 1 | Database Foundation |
| 2 | ASP.NET Core Backend Foundation |
| 3 | Customer Authentication |
| 4 | SEO-First Public Website |
| 5 | Booking Engine |
| 6 | Pricing Engine |
| 7 | Admin Portal |
| 8 | Driver & Vehicle Management |
| 9 | Notifications |
| 10 | Google Maps Integration |
| 11 | SEO CMS & Landing Pages |
| 12 | Testing & Security Hardening |
| 13 | Performance Optimization |
| 14 | Production Deployment |
| 15 | SEO Launch & Monitoring |
| Future | Online Payment (not V1) |

Full scope, dependencies, and acceptance criteria: [development roadmap](docs/roadmap/development-roadmap.md).

## Local development setup

Prerequisites:

- Node.js 22+
- npm 10+
- .NET SDK 8 (see `global.json`)
- PostgreSQL 16+ is required from Phase 1 onward; Phase 0 does not need a running database

Copy environment templates (names only; no secrets):

```bash
cp .env.example .env
cp apps/web/.env.example apps/web/.env.local
cp apps/admin/.env.example apps/admin/.env.local
cp apps/api/.env.example apps/api/.env
```

Install JavaScript dependencies:

```bash
cd apps/web && npm install
cd ../admin && npm install
```

Restore .NET tools from the repository root:

```bash
dotnet restore
```

## How to run each application

Default local ports (uncommon on purpose):

| App | URL |
| --- | --- |
| Public website | http://127.0.0.1:43121 |
| Admin portal | http://127.0.0.1:43122 |
| API (Swagger in Development) | http://127.0.0.1:43130/swagger |
| API health | http://127.0.0.1:43130/api/health |

```bash
# Public website
cd apps/web && npm run dev

# Admin portal
cd apps/admin && npm run dev

# API
cd apps/api && dotnet run --launch-profile http
```

Helper scripts: `scripts/dev-web.sh`, `scripts/dev-admin.sh`, `scripts/dev-api.sh`.

Verify Phase 0 builds and tests:

```bash
chmod +x scripts/*.sh
./scripts/verify-phase-0.sh
```

## Coding standards

- Inspect existing code and documentation before changing architecture.
- Change only files required for the requested phase.
- Keep public website and admin portal separate.
- Keep API controllers thin; put business rules in services.
- Validate all API input. Never trust frontend authorization.
- Prefer small, reviewable changes.
- Do not add microservices, payment code, or unused abstractions.
- Preserve public URLs once they are published.
- Never commit secrets, `.env` files, or credentials.

## SEO strategy

SEO is a core product requirement, not a later add-on. The public site must remain crawlable, indexable, mobile-first, and served with SSR/SSG, metadata, canonical URLs, structured data (later), sitemap, and `robots.txt`. URL strategy is documented in [docs/seo/url-strategy.md](docs/seo/url-strategy.md). This stack supports a technically strong SEO foundation; it does not guarantee search ranking.

## Security principles

HTTPS in production, role-based authorization on the server, parameterized data access, secret management outside source control, CORS limited to known origins, audit logging for operational actions. Details: [docs/architecture/security-architecture.md](docs/architecture/security-architecture.md).

## What Phase 0 includes

- Repository layout
- Initialized web, admin, and API applications
- Health endpoint and foundation tests
- Architecture documentation and ADRs
- Cursor Agent rules
- CI workflow

## What Phase 0 does not include

Customer auth, bookings, pricing, drivers, vehicles, admin operations, notifications, Google Maps, SEO CMS, or any payment capability.

## Next recommended phase

**Phase 1 — Database Foundation**
