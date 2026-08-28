# Backend architecture

## Phase 8 assignment boundary

`AdminFleetController` exposes bounded admin-only driver and vehicle candidate DTOs. `AdminBookingService` remains the authoritative assignment boundary: it re-reads persisted eligibility and exact requested vehicle type, locks the chosen resources in a transaction, computes the existing assignment range from operational settings, and atomically writes booking snapshots, `accepted → driver_assigned`, history, and audit data. PostgreSQL driver and vehicle exclusion constraints remain the final overlap guard; their failures are translated to safe `409` responses. Initial assignment only is supported.

The same admin fleet boundary now owns operational CRUD and roster tagging. Driver creation atomically creates `users`, `user_role(driver)`, and `driver`; the browser never selects a role. Driver numbers come from a PostgreSQL sequence, and driver/vehicle updates use `xmin`. Deactivation is reversible and preserves history. Roster changes close current temporal rows and insert replacements in one transaction, while booking snapshots remain immutable.

## Application

`apps/api` is an ASP.NET Core 8 Web API (`BangaloreTaxi.Api`). It is a modular monolith: one process, one PostgreSQL database, one deployable API. See [ADR-001](../decisions/ADR-001-modular-monolith.md).

Phase 2 provides the HTTP kernel; Phase 5 adds phone/OTP identity and Phase 6 adds authenticated customer bookings. There is no Payment module in V1 ([ADR-004](../decisions/ADR-004-no-payment-v1.md)).

OpenAPI/Swagger is enabled in Development at `/swagger`.

## Why a single project

Phase 0 placed the API in `apps/api` with tests in `tests/unit` and `tests/integration`. Phase 2 **keeps that layout**. Separate Domain/Application/Infrastructure class libraries are not required yet: they would add project ceremony without extra modules to isolate.

Conceptual layers live as folders inside `BangaloreTaxi.Api`:

```text
apps/api/
  Program.cs                 Host
  Application/               Exceptions used by services (no ASP.NET types)
  Configuration/             Options (CORS, operations, rate limits)
  Hosting/                   Pipeline, DI, errors, health, security headers
  Health/                    GET /api/health identity payload
  Persistence/               EF Core, PostgreSQL, migrations (infrastructure)
```

Capability folders (`Bookings/`, `Auth/`) are added only when implemented, not as empty shells.

```text
Bookings/
  Controllers/
  Services/
  DTOs/
  Validators/
```

### Dependency direction

```text
Hosting / Controllers  →  Application  →  (future domain services)
Hosting / Persistence  →  PostgreSQL
```

Controllers stay thin. Do not put business rules in `Program.cs` or controllers. EF Core `DbContext` is the persistence abstraction; do not add a generic repository layer.

Domain concepts must not take a dependency on HTTP or vendor SDKs. Persistence entities currently sit in `Persistence/Entities` because they are EF-mapped. If a later phase needs persistence-free domain types, extract them then.

## Modules

Identity/Customers (OTP in Phase 5), customer Bookings (Phase 6), and admin booking decisions (Phase 7) are implemented. Drivers, Vehicles, Pricing, Notifications, and SEO administration remain future operational modules.

Payment is a future phase only.

## Health

| Endpoint | Meaning | Database |
| --- | --- | --- |
| `GET /health/live` | Process is up (liveness) | No |
| `GET /health/ready` | Ready to take traffic (readiness) | Yes — PostgreSQL `CanConnect` |
| `GET /health` | Same as readiness | Yes |
| `GET /api/health` | Service identity (name, phase, UTC now) | No |

Ready/live responses are JSON `{ status, checks[] }` with **no** connection strings or exception text.

Kubernetes is not used. The split exists so a load balancer or future orchestrator can probe liveness without failing when the database is briefly unavailable.

The API does **not** run migrations on startup. Apply schema with `dotnet ef database update`.

## Configuration

| File | Role |
| --- | --- |
| `appsettings.json` | Non-secret defaults (operations, rate limit) |
| `appsettings.Development.json` | Local CORS + local connection string |
| `appsettings.Staging.json` | Staging CORS empty until env is set |
| `appsettings.Testing.json` | Test CORS origins |
| Environment / `.env` | Secrets and production connection string |

`ConnectionStrings:DefaultConnection` is required in Staging/Production. Development and Testing may fall back to the documented local Docker credentials.

