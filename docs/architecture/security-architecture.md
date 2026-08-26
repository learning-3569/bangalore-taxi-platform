# Security architecture

The platform will store customer names, phone numbers, addresses, and trip history. Treat that as personal data. Authentication is Phase 3; this document binds later phases.

## Principles

1. Never commit secrets, connection strings with passwords, API keys, or `.env` files.
2. Never hardcode credentials.
3. Never trust the frontend for authorization.
4. Validate and sanitize all input at the API boundary.
5. Deny by default.

## Authentication (Phase 3+)

- Customers authenticate against the API. Exact mechanism (cookie session vs bearer) is decided in Phase 3; prefer HTTP-only cookies for browser apps if CSRF can be handled cleanly, or bearer tokens with consistent CSRF/XSS controls.
- Admins authenticate separately. Admin accounts are not customer accounts with a flag unless a later ADR justifies it.
- Passwords: salted hashes using ASP.NET Identity password hasher or equivalent; never store plaintext or reversible encryption.
- Reset password via time-limited, single-use tokens.

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

## CSRF

If cookie-based auth is chosen, use antiforgery tokens or SameSite strategies documented in Phase 3. SPA bearer-in-header setups reduce CSRF but increase XSS impact; pick one model and apply it consistently.

## XSS

- React/Next default escaping; do not use `dangerouslySetInnerHTML` for CMS HTML without a sanitizer.
- SEO CMS content in Phase 11 must be sanitized before render.
- Content-Security-Policy to be added in Phase 12.

## SQL injection

- EF Core parameterized queries only.
- No concatenated SQL. If raw SQL is ever required, use parameters.

## Rate limiting

- Foundation: global per-IP fixed window (see backend architecture). Named `auth` and `public-write` policies are registered for later endpoints.
- Phase 12 can tighten login, booking create, and password reset further.

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
