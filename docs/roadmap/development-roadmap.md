# Development roadmap

The product is built one phase at a time in this repository. When a phase is requested, implement only that phase. Do not start the next phase automatically.

**Current phase: Phase 6 — Authenticated Booking Engine (complete)**

The authoritative sequence is **Phase 5 = Phone number + OTP authentication** and **Phase 6 = Authenticated Booking Engine**. Older headings below are retained only as historical planning context and are superseded where their numbering conflicts. Do not start pricing, admin operations, assignment, or payment automatically.

Payment is a future phase outside V1.

---

## Phase 0 — Architecture & Project Setup

**Objective:** Establish repository, applications, documentation, Cursor rules, and verified builds.

**Scope:** Next.js web, Next.js admin, ASP.NET Core API skeleton, health endpoint, tests, docs, ADRs, CI.

**Dependencies:** None.

**Backend:** API project, Swagger, `GET /api/health`.

**Frontend:** Foundation layouts, homepage placeholders, robots/sitemap stubs.

**Database:** Documentation only.

**Tests:** Health unit + integration tests.

**Acceptance criteria:** All three apps build; API tests pass; docs and `.cursor/rules` exist; no business features.

**Out of scope:** Auth, bookings, pricing, fleet, notifications, maps, CMS, payment.

---

## Phase 1 — Database Foundation

**Objective:** Connect PostgreSQL with EF Core and a first migration pipeline.

**Scope:** DbContext, connection configuration, full approved schema, first migration, local runbook, lookup seed data. No booking APIs.

**Dependencies:** Phase 0.

**Backend:** EF Core, Npgsql, design-time factory if needed.

**Frontend:** None required.

**Database:** PostgreSQL instance, migrations, naming conventions.

**Tests:** Migration applies against a test database or equivalent smoke test.

**Acceptance criteria:** `dotnet ef database update` works locally with documented env vars; no secrets in git.

**Out of scope:** Full domain implementation, booking APIs, seed of production content.

---

## Phase 2 — ASP.NET Core Backend Foundation

**Objective:** Shared API kernel: error handling, logging, CORS production config, module folder conventions, ProblemDetails.

**Scope:** Cross-cutting API infrastructure used by later modules.

**Dependencies:** Phase 1.

**Backend:** Pipeline, exception middleware, options pattern.

**Frontend:** Point env URLs only if needed.

**Database:** None unless kernel tables (e.g. audit) are explicitly in scope.

**Tests:** Pipeline tests (unknown route, health).

**Acceptance criteria:** Consistent error shape; Swagger describes health; CORS configurable.

**Out of scope:** Business endpoints.

---

## Phase 3 — Public website UI/UX + SEO foundation (implemented)

**Objective:** Customer-facing homepage, temporary design system, and crawlable SEO plumbing. No auth, no booking API, no payment, no admin, no Maps.

**Scope:** `apps/web` homepage sections, header/footer, UI-only booking widget, legal placeholders, metadata, robots, sitemap of real paths, conservative JSON-LD.

**Dependencies:** Phase 0 frontend foundation.

**Acceptance criteria:** Homepage and navigation work on mobile; production build and web tests pass; no fabricated NAP, reviews, or thin SEO URLs.

**Out of scope:** OTP/login, `POST /api/v1/bookings`, payment, admin portal, driver apps, SMS/WhatsApp, Google Maps.

The original “Phase 3 — Customer Authentication” section below is **not** the current workstream.

---

## Phase 3 — Customer Authentication

**Objective:** Customers can register, log in, log out, reset password, and manage a basic profile.

**Scope:** Identity for customers. Admin identity may be stubbed or implemented if required to test; prefer customer-only unless specified.

**Dependencies:** Phase 2.

**Backend:** Auth endpoints, password hashing, session/token issuance.

**Frontend:** Public site account screens only.

**Database:** User/customer tables.

**Tests:** Register/login/reset validation and security cases.

