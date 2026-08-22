# SEO architecture

SEO is a core product requirement. The business depends on Google organic discovery in Bangalore. This stack can provide crawlability, indexability, performance, and structured HTML. It cannot guarantee ranking or “position one”.

## Public vs admin

| App | Crawlers |
| --- | --- |
| `apps/web` | Allow; sitemap; indexable metadata |
| `apps/admin` | Disallow all; `noindex` |

## Technical foundation (in place or planned)

| Capability | Phase 0 | Later |
| --- | --- | --- |
| Semantic HTML, one H1 per page | Homepage | All pages |
| Mobile-first layout | Homepage | All pages |
| SSR / SSG via App Router | Yes | SEO pages SSG/ISR |
| Metadata API (title, description) | Yes | Per page + CMS |
| Canonical URLs | Homepage | Every public URL |
| `robots.ts` | Allow `/` | Keep admin blocked |
| `sitemap.ts` | Homepage only | All published URLs |
| Structured data (LocalBusiness, FAQ, TaxiService) | No | Phase 4 / 11 |
| XML sitemap of CMS pages | No | Phase 11 |
| Internal linking | No | Phase 4 / 11 |
| Performance / Core Web Vitals | Next defaults | Phase 13 |
| Local SEO (NAP consistency, geo metadata) | Copy only | Phase 15 |

## CMS-driven SEO pages (Phase 11)

Administrators will create and edit pages without code changes. Conceptual fields:

- Slug
- Title
- Meta description
- H1
- Content
- Canonical URL
- Featured image
- FAQ items
- Structured data type
- Published status
- Created / updated timestamps

The public site resolves a published slug to a generated page. Unpublished pages return 404. Canonical should default to the public absolute URL of the slug unless an editor overrides it for consolidating duplicates.

Do not generate thin or duplicate doorway pages. Each URL needs distinct intent and useful content (airport pickup vs outstation vs a specific city pair).

## Rendering

- Published landing pages: static generation or ISR so crawlers receive full HTML.
- Revalidate when an admin publishes.
- Do not hide primary copy behind client-only fetch.

## Structured data (future)

JSON-LD in the page, driven from CMS fields, not hand-copied per route. Types likely: `LocalBusiness` / `TaxiService`, `FAQPage`, `BreadcrumbList`. Validate with Google’s rich results tools in Phase 15.

## Performance

Fast TTFB, compressed assets, image `next/image`, minimal client JS on marketing pages. Performance work is Phase 13; do not add heavy client libraries to SEO pages.

## What not to do

- Do not ship dozens of near-duplicate templates in Phase 0.
- Do not put booking widgets that block HTML behind a JS-only wall.
- Do not promise search rankings in UI copy or documentation.
- Do not index the admin portal, APIs, or account pages that leak personal data.
