# Frontend architecture

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

Phase 0 ships:

- Root layout with `metadataBase`, title template, Open Graph, `lang="en-IN"`
- Homepage foundation copy (not the full marketing site)
- `robots.ts` allowing crawl
- `sitemap.ts` listing the homepage only

Do not create the full SEO route set in Phase 0 (`/bangalore-taxi`, airport pages, city-pair pages, blog). Those belong to Phase 4 and Phase 11.

Suggested later App Router layout (not created yet):

```text
src/app/
  layout.tsx
  page.tsx
  robots.ts
  sitemap.ts
  about/
  contact/
  faq/
  cars/
  blog/
  [seoSlug]/          # CMS-driven landing pages, collision-safe
  account/            # authenticated customer area
  book/               # booking wizard
```

Customer authentication and booking UI are out of scope until Phases 3 and 5.

## Admin portal (`apps/admin`)

- `robots` disallow all user agents.
- Metadata robots: `noindex, nofollow`.
- No public marketing content.
- Future: dashboard, booking desk, customers, drivers, vehicles, pricing, SEO CMS.

Admin auth is not implemented in Phase 0. When it is, sessions must be independent from the public site.

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