**Acceptance criteria:** Authenticated customer can load their profile; passwords stored hashed; no secrets logged.

**Out of scope:** Bookings, social login, payment.

---

## Phase 4 — SEO route & location page foundation (implemented)

**Objective:** High-quality, crawlable route landers with a reusable template and a typed content model. No thin mass generation.

**Scope:** Six demonstration route pages, metadata/canonicals, breadcrumbs, related-route linking, sitemap of published slugs, FAQ/Service JSON-LD without fake NAP/prices. Location-page types exist; no `/taxi-service-*` URLs published.

**Dependencies:** Phase 3 public website.

**Acceptance criteria:** Unique title/H1/canonical per published route; unpublished drafts stay out of the sitemap; homepage popular routes link only to real pages; web tests, lint, and production build pass.

**Out of scope:** OTP, booking APIs, pricing engine, Maps, SEO CMS, Category E locality pages, combinatorial route generation.

---

## Phase 4B — Scalable route catalog foundation (implemented)

**Objective:** Location + route catalogs that can grow via CMS without a React file per corridor, without auto-combining localities.

**Scope:** Typed `LocationContent`, `originId`/`destinationId` on routes, `published` vs `indexable`, catalog registry helpers, related-route fallback, reserved-slug validation, parent pages `/airport-taxi-bangalore` and `/outstation-taxi-bangalore`. No PostgreSQL SEO tables.

**Future SEO growth (not done):** SEO CMS → location management → route management → content authoring → review → publish → index → sitemap → Search Console monitoring.

---

## Phase 4C — SEO safety & pre-authentication hardening (implemented)

**Objective:** Close indexing and catalog-safety gaps from the Phase 4B review before authentication work.

**Scope:** Unpublished review fixtures, parent-service `published` gating, explicit `INDEX_PUBLIC` indexing flag, legal-placeholder noindex/sitemap exclusion, single sitemap path helper, catalog validation for catalog-state contradictions. No OTP, booking, payment, or UI redesign.

**Acceptance criteria:** Review fixtures are not generated in production; staging/preview stay noindex unless the flag is set; legal placeholders are noindex and off the sitemap; web tests, lint, and production build pass.

**Out of scope:** Phase 5 authentication, booking APIs, payment.

---

## Phase 5 — Phone number + OTP authentication (implemented)

**Objective:** Customers verify a mobile number with OTP and receive an authenticated session before any future booking API.

**Scope:** E.164 normalization, OTP challenges, Customer-only self-registration, JWT access + rotating refresh sessions, Next.js login UI and BFF cookies, forwarded headers, auth rate limits. No booking persist, no payment, no production SMS vendor.

**Acceptance criteria:** Request/verify/refresh/logout/`me` work; OTP is hashed, single-use, expiry- and attempt-limited; new users get Customer role only; browser refresh is HttpOnly; tests and builds pass.

**Out of scope:** `POST /api/v1/bookings`, passwords, admin/driver self-registration, production SMS.

---

## Phase 6 — Authenticated Booking Engine (implemented)

**Objective:** Authenticated customers create booking requests, receive concurrency-safe `BLR-{year}-{sequence}` numbers, list/read only their own requests, and cancel only safe statuses.

**Scope:** Customer booking API and BFF, Bangalore local-time conversion, pending status/history, per-customer idempotency, `/account/bookings` list/details, and customer cancellation. The nullable database `customer_id` remains for historical/future flexibility, while the V1 public API requires authenticated ownership.

**Cancellation:** `pending`, `accepted`, and `confirmed` may be cancelled. `driver_assigned` is conservatively excluded because assignment represents an operational commitment; all terminal/in-progress states are also excluded.

**Out of scope:** Guest booking, pricing/fare calculation, payment, admin booking operations, maps, and driver/vehicle assignment.

---

## Phase 4 — SEO-First Public Website

**Objective:** Real marketing pages with metadata, internal linking, sitemap expansion, mobile-first content for core Bangalore intents.

