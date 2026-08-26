# SEO architecture

SEO is a core product requirement. The business depends on Google organic discovery in Bangalore. This stack can provide crawlability, indexability, performance, and structured HTML. It cannot guarantee ranking or “position one”.

## Public vs admin

| App | Crawlers |
| --- | --- |
| `apps/web` | Allow; sitemap; indexable metadata |
| `apps/admin` | Disallow all; `noindex` |

## Homepage (Phase 3)

The public homepage remains a primary landing URL. It uses:

- One H1 per page (homepage carousel heading; route pages have their own H1)
- Title, meta description, canonical, Open Graph, and Twitter summary via `createPageMetadata` (`src/lib/seo.ts`)
- `metadataBase` from `NEXT_PUBLIC_SITE_URL` (fallback `http://127.0.0.1:43121` — not a production domain)
- FAQ copy in HTML plus `FAQPage` JSON-LD that matches that copy
- `WebSite`, `Organization`, and `TaxiService` JSON-LD **without** telephone, street address, opening hours, aggregate ratings, or review stars

Local/dev hosts are **not** advertised as indexable: `isPublicIndexable()` sets `robots` to disallow `/` and page metadata to `noindex` when the configured origin is `localhost` or `127.0.0.1`. Production indexing requires a real `NEXT_PUBLIC_SITE_URL`.

Do not add AggregateRating or Review schema until real, permissioned reviews exist.

## Technical foundation

| Capability | Status |
| --- | --- |
| Semantic HTML, one H1 per page | Homepage, legal, published route landers |
| Mobile-first layout | Homepage + route template |
| SSR / SSG via App Router | Yes; route slugs via `generateStaticParams` |
| Metadata API | Central helper; per-page overrides |
| Canonical URLs | Relative paths; origin from env; each route self-canonicalizes |
| `robots.ts` | Allow `/` only when `isPublicIndexable()`; sitemap URL |
| `sitemap.ts` | Home, legal, indexable services, indexable routes |
| Structured data | Conservative JSON-LD helpers; CMS later |
| Internal linking | Homepage hashes, legal URLs, published routes only |
| Performance | Server Components; small client islands (header, booking, sticky CTA) |
| Local SEO (NAP) | Placeholders only until the business confirms details |

Test-only or private routes: unpublished slugs 404. `published && !indexable` may SSG with `noindex` and stay out of the sitemap (`review-only-demo-route` fixture).

## Service, route, and location pages

### Taxonomy

| Category | Examples | Status |
| --- | --- | --- |
| A — Core service | `/airport-taxi-bangalore`, `/outstation-taxi-bangalore` | Published parent landers (Phase 4B) |
| B — Airport outbound | `/whitefield-to-bangalore-airport-taxi` | Six indexable demonstration routes |
| C — Airport inbound | `/bangalore-airport-to-whitefield-taxi` | Published where inbound intent differs |
| D — Outstation | `/bangalore-to-mysore-taxi` | Mysore and Coorg only |
| E — Locality service | `/taxi-service-whitefield` | `LocationPageContent` only. **No pages.** Location catalog records are not URLs. |

### Location model

`LocationContent` in `apps/web/src/content/seo/locations.ts`: id, name, slug, optional alternateName, type (city / locality / airport / outstation / landmark), city, state, country, optional airportCode and coordinates, published.

Locations can be referenced by many future routes. **They do not generate pages.** Do not combinatorially pair every origin with every destination.

### Route model

`RoutePageContent` stores `originId` and `destinationId` into that catalog, plus unique editorial fields (H1, intro, pickup/destination/guidance, FAQ, etc.). Curated `relatedSlugs` are preferred; `getRelatedRoutes` may add reverse / same-origin / same-destination / same-type **published + indexable** routes only.

Flags:

| published | indexable | Effect |
| --- | --- | --- |
| false | — | Not generated, not in sitemap |
| true | false | Generated, `noindex`, omitted from sitemap |
| true | true | SSG, indexable (when the site origin is production), sitemap |

### Route registry

