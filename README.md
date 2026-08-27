# Bangalore Taxi Booking Platform

Advance taxi booking for a Bangalore fleet of about 20 cars. Customers discover the service through Google search, request trips in advance, and receive confirmation with driver details after an administrator accepts the request and assigns a vehicle.

**Current Phase: Phase 5 — Phone number + OTP authentication (complete)**

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
| Database | PostgreSQL 16+ (EF Core schema in Phase 1) |
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
| 0 | Architecture & Project Setup |
| 1 | Database Foundation |
| 2 | ASP.NET Core Backend Foundation |
| 3 | Public website UI/UX + SEO foundation (complete) |
| 4 | SEO route & location page foundation (complete) |
| 4B | Scalable route catalog foundation (complete) |
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
- PostgreSQL 16+ (Docker Compose: `docker compose up -d` or `scripts/dev-postgres.sh`)

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
dotnet tool restore
```

Apply the database (after PostgreSQL is running; see [docs/database/local-setup.md](docs/database/local-setup.md)):

```bash
dotnet ef database update --project apps/api/BangaloreTaxi.Api.csproj --startup-project apps/api/BangaloreTaxi.Api.csproj
```

## How to run each application

Default local ports (uncommon on purpose):

| App | URL |
| --- | --- |
| Public website | http://127.0.0.1:43121 |
| Admin portal | http://127.0.0.1:43122 |
| API (Swagger in Development) | http://127.0.0.1:43130/swagger |
| API health (live) | http://127.0.0.1:43130/health/live |
| API health (ready, PostgreSQL) | http://127.0.0.1:43130/health/ready |
| API identity | http://127.0.0.1:43130/api/health |

```bash
# Public website
cd apps/web && npm run dev

# Admin portal
cd apps/admin && npm run dev

# API
cd apps/api && dotnet run --launch-profile http
```

Helper scripts: `scripts/dev-web.sh`, `scripts/dev-admin.sh`, `scripts/dev-api.sh`, `scripts/dev-postgres.sh`.

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

## What Phase 4 includes

Six high-quality route landers (airport + reverse airport + outstation), typed SEO content model, reusable `RouteLandingPage`, sitemap of published slugs only, breadcrumbs and conservative JSON-LD. No mass-generated locality pages. Details: [SEO architecture](docs/seo/seo-architecture.md).

## What Phase 3 includes

Public site (`apps/web`): mobile-first homepage, design tokens, header/footer, UI-only booking form, SEO metadata, robots, sitemap of real URLs, conservative JSON-LD. Details: [public website UI](docs/architecture/public-website-ui.md).

## What Phase 2 includes

- HTTP kernel: Problem Details, exception mapping, CORS, security headers, rate limiting, structured request logs with `traceId`
- Liveness/readiness health checks against PostgreSQL
- OpenAPI/Swagger in Development
- Options pattern for operations, CORS, and rate limits
- Pipeline and PostgreSQL-ready integration tests

```bash
docker compose up -d
dotnet ef database update --project apps/api/BangaloreTaxi.Api.csproj --startup-project apps/api/BangaloreTaxi.Api.csproj
cd apps/api && dotnet run --launch-profile http
```

Then open http://127.0.0.1:43130/health/live and http://127.0.0.1:43130/swagger

## What Phase 2 does not include

Customer auth, booking/pricing/fleet APIs, notifications, Maps, SEO CMS, payment, or frontend work.

## Next recommended phase

Await an explicit request. Do **not** start customer authentication, the booking engine, admin, or payment automatically.