**Scope:** Reserved core routes (taxi, airport, outstation, one-way, round-trip, corporate, about, contact, FAQ, cars). High-quality unique copy. No thin duplicates.

**Dependencies:** Phase 0 frontend foundation. CMS (Phase 11) is not required if content is code-managed first.

**Backend:** Optional read-only content endpoints only if needed; static content is acceptable.

**Frontend:** Public website pages, metadata, sitemap.

**Database:** None unless content is already CMS-backed.

**Tests:** Build-time page generation; smoke that metadata is present.

**Acceptance criteria:** Core routes return 200 HTML with unique title/H1/canonical; robots and sitemap include them; admin still noindex.

**Out of scope:** Full CMS, booking wizard, blog at scale unless specified.

---

## Historical Phase 5 — Booking Engine (superseded by Phase 6 above)

**Objective:** Customers submit booking requests; bookings persist with statuses; customers can list upcoming/history and cancel per rules.

**Scope:** Booking entity, create, read, cancel. Admin accept/reject may wait for Phase 7 if specified, but status model should exist.

**Dependencies:** Phases 1–3. Pricing quote may be a stub until Phase 6 (must not hardcode fare logic in the frontend; a temporary server stub is allowed only if Phase 6 is not done).

**Backend:** Bookings module.

**Frontend:** Public booking flow and customer booking lists.

**Database:** Booking tables, indexes, concurrency token.

**Tests:** Create, list, cancel rules, authorization (owner vs other).

**Acceptance criteria:** Request stored as Pending; customer sees their bookings; cannot see others'.

**Out of scope:** Driver assignment, maps, payment, admin desk (unless explicitly included).

---

## Historical Phase 6 — Pricing Engine (not current; future phase number to be reassigned)

**Objective:** Server-side fare estimates from configurable rules.

**Scope:** Base, minimum, per-km, vehicle-specific, airport, toll, waiting, night, one-way, round-trip, outstation.

**Dependencies:** Phase 2. Vehicle types may be enumerated until Phase 8.

**Backend:** Pricing module; quote endpoint.

**Frontend:** Display quote from API only.

**Database:** PricingRule / VehiclePricing.

**Tests:** Deterministic fare cases for each component.

**Acceptance criteria:** Same inputs produce the same fare; frontend contains no fare formulas.

**Out of scope:** Payment, coupons, surge unless requested.

---

## Phase 7 — Admin Booking Operations (implemented)

**Objective:** Authorized staff review booking requests and transition pending requests to accepted or rejected.

**Scope:** Paginated/filterable queue, operational booking detail, accept/reject actions, status history, audit, and concurrency conflict handling in `apps/admin` and `apps/api`.

**Dependencies:** Phases 5 and 6. Existing OTP authentication and persisted roles are reused.

**Backend:** Admin booking endpoints; authorization.

**Frontend:** Admin app operational UI.

**Database:** Audit log for admin actions.

**Tests:** Anonymous/customer authorization failures, role spoofing, pagination/filtering, details, accept/reject, history/audit, invalid and competing transitions, and admin UI states.

**Acceptance criteria:** Admin can accept/reject a pending booking; public site remains unchanged in URL structure.

**Out of scope:** Confirmation/assignment, driver and vehicle operations, notifications, pricing/payment, and SEO CMS.

---

## Phase 8 — Driver & Vehicle Management

**Objective:** Admins manage drivers and vehicles and assign them to bookings without overlap.

**Scope:** CRUD, active flags, assign/change driver and vehicle, double-booking protection.

**Dependencies:** Phases 5 and 7.

**Backend:** Drivers, Vehicles, assignment transaction + conflict error.

**Frontend:** Admin management screens.

**Database:** Exclusion/locking as designed.

**Tests:** Concurrent assignment conflict; overlapping window rejected.

**Acceptance criteria:** Two overlapping assignments for one vehicle cannot both succeed.

