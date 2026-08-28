# Frontend architecture

## Phase 8 assignment UI

Assignment operations live only in `apps/admin`, on the accepted booking detail page. Its BFF proxies bounded eligible-driver and exact-type vehicle queries through the established admin session flow; the API still enforces authorization and every domain rule. The customer app only renders the persisted `driver_assigned` status, driver display name, and vehicle type/registration. It never receives driver phone/account data, audit data, or admin controls.

The authenticated admin shell exposes only implemented sections: Bookings, Drivers, and Vehicles. `/drivers` and `/vehicles` provide bounded searchable lists, create forms, editable details, reversible deactivation, and temporal roster tagging. All calls use the existing admin BFF/auth provider; navigation is not an authorization boundary. No fleet management is present in `apps/web`.

## Applications

Two Next.js 15 App Router applications:

| Path | Audience | SEO |
| --- | --- | --- |
| `apps/web` | Customers and search engines | Primary product surface |
| `apps/admin` | Staff | Must not be indexed |

They must stay separate. Shared design tokens may be copied sparingly; do not create a premature monorepo package for UI until duplication actually hurts.

Stack: TypeScript, Tailwind CSS, App Router. No second component library in Phase 0. When UI primitives are needed, prefer shadcn/ui in the app that needs them rather than a third library.

## Public website (`apps/web`)

SEO is a core architectural requirement. See [SEO architecture](../seo/seo-architecture.md) and [URL strategy](../seo/url-strategy.md).

Rendering:

- Marketing and SEO landing pages: static generation or ISR where content is CMS-driven.
- Pages that need request-time personalization: server rendering.
- Customer account and booking flows: server components plus authenticated client islands where necessary.

Do not default to a purely client-rendered SPA. Search-friendly HTML must be present in the first response.

Phase 3 ships the marketing homepage, header/footer, UI-only booking widget, legal URL placeholders, metadata helpers, JSON-LD (without fake NAP/ratings), `robots.ts`, and a sitemap of real public paths only. See [public website UI](public-website-ui.md).

Phase 4 adds `src/app/[slug]/page.tsx` for published route landers (`generateStaticParams`, `dynamicParams = false`). Content lives in `src/content/seo/`. Do not mass-generate locality pages.

Suggested later App Router layout:

```text
src/app/
  layout.tsx
  page.tsx
  robots.ts
  sitemap.ts
  [slug]/             # published SEO landers (routes now; CMS later)
  privacy-policy/
  terms-and-conditions/
  about/              # reserved
  contact/
  faq/
  cars/
  blog/
  account/            # authenticated customer area
  book/               # booking wizard
```

Phase 5 provides OTP login through HttpOnly refresh cookies in the Next.js BFF. Phase 6 makes the booking widget persist through the authenticated booking API, preserves every meaningful form field through OTP, and adds noindex `/account/bookings` list/detail pages. A success message is rendered only from a successful persisted response.

## Admin portal (`apps/admin`)

- `robots` disallow all user agents.
- Metadata robots: `noindex, nofollow`.
- No public marketing content.
- Phase 7: `/login`, `/bookings`, and `/bookings/[id]` provide the internal booking queue and accept/reject workflow.
- The admin app reuses phone OTP plus rotating refresh sessions through its own BFF. Refresh tokens remain HttpOnly; refresh/logout require the CSRF cookie/header pair.
- The UI checks the returned role for safe routing and navigation, while `apps/api` remains the authorization boundary for every admin read/write.

Admin sign-in never accepts a requested role. Existing users receive roles from persisted identity, and a non-admin session receives an access-denied UI while the API returns 403 for operations.

## Data access

Frontends call `apps/api` over HTTPS JSON. No business pricing logic in the browser. No authorization decisions that the API does not re-check.

Environment variables (names only):

- `NEXT_PUBLIC_SITE_URL`
- `NEXT_PUBLIC_API_BASE_URL`
- `NEXT_PUBLIC_ADMIN_URL` (admin app)

## Local ports

- Web: `43121`
- Admin: `43122`

## Rules for future agents

- Preserve published public URLs.
- Consider crawl impact before renaming routes.
- Do not implement later-phase screens “while you are here”.
- Keep admin routes out of the public sitemap.
