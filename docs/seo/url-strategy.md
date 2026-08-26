# URL strategy

Stable, readable, lowercase, hyphenated paths. Once a public URL is published, changing it requires a 301 and a sitemap update. Prefer never to change it.

## Host

Production host is decided in Phase 14. Use `NEXT_PUBLIC_SITE_URL` for canonical absolute URLs. No trailing slash (Next.js default).

## Live public paths

```text
/
/privacy-policy
/terms-and-conditions
/airport-taxi-bangalore
/outstation-taxi-bangalore
/whitefield-to-bangalore-airport-taxi
/bangalore-airport-to-whitefield-taxi
/electronic-city-to-bangalore-airport-taxi
/koramangala-to-bangalore-airport-taxi
/bangalore-to-mysore-taxi
/bangalore-to-coorg-taxi
```

Homepage sections use fragment identifiers (`/#book`, `/#services`, `/#airport`, `/#outstation`, `/#fleet`, `/#about`, `/#contact`, `/#faq`). Fragments are not separate sitemap entries.

Editorial convention: customer copy may say **Bangalore** and **Bangalore Airport**; the brand is **Bengaluru Cabs**; the airport’s full name **Kempegowda International Airport** is used where it helps, not in every sentence.

## Route URL convention

Indexable route landers are lowercase, hyphenated, and descriptive:

```text
/{origin}-to-{destination}-taxi
```

Examples: `/whitefield-to-bangalore-airport-taxi`, `/bangalore-to-mysore-taxi`.

Do **not** use query-string SEO pages (`/route?from=whitefield&to=airport`).

Older example paths such as `/taxi-from-bangalore-to-mysore` are **not** published aliases. If they are ever needed, add a 301 to the hyphenated canonical.

## Reserved routes (not implemented yet)

Implement only with unique, useful content. These slugs are listed in `reservedPublicSlugs` so `[slug]` cannot claim them:

```text
/bangalore-taxi
/bangalore-cab-booking
/bangalore-airport-taxi
/one-way-taxi-bangalore
/round-trip-taxi-bangalore
/corporate-taxi-bangalore
/cars
/about
/contact
/faq
/blog
```

When adding a new static App Router folder, append its first segment to `reservedPublicSlugs` and add a test.

`/airport-taxi-bangalore` and `/outstation-taxi-bangalore` are **live** parent pages, not reserved-empty.

City-pair and locality pages (create individually, never as a bulk scrape). Location records for HSR, Marathahalli, Ooty, Chennai, etc. do **not** create URLs:

```text
/bangalore-to-chennai-taxi
/bangalore-to-ooty-taxi
/hsr-layout-to-bangalore-airport-taxi
/taxi-service-whitefield
/taxi-service-electronic-city
```

If both `/airport-taxi-bangalore` and `/bangalore-airport-taxi` are ever published, they must differ in intent or one must canonical to the other.

## Collision policy

CMS slugs must not overlap reserved app routes (`/book`, `/account`, `/login`, `/api`, parent service URLs, legal). Frontend validation is `validateSeoCatalog`. Repeat the same list in admin when the CMS is built.

## Canonicalization

- One preferred URL per intent.
- `www` vs apex decided at deploy; set canonical to the chosen origin.
- Query parameters are not canonical for SEO pages.

## Internal linking

Homepage sections remain hash links except Airport Taxi and Outstation, which point at the parent service pages. Route landers are real hrefs. Do not link unpublished or noindex fixtures from the homepage.

## Internationalization

English (`en-IN`) for V1. Do not add locale prefixes until there is a real Kannada/Hindi content program.

## Admin

Admin uses a separate origin. It is never listed in the public sitemap.