**Out of scope:** Driver mobile app.

---

## Phase 9 — Notifications

**Objective:** Send customer and admin messages on booking events.

**Scope:** Abstraction + at least one provider (email or SMS) with config. WhatsApp adapter if credentials exist, otherwise interface + stub.

**Dependencies:** Phase 5+.

**Backend:** Notification module, post-commit dispatch.

**Frontend:** None except copy that messages will be sent.

**Database:** Notification/outbox records as needed.

**Tests:** Event produces the right template intent; failures don't corrupt booking state.

**Acceptance criteria:** Documented events fire; provider isolated behind an interface.

**Out of scope:** Marketing campaigns.

---

## Phase 10 — Google Maps Integration

**Objective:** Autocomplete, coordinates, distance/route for pickup and drop.

**Scope:** `ILocationService` + Google adapter. Keys from environment. Fallback behavior documented.

**Dependencies:** Booking UI (Phase 5).

**Backend:** Maps adapter.

**Frontend:** Public (and admin) address fields use API suggestions.

**Database:** Persist coordinates on bookings (already in model).

**Tests:** Adapter mocked; booking still possible with manual address if provider down (if that is the chosen fallback).

**Acceptance criteria:** No Google keys in git; domain does not depend on SDK types.

**Out of scope:** Live driver tracking.

---

## Phase 11 — SEO CMS & Landing Pages

**Objective:** Business owner publishes SEO pages from admin without code changes.

**Scope:** SeoPage CRUD, publish, slug validation, public renderer, sitemap includes published pages, FAQ structured data.

**Dependencies:** Phases 4 and 7.

**Backend:** SEO module.

**Frontend:** Admin editor; public dynamic/ISR pages.

**Database:** SeoPage tables.

**Tests:** Unpublished 404; slug collision with reserved routes rejected.

**Acceptance criteria:** New published slug appears in HTML and sitemap after revalidation.

**Out of scope:** Thin mass-generated doorway pages.

---

## Phase 12 — Testing & Security Hardening

**Objective:** Raise confidence and close security gaps.

**Scope:** Broader tests, rate limiting, CSP, HTTPS enforcement, review of authz, dependency audit.

**Dependencies:** Features to date.

**Backend/Frontend:** Hardening only.

**Database:** Indexes/constraints review.

**Tests:** Expansion of critical path coverage.

**Acceptance criteria:** Documented checklist signed off in the phase report.

**Out of scope:** New product features.

---

## Phase 13 — Performance Optimization

**Objective:** Improve Core Web Vitals and API latency for booking and SEO pages.

**Scope:** Caching, image/font strategy, query review.

**Dependencies:** Phase 4+ pages exist.

**Acceptance criteria:** Measured improvement on key pages; no SEO regressions (HTML still complete).

**Out of scope:** Premature microservices split.

---

## Phase 14 — Production Deployment

**Objective:** Staging and production environments, HTTPS, backups, logs.

**Scope:** Hosting, PostgreSQL, env config, migrations in deploy, admin restriction.

**Dependencies:** Hardening.

**Acceptance criteria:** Repeatable deploy; secrets in the host; health checks.

**Out of scope:** Kubernetes unless operations explicitly require it.

---

## Phase 15 — SEO Launch & Monitoring

**Objective:** Search Console, analytics, sitemap submission, local SEO consistency, monitoring.

**Scope:** Operational SEO launch, not ranking guarantees.

**Dependencies:** Production public site.

**Acceptance criteria:** Sitemap submitted; monitoring in place; no accidental admin indexing.

**Out of scope:** Buying links or spam tactics.

---

## Future phase — Online Payment

**Objective:** Collect payment only if the client requests it.

**Scope:** TBD (provider, capture timing, refunds). New ADR required.

**Dependencies:** Stable bookings.

**Out of scope for V1:** All payment code, tables, webhooks, UI.

See [ADR-004](../decisions/ADR-004-no-payment-v1.md).
