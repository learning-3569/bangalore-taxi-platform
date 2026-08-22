# URL strategy

Stable, readable, lowercase, hyphenated paths. Once a public URL is published, changing it requires a 301 and a sitemap update. Prefer never to change it.

## Host

Production host is decided in Phase 14. Use `NEXT_PUBLIC_SITE_URL` for canonical absolute URLs. No trailing slash (Next.js default).

## Reserved Phase 4/11 routes

Do not implement all of these in Phase 0. When implemented, keep them unique and content-rich:

```text
/
/bangalore-taxi
/bangalore-cab-booking
/airport-taxi-bangalore
/bangalore-airport-taxi
/outstation-taxi-bangalore
/one-way-taxi-bangalore
/round-trip-taxi-bangalore
/corporate-taxi-bangalore
/cars
/about
/contact
/faq
/blog
```

City-pair and area pages (examples):

```text
/taxi-from-bangalore-to-mysore
/taxi-from-bangalore-to-coorg
/bangalore-airport-to-whitefield
/bangalore-airport-to-electronic-city
```

## Collision policy

CMS slugs must not overlap reserved app routes (`/book`, `/account`, `/login`, `/api`). Validate in admin on save. Reserved list lives in backend configuration when the CMS is built.

## Canonicalization

- One preferred URL per intent.
- `www` vs apex decided at deploy; set canonical to the chosen origin.
- Query parameters are not canonical for SEO pages.
- If two marketing URLs would compete (airport-taxi-bangalore vs bangalore-airport-taxi), they must differ in intent and copy, or one must canonical to the other.

## Internal linking

Homepage and relevant landers link to airport, outstation, and city-pair pages. Blog posts link to booking landers, not to thin tag archives.

## Internationalization

English (`en-IN`) for V1. Do not add locale prefixes until there is a real Kannada/Hindi content program.

## Admin

Admin uses a separate origin. It is never listed in the public sitemap.
