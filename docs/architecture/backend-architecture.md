# Backend architecture

## Application

`apps/api` is an ASP.NET Core 8 Web API (`BangaloreTaxi.Api`). It is a modular monolith: one process, one solution, modules by business capability.

OpenAPI/Swagger is enabled in Development (`/swagger`).

Phase 0 exposes only `GET /api/health`.

## Organization

When a module is implemented, prefer this shape inside the API project (or a dedicated project only if a later ADR says so):

```text
Bookings/
  Controllers/
  Services/
  DTOs/
  Models/
  Validators/
```

Do not create empty module folders before the phase that needs them.

Do not apply the Repository pattern everywhere. EF Core `DbContext` is already an abstraction. Add repositories only when they hide a real querying complexity or enable a test seam that is otherwise painful.

Controllers stay thin: parse HTTP, call a service, map errors to problem details. Domain and application rules live in services.

## Future modules

Authentication, Customers, Bookings, Pricing, Drivers, Vehicles, Notifications, SEO, Administration.

There is no Payment module in V1. Do not add payment controllers, services, or tables. See [ADR-004](../decisions/ADR-004-no-payment-v1.md).

## API conventions

Documented in [API design](../api/api-design.md). Summary:

- REST, JSON, `/api/{resource}`
- Validation on every write
- ProblemDetails for errors
- Server-side authorization; ignore client-supplied roles
- Idempotent assignment operations where practical

## Persistence

EF Core + PostgreSQL from Phase 1. Migrations are the source of schema truth. The API owns transactions for booking assignment so double-booking cannot occur from a race. Details: [database design](../database/database-design.md).

## Integrations

Maps and notifications are consumed through interfaces owned by the API. Vendor SDKs stay in adapter classes. Configuration via environment variables.

## Testing

| Project | Role |
| --- | --- |
| `tests/unit` | Pure unit tests of domain/application logic |
| `tests/integration` | HTTP pipeline tests via `WebApplicationFactory` |

Phase 0: health contract unit test + health endpoint integration test.

Write tests for important business logic as soon as that logic exists (pricing, assignment, cancellation rules). Do not add a second test framework.

## Configuration

- `appsettings.json` — non-secret defaults
- `appsettings.Development.json` — local CORS origins
- Environment variables / `.env` (not committed) — secrets and connection strings

`TreatWarningsAsErrors` is enabled on API and test projects.
