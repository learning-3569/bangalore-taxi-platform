# System architecture

## Current phase

Phase 3 — Public website homepage, design system, and SEO foundation. API remains the Phase 2 HTTP kernel (no booking or auth endpoints).

## Context

The business operates approximately 20 taxis in Bangalore. Customers should find the company through Google search, request a trip in advance, and receive confirmation after an administrator accepts the request and assigns a driver and vehicle. V1 has no online payment.

## Style: modular monolith

One ASP.NET Core process, one PostgreSQL database, two Next.js applications. Backend code is grouped by business capability (Bookings, Pricing, Customers, Drivers, Vehicles, Notifications, SEO, Administration, Authentication).

Rejected for V1: microservices, Kubernetes, Kafka, distributed sagas, service mesh. Scale is a 20-car operation. Complexity would increase cost and failure modes without benefit. See [ADR-001](../decisions/ADR-001-modular-monolith.md).

## Runtime view

```text
Google crawler / customer browser
        │
        ▼
 apps/web  (Next.js, SEO-first, public)
        │  HTTPS REST (JSON)
        ▼
 apps/api  (ASP.NET Core modular monolith)
        │
        ▼
 PostgreSQL (from Phase 1)

Staff browser
        │
        ▼
 apps/admin  (Next.js, noindex, internal)
        │  HTTPS REST (JSON)
        ▼
 apps/api
```

External systems (later, behind interfaces):

- Maps provider (autocomplete, geocode, distance)
- Notification providers (WhatsApp, SMS, email)

Payment providers are **not** part of V1 and must not appear as modules, tables, or UI.

## Applications

| App | Responsibility | Indexing |
| --- | --- | --- |
| `apps/web` | Marketing, SEO pages, customer booking UI | Indexable |
| `apps/admin` | Operations: bookings, fleet, pricing, SEO CMS | `noindex` |
| `apps/api` | Domain rules, persistence, integrations | N/A |

The public website and admin portal must remain separate applications. Do not merge them. Do not share cookies across sites without an explicit later decision.

## Future backend modules

Implemented only when the matching phase is requested:

| Module | First phase |
| --- | --- |
| Shared kernel (errors, time, IDs) | 2 |
| Authentication | 3 |
| Customers | 3 |
| SEO content serving | 4 / 11 |
| Bookings | 5 |
| Pricing | 6 |
| Administration | 7 |
| Drivers | 8 |
| Vehicles | 8 |
| Notifications | 9 |
| Maps (anti-corruption) | 10 |

Do not create empty module folders ahead of those phases.

## Booking flow (future)

```text
Customer → search → pickup/drop/date/time → vehicle type
       → estimated fare (API pricing) → submit request
       → admin receives request → accept/reject
       → admin assigns driver and vehicle
       → booking confirmed → customer notified with driver details
```

Fare estimates are calculated on the backend. The frontend must not hardcode pricing rules.

## Notification architecture (future)

Phase 9. Documented now so later work has a stable seam.

Notifications are domain events handled inside the monolith (in-process), not a message bus.

Customer messages: request received, accepted, rejected, confirmed, driver assigned, driver details, trip reminder, cancellation.

Admin messages: new booking request, customer cancellation.

Provider abstraction:

```text
INotificationSender
  ├── WhatsAppNotificationSender
  ├── SmsNotificationSender
  └── EmailNotificationSender
```

The domain emits a notification intent (template + recipient + payload). The sender implementation is chosen by configuration. Do not call a vendor SDK from controllers or from the Bookings module directly.

## Maps architecture (future)

Phase 10. Isolate Google Maps Platform (or a replacement) behind:

```text
ILocationService
  ├── SuggestAddresses(query)
  ├── Geocode(address)
  └── EstimateRoute(origin, destination)
```

Bookings persist addresses and coordinates. Distance used for pricing comes from this service, with a documented fallback if the provider is unavailable. API keys live in environment configuration only.

## Double-booking protection (future)

When a vehicle is assigned, the API must prevent overlapping assignments for the same vehicle. See [database design](../database/database-design.md). Enforcement belongs in the API transaction, not in the UI.

## Environments

| Environment | Purpose |
| --- | --- |
| Development | Local machines / cloud agent |
| Staging | Pre-production with production-like data shape |
| Production | Live public site, admin, API, PostgreSQL |

Configuration via environment variables and `appsettings.{Environment}.json`. Never commit secrets.

## Deployment shape (Phase 14)

Likely: one API process, one PostgreSQL instance, two Node/Next deployments (or one Node host per app). Exact hosting is decided in Phase 14. Do not introduce container orchestration in earlier phases unless operations require it.

## Quality constraints

Optimize for simplicity, maintainability, security, SEO, performance, and testability. Do not over-engineer for twenty cars. Extend by adding modules inside the monolith, not by splitting services.
