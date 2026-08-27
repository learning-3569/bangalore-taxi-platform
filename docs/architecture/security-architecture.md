# Security architecture

The platform will store customer names, phone numbers, addresses, and trip history. Treat that as personal data. Customer authentication is Phase 5 (phone + OTP).

## Principles

1. Never commit secrets, connection strings with passwords, API keys, or `.env` files.
2. Never hardcode credentials.
3. Never trust the frontend for authorization.
4. Validate and sanitize all input at the API boundary.
5. Deny by default.

## Authentication (Phase 5)

Customer authentication is phone + OTP. See [identity-architecture.md](identity-architecture.md).

- No password, email, or guest login in V1.
- Self-registration assigns the `customer` role only.
- Access tokens are JWTs; refresh sessions are hashed in PostgreSQL.
- Browser refresh credentials are HttpOnly cookies (Next.js BFF). Do not store refresh tokens in `localStorage`.
- CSRF: `X-CSRF-Token` on cookie-authenticated refresh/logout.
- Production SMS is not wired; `Auth:Otp:Provider` must not be `Development` in Production.

## Authorization

- Roles: at minimum `Customer` and `Admin`. Additional roles (`Dispatcher`) only if operations need them.
- Enforce on the API for every protected endpoint.
- Customers may access only their own bookings.
- Admin portal UI hiding is not security.

## API validation

- Data annotations and/or FluentValidation on DTOs.
- Reject unknown enums, impossible coordinates, and past pickup times per business rules.
- Bounded string lengths matching database columns.

## HTTPS

- Production: TLS only. Redirect HTTP to HTTPS at the edge.
- Local Development API runs HTTP on `127.0.0.1:43130`. Production enables HSTS and HTTPS redirection; certificates live on the host, not in git.

## CORS

- Allow only the public site and admin origins.
- Development origins are listed in `appsettings.Development.json`.
- Production origins come from configuration, not `AllowAnyOrigin` with credentials.
- Credentials are enabled only with an explicit origin list (cookie-based refresh).

## CSRF

Browser refresh/logout use a double-submit `X-CSRF-Token` header matched to the `bt_csrf` cookie. Mobile/BFF clients send `X-Auth-Client: bearer` and do not rely on cookies. See [identity-architecture.md](identity-architecture.md).

## XSS

- React/Next default escaping; do not use `dangerouslySetInnerHTML` for CMS HTML without a sanitizer.
- SEO CMS content in Phase 11 must be sanitized before render.
- Content-Security-Policy to be added in Phase 12.

## SQL injection

- EF Core parameterized queries only.
- No concatenated SQL. If raw SQL is ever required, use parameters.

## Rate limiting

- Foundation: global per-IP fixed window (see backend architecture). Named `auth` (10/min) is applied to OTP request/verify/refresh/logout. Per-phone cooldown and hourly caps are enforced in `AuthService`.
- Configure `ForwardedHeaders:KnownProxies` in production so IP limits see the real client, not the load balancer.

## Security headers

- API sets `X-Content-Type-Options`, `X-Frame-Options`, `Referrer-Policy`, `Permissions-Policy`, and a conservative CSP (Swagger-friendly in Development).
- Content-Security-Policy for the public website remains a Phase 12 concern for `apps/web`.

## Configuration and secrets

- Development, Staging, Production.
- Secrets in the host environment or a secret store, not git.
- `.env.example` files contain variable names (and non-secret local URLs) only.

## Audit logging

- Record admin actions on bookings: accept, reject, assign, cancel, status change.
- Record authentication failures at a safe detail level (no passwords).
- `AuditLog` table exists (Phase 1). Write rows from application services after successful admin mutations. Do not intercept every EF change.

## Error handling

- Do not leak stack traces to clients in production.
- Use ProblemDetails.
- Log internally with `traceId` (`HttpContext.TraceIdentifier`).
- PostgreSQL unique/exclusion violations map to 409, not 500.

## Admin isolation

- Admin app sends `noindex`.
- Restrict admin URL at the network layer in production if possible (VPN or IP allowlist) in addition to authentication.

## Public site indexing

- `apps/web` is noindex unless `INDEX_PUBLIC=true` (or `NEXT_PUBLIC_INDEX_PUBLIC=true`).
- Do not treat a public-looking hostname (including Vercel previews) as permission to index.
- Catalog review fixtures stay unpublished so they are not generated as public HTML.
- Legal placeholder pages stay `noindex` and off the sitemap until approved copy exists.
- `/login` is `noindex` (account URL, not a marketing lander).
