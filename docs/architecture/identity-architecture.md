# Identity architecture (Phase 5)

Customers authenticate with **phone number + OTP**. There is no password login, email login, or guest booking in V1. The same `users` / `role` tables from Phase 1 are used. There is no second identity store.

## Flow

1. Client submits a phone number.
2. The API normalizes it to E.164 and creates a hashed OTP challenge.
3. `IPhoneOtpSender` delivers the code (Development sender in local/test only).
4. On verify, a Customer user is created if needed (Customer role only for new accounts).
5. A short-lived JWT access token is issued. A hashed refresh session is stored.
6. Browser clients keep the refresh credential in an HttpOnly cookie (via the Next.js BFF). Mobile clients use `X-Auth-Client: bearer` and receive the refresh token in JSON.

## Phone normalization

Default region is India (`+91`).

| Input | Stored |
| --- | --- |
| `9876543210` | `+919876543210` |
| `+919876543210` | `+919876543210` |
| `919876543210` | `+919876543210` |

Numbers that already include `+` and match E.164 (`+` + 8–15 digits) are accepted so later countries are possible. `users.phone_e164` is unique when present and must match that pattern (check constraint). Empty strings are not stored.

`password_hash` remains nullable and unused. It is reserved if a later phase needs staff passwords. Public OTP never reads or writes it.

## OTP challenges (`otp_challenge`)

- Cryptographically random numeric code (default length 6).
- HMAC-SHA256 hash with per-challenge salt + configured pepper. Plaintext is not stored.
- Default expiry 300 seconds, 5 verify attempts, 60-second resend cooldown, 5 requests per phone per hour.
- Previous unused challenges for the same phone are consumed on resend.
- Success consumes the row (single use).
- Values are never logged or returned in Production.

Configuration: `Auth:Otp` in `appsettings.json`.

## OTP provider

`IPhoneOtpSender` is the only send dependency.

| Provider | When |
| --- | --- |
| `Development` | Development and Testing only. In-memory last code; `GET /api/v1/auth/otp/dev-peek` |
| `Unconfigured` | Staging/Production until an SMS vendor is wired. Request fails closed (503). |

Production **cannot** set `Auth:Otp:Provider` to `Development` (options validation + factory guard). Future adapters (MSG91, Exotel, Twilio, SNS) implement `IPhoneOtpSender`.

## Sessions and tokens

| Token | Lifetime | Storage |
| --- | --- | --- |
| Access JWT | 15 minutes (`Auth:Jwt:AccessTokenMinutes`) | Authorization header. Browser memory only. |
| Refresh | 14 days | SHA-256 in `refresh_session`. Browser: HttpOnly cookie. Mobile: JSON body. |

Refresh rotation: each refresh revokes the old row and inserts a replacement. Replaying a replaced token revokes remaining sessions for that user (`refresh_replay` audit).

JWT claims: `sub` / name identifier = user id, `role` values from `user_role`, `phone`, `customer_id` when present. Clients cannot choose a role.

Logout revokes the current refresh session and clears cookies.

## Browser vs mobile

- **Next.js BFF** (`/api/auth/*`): calls the API with `X-Auth-Client: bearer`, stores refresh in HttpOnly `bt_refresh`, sets non-HttpOnly `bt_csrf`. Mutating BFF routes require `X-CSRF-Token`.
- **API cookies** (direct browser-to-API): HttpOnly refresh + CSRF cookie on path `/api/v1/auth`. Cookie `SameSite` from `Auth:Cookie:SameSite` (Lax locally; use `None` + `Secure` when the web origin and API origin are cross-site in production).
- **Mobile**: `X-Auth-Client: bearer` on verify/refresh/logout; refresh token in JSON. Do not put refresh tokens in `localStorage`.

## CSRF

Cookie-authenticated refresh/logout require a matching `X-CSRF-Token` header (double-submit). Same-site Lax cookies additionally reduce cross-site POST risk. CORS never uses `AllowAnyOrigin` with credentials.

## CORS

`Cors:AllowedOrigins` explicit list. Development includes `http://127.0.0.1:43121` and `43122`. Credentials allowed only with that allowlist. Production/staging: set origins via configuration.

## Forwarded headers

`UseForwardedHeaders` runs before rate limiting. Loopback is trusted by default. Production must set `ForwardedHeaders:KnownProxies` (and optionally `KnownNetworks`) to the load balancer. Do not trust arbitrary internet `X-Forwarded-For`.

## Rate limiting

Named policy `auth` (10/min per IP) is applied to `/api/v1/auth/*`. Application rules add per-phone cooldown and hourly caps. OTP request cooldown (and hourly cap) return HTTP 429 with a `Retry-After` delta in seconds and `retryAfterSeconds` in the problem body. The values are wait times only; they do not describe OTP or challenge state.

## Roles

New self-registration: `customer` role only + `customer` profile. Existing identities keep their roles; a missing customer profile/role is added so they can book later. Admin and driver accounts are not created through these endpoints.

## Audit (`audit_log`)

Events: `otp_requested`, `otp_verified`, `otp_verify_failed`, `otp_verify_locked`, `session_created`, `logout`, `refresh_replay`. Payloads may include phone last-4, never OTP or tokens.

## Endpoints

```text
POST /api/v1/auth/otp/request
POST /api/v1/auth/otp/verify
POST /api/v1/auth/refresh
POST /api/v1/auth/logout
GET  /api/v1/auth/me
GET  /api/v1/auth/otp/dev-peek   # Development/Testing only
```
