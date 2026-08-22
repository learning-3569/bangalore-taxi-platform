# API design

Phase 0 implements only `GET /api/health`. All other endpoints are future conventions.

## Style

- REST over HTTPS
- JSON
- ASP.NET Core controllers
- OpenAPI / Swagger in Development
- Versioning: URL prefix `/api/` for V1; add `/api/v2` only if a breaking public contract appears

## Envelope

Success: the resource JSON (no unused wrapper).

Errors: RFC 7807 ProblemDetails (`application/problem+json`) with `title`, `status`, `detail` (safe), and `errors` for validation.

## Naming

- Kebab-case paths
- camelCase JSON
- Plural nouns: `/api/bookings`, `/api/drivers`, `/api/vehicles`
- Actions as sub-resources when they are state transitions: `POST /api/bookings/{id}/accept`

## Future endpoints (not implemented)

```text
GET    /api/health

GET    /api/bookings
GET    /api/bookings/{id}
POST   /api/bookings
PUT    /api/bookings/{id}
POST   /api/bookings/{id}/accept
POST   /api/bookings/{id}/reject
POST   /api/bookings/{id}/assign-driver
POST   /api/bookings/{id}/cancel

GET    /api/drivers
GET    /api/vehicles

GET    /api/pricing
```

Additional resources will appear with their phases (customers, seo-pages, pricing rules). Do not implement them early.

There are no `/api/payments` routes in V1.

## Authorization (future)

| Area | Access |
| --- | --- |
| Health | Anonymous |
| Booking create | Authenticated customer (or a documented guest-booking exception) |
| Booking read | Owner or admin |
| Accept/reject/assign | Admin |
| Pricing quote | Public or authenticated, but calculation always server-side |
| Pricing rule admin | Admin |
| SEO write | Admin |
| SEO public read | Public, published pages only |

## Validation

Every write endpoint validates DTOs. Fail closed. Do not bind entity models directly to controllers.

## Idempotency

Assignment and accept should be safe to retry: accepting an already-accepted booking returns the current state or a documented 409, never a duplicate side effect.

## Pagination

List endpoints use `page` + `pageSize` (capped) and stable sort (`pickupAt desc` for bookings).

## Health contract (Phase 0)

`GET /api/health` → 200

```json
{
  "status": "ok",
  "service": "BangaloreTaxi.Api",
  "phase": "0",
  "utcNow": "2026-08-22T09:00:00+00:00"
}
```