`TimeProvider` is registered as a singleton (`TimeProvider.System`). System timestamps are UTC. Phase 6 interprets submitted local pickup values in IANA `Asia/Kolkata`, persisting the UTC instant in `pickup_at`, the zone in `pickup_timezone`, and the local calendar date in `pickup_local_date`.

`IHttpClientFactory` is registered. Named clients for Maps, WhatsApp, SMS, email, and payment are added when those phases exist. Do not register fake providers.

### DI lifetimes

| Lifetime | Use |
| --- | --- |
| Singleton | `TimeProvider`, stateless helpers |
| Scoped | `BangaloreTaxiDbContext`, future request services |
| Transient | Lightweight stateless helpers with no shared state |

Do not resolve scoped services from singletons. Do not use a service locator.

## Errors

`IExceptionHandler` + RFC 7807 Problem Details. Production never returns stack traces, connection strings, or SQL text.

| Situation | Status |
| --- | --- |
| Validation (`ApiController` model state) | 400 |
| PostgreSQL foreign key / check | 400 |
| Missing resource (`NotFoundException`) | 404 |
| Unique or exclusion violation; `ConflictException` | 409 |
| Rate limit | 429 |
| Unhandled | 500 |

`traceId` is `HttpContext.TraceIdentifier` (and Activity when present). Log the same id with the exception. Do not log passwords, tokens, OTPs, or connection strings.

Success bodies remain the resource JSON (no wrapper). HTTP status codes carry the outcome.

## Validation

`[ApiController]` + DataAnnotations on future DTOs. Invalid model state returns Problem Details with `errors`. Do not bind EF entities to controllers. FluentValidation is not added; add it later only if DataAnnotations are insufficient.

## Transactions and concurrency

No global per-request transaction. Booking creation uses an explicit transaction and an atomic PostgreSQL upsert of `booking_number_sequence`; the booking and initial history commit together. Cancellation and admin accept/reject use PostgreSQL `xmin` optimistic concurrency. Each successful admin decision updates status and inserts history plus audit data in one save; competing decisions return 409 and create no partial history/audit. The UI is not the concurrency guard.

## Logging

Built-in `ILogger`. Each request logs method, path, status, and trace id — **not** query strings or bodies (those may contain PII or tokens).

## CORS

Policy `FrontendApps`. Origins from `Cors:AllowedOrigins`. Never `AllowAnyOrigin` with credentials. Development: `http://127.0.0.1:43121` and `http://127.0.0.1:43122` (and localhost aliases). Production/staging origins come from environment configuration.

## Security headers

`X-Content-Type-Options: nosniff`, `X-Frame-Options: DENY`, `Referrer-Policy: no-referrer`, `Permissions-Policy`. CSP is relaxed in Development (Swagger) and `default-src 'none'` otherwise. Broader CSP/HSTS at the edge can be tightened in Phase 12.

## HTTPS

Local Development: HTTP on `127.0.0.1:43130`. Production: `UseHsts` + `UseHttpsRedirection`; terminate TLS at the host. No certificates in this repo.

## Authentication / authorization / audit

Phone + OTP is implemented (Phase 5). See [identity-architecture.md](identity-architecture.md). Role names in the database: `customer`, `admin`, `driver`. JWT bearer on the API; refresh sessions in PostgreSQL.

Audit rows are written from `AuthService` for OTP/session events and from `AdminBookingService` for accept/reject actions — not as a blanket EF interceptor. Rejection detail stays in the internal audit record; customer history receives the safe message “Booking request not accepted.”

## Rate limiting

Built-in `AddRateLimiter`. Global fixed window per IP (default 120/minute). Named policies `auth` (10/min), `public-write` (30/min), and `admin-write` (30/min per authenticated user, IP fallback). Health checks disable rate limiting.

## Request size

Kestrel `MaxRequestBodySize` = 1 MiB.

## API versioning

Business routes will use `/api/v1/{resource}` by convention. No versioning NuGet package. A `/api/v2` prefix is added only if a breaking public contract appears. Health stays unversioned (`/health`, `/api/health`).

## Testing

| Project | Role |
| --- | --- |
| `tests/unit` | Exception mapping, formatters, contracts |
| `tests/integration` | Pipeline, PostgreSQL schema, `/health/ready` |

Schema and ready checks use `bangalore_taxi_test` (or `SCHEMA_TEST_CONNECTION` in CI), never `EnsureDeleted` on `bangalore_taxi`. Do not use EF InMemory for PostgreSQL behavior.

`TreatWarningsAsErrors` is enabled on API and test projects.
