# API design

Phase 2 implements the HTTP kernel and health endpoints. Business resources are not implemented.

## Style

- REST over HTTPS in production; HTTP is allowed locally
- JSON, camelCase
- ASP.NET Core controllers (thin)
- OpenAPI / Swagger in Development at `/swagger`
- Future business routes: `/api/v1/{resource}` (kebab-case paths, plural nouns)
- Health is unversioned: `/health`, `/health/live`, `/health/ready`, `/api/health`

There is no versioning library. Introduce `/api/v2` only for a breaking public contract.

## Envelope

Success: the resource JSON (no unused wrapper).

Errors: RFC 7807 Problem Details (`application/problem+json`) with `title`, `status`, `detail` (safe), `traceId`, and `errors` for validation.

```json
{
  "type": "https://httpstatuses.io/409",
  "title": "Conflict",
  "status": 409,
  "detail": "The request conflicts with existing data.",
  "traceId": "0HMV…"
}
```

## HTTP status codes

| Code | When |
| --- | --- |
| 200 | Successful read or in-place update |
| 201 | Resource created |
| 204 | Success with no body |
| 400 | Validation or check/FK integrity |
| 401 | Unauthenticated (Phase 3+) |
| 403 | Authenticated but not allowed (Phase 3+) |
| 404 | Missing resource or unknown route |
| 409 | Unique/exclusion conflict (e.g. overlapping assignment) |
| 422 | Use when a later phase needs semantic validation distinct from 400 |
| 429 | Rate limit |
| 500 | Unexpected error (no stack trace in production) |

Do not return 200 for failures.

## Naming (future)

- Actions as sub-resources: `POST /api/v1/bookings/{id}/accept`
- List: `page` + `pageSize` (capped), stable sort (`pickupAt desc` for bookings)

## Current endpoints

```text
GET /health
GET /health/live
GET /health/ready
GET /api/health
```

`GET /api/health` → 200

```json
{
  "status": "ok",
  "service": "BangaloreTaxi.Api",
  "phase": "5",
  "utcNow": "2026-08-22T09:00:00+00:00"
}
```

## Auth endpoints (Phase 5)

```text
POST /api/v1/auth/otp/request
POST /api/v1/auth/otp/verify
POST /api/v1/auth/refresh
POST /api/v1/auth/logout
GET  /api/v1/auth/me
```

OTP request body: `{ "phoneNumber": "9876543210" }`. Always a generic success; it does not reveal whether the account exists. Production never returns the OTP. Resend cooldown (and the hourly request cap) respond with HTTP 429, a `Retry-After` delay in seconds, and `retryAfterSeconds` in the problem body. Those values are wait times only.

OTP verify body: `{ "phoneNumber": "9876543210", "otp": "123456" }`. Response includes `accessToken`, `accessTokenExpiresAt`, `user`. `refreshToken` is included only when `X-Auth-Client: bearer` is sent (mobile / Next.js BFF).

`GET /api/v1/auth/me` requires `Authorization: Bearer`. Returns `userId`, `customerId`, `phoneNumber`, `maskedPhone`, `roles`.

See [identity-architecture.md](../architecture/identity-architecture.md).

## Future endpoints (not implemented)

```text
GET    /api/v1/bookings
POST   /api/v1/bookings
GET    /api/v1/bookings/{id}
POST   /api/v1/bookings/{id}/accept
POST   /api/v1/bookings/{id}/reject
POST   /api/v1/bookings/{id}/assign-driver
POST   /api/v1/bookings/{id}/cancel

GET    /api/v1/drivers
GET    /api/v1/vehicles
GET    /api/v1/pricing
```

There are no `/api/payments` routes in V1.

## Authorization (future)

| Area | Access |
| --- | --- |
| Health | Anonymous |
| Booking create | Authenticated customer (Phase 6+). Guest booking is not in V1. |
| Booking read | Owner or admin |
| Accept/reject/assign | Admin |
| Pricing quote | Public or authenticated; calculation always server-side |
| Pricing rule admin | Admin |
| SEO write | Admin |
| SEO public read | Public, published pages only |

## Validation

Every write endpoint validates DTOs (`[ApiController]` + DataAnnotations). Fail closed. Do not bind entity models to controllers.

## Idempotency

Assignment and accept should be safe to retry: never duplicate side effects; return current state or 409.

## Rate limiting

Global per-IP window. `[EnableRateLimiting("auth")]` is applied to `/api/v1/auth/*`. `[EnableRateLimiting("public-write")]` remains for a future booking create.