`apps/web/src/content/seo/catalog.ts` is the frontend catalog API: `getRouteBySlug`, `getPublishedRoutes`, `getIndexableRoutes`, `getRoutesByType`, `getRoutesFromLocation`, `getRoutesToLocation`, `getRelatedRoutes`, `getReverseRoute`, plus location and parent-service helpers.

`validateSeoCatalog` runs at module load (duplicate slugs, unknown locations, reserved collisions, missing indexable metadata, bad related slugs, self-routes).

### Route creation workflow

1. Add or reuse a **location** record (no page).
2. Author a **route** object with unique copy. Never auto-build from two location lists.
3. Set `published` / `indexable` deliberately.
4. Link from parent service / homepage only after the URL exists.
5. Later: same objects from SEO CMS instead of TypeScript modules.

### CMS migration seam

`RouteLandingPage` and `ServiceLandingPage` should keep consuming `RoutePageContent` / `ServicePageContent`. A future ASP.NET + PostgreSQL CMS replaces the TypeScript arrays behind `catalog.ts` (fetch published rows, map into these types). Do not fork a second public renderer.

### Scale strategy

Priority corridors stay SSG (`generateStaticParams` from the catalog). At hundreds or thousands of routes, do not assume every URL is prerendered forever: ISR / on-demand revalidation on CMS publish, or server render long-tail slugs. Sitemap must still list only `published && indexable`. No origin×destination product.

A later SEO CMS (roadmap Phase 11) should map to this shape rather than inventing a second public renderer.

When a lander ships:

- Unique title, H1, and body
- Canonical to itself unless consolidating a duplicate
- It appears via `getIndexableSeoPaths()` so the sitemap stays truthful
- Link it from the homepage only when the page exists

## Internal linking

Intended hierarchy:

```text
Homepage
  → /airport-taxi-bangalore and /outstation-taxi-bangalore
    → Explicit route landers (unique copy only)
      → Supporting content / blog (later)
```

Do not build footer or body link farms. Popular routes on the homepage are **links only when a unique page exists**; otherwise they remain labels.

## Metadata and canonical URLs

`getSiteUrl()` must not hardcode the eventual production host. Phase 14 chooses the domain. Every public page should call `createPageMetadata` with its path so canonical and Open Graph `url` stay aligned.

## Sitemap and robots

- Sitemap lists home, legal, **indexable** services, and **indexable** routes. Drafts and `noindex` review URLs must not appear. Location catalog rows are not URLs.
- Architecture: later map CMS `published` slugs into `getIndexableSeoPaths()`.
- `robots.txt` allows crawlers on a public production origin. Localhost is disallowed. Admin remains a separate app with its own disallow-all robots file.

## Structured data

Helpers live in `src/components/seo/JsonLd.tsx`.

| Type | When to emit | Do not |
| --- | --- | --- |
| `WebSite` | Sitewide | Invent a SearchAction that does not work |
| `Organization` | Sitewide | Fake logo URL until assets exist |
| `TaxiService` | Homepage / service landers | Phone, address, geo, ratings until confirmed |
| `FAQPage` | When FAQ HTML is on the page | Answers that contradict live policy |
| `BreadcrumbList` | Nested pages (legal; Home → service → route) | Fake parents |
| `Service` | Dedicated route/service landers | Offers, prices, availability |

### Business details still required for LocalBusiness completeness

Telephone, email, postal address, geo coordinates, opening hours, price range, and a real logo URL. Until those are provided, keep NAP out of JSON-LD rather than inventing it.

## Content strategy

Write for travellers first. Mention Bangalore taxi, cab booking, airport taxi, and outstation naturally. Avoid repeating the same phrase in every heading. Expand with dedicated pages only when there is something new to say (meeting point at BLR, typical outstation duration, vehicle fit).

## What not to do

- Do not ship dozens of near-duplicate templates.
- Do not hide primary copy behind a JS-only wall (the booking widget is a small client island; the rest is server-rendered).
- Do not promise search rankings in UI copy or documentation.
- Do not index the admin portal, APIs, or account pages that leak personal data.
