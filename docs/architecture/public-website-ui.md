# Public website UI

Phase 3 established the customer-facing Next.js site (`apps/web`) as a mobile-first taxi company homepage with SEO foundations. Phase 4 added a reusable route landing template and six demonstration SEO pages. Authentication, booking APIs, Maps, payment, and the admin portal remain out of scope.

## Design principles

- Trustworthy local operator, not a SaaS dashboard template.
- Usability over decoration: light hover/focus, no animation libraries, no stock photography.
- One visual system in `src/app/globals.css` CSS variables mapped into Tailwind `@theme`.
- Tokens can be swapped when real brand assets arrive; do not scatter hex colours in components.

## Temporary brand tokens

| Token | Value | Role |
| --- | --- | --- |
| `--brand` | `#1e3a4c` | Primary (header, primary buttons, footer) |
| `--accent` | `#b45309` | Booking CTAs (taxi amber, used sparingly) |
| `--paper` / `--paper-raised` | `#f4f1ea` / `#fffcf7` | Page and card backgrounds |
| `--ink` / `--ink-muted` | `#1c1917` / `#57534e` | Text |
| `--line` | `#e4ddd2` | Borders |

Typography: **Source Serif 4** for headings (`font-serif`), **Source Sans 3** for UI and body (`font-sans`). Heading scale is implemented via `SectionHeading` and page `h1` classes (`text-4xl`/`text-5xl` hero, `text-2xl`/`text-3xl` sections). Body is `text-base`/`text-sm` with relaxed line height.

Buttons: primary (brand), secondary (outlined), accent (book). Cards: 12px radius, light border, faint shadow. Form fields: labelled, 8px radius, brand focus ring. Spacing: container `max-w-6xl` with `px-4 sm:px-6 lg:px-8`; section padding `py-14 sm:py-20`.

## Components

Business-oriented pieces live under `src/components/`:

| Component | Purpose |
| --- | --- |
| `layout/Header` | Desktop nav, mobile drawer, Book CTA |
| `layout/Footer` | Services, contact placeholders, legal |
| `ui/Button`, `Container`, `Card`, `SectionHeading`, `Breadcrumbs` | Shared chrome |
| `ui/Fields` | Text, select, date/time via native inputs |
| `booking/BookingForm` | UI-only request form (no API); optional route prefill |
| `routes/RouteLandingPage` | Server-rendered SEO route template |
| `content/ServiceCard`, `VehicleCard`, `FaqItem`, `Testimonial` | Homepage content blocks |
| `seo/JsonLd` | JSON-LD helpers |
| `legal/LegalPlaceholder` | Non-binding legal URL stubs |

## Responsive strategy

Layouts are written mobile-first (`grid` stacking, then `sm:`/`md:`/`lg:`). Primary nav is a simple list from `lg` up; below that a button opens a full-width menu with Escape, return-focus, `aria-expanded`, and body scroll lock. Target widths: 320–414 phones, 768 tablet, 1024+ desktop.

## Accessibility

Semantic landmarks (`header`, `nav`, `main`, `footer`), skip link, one `h1` on the homepage, labelled fields, visible `:focus-visible`, FAQ via native `details`/`summary`, decorative SVGs `aria-hidden` with a short `aria-label` on the placeholder graphic.

## SEO principles

See [SEO architecture](../seo/seo-architecture.md). Homepage copy targets Bangalore taxi / airport / outstation intent without stuffing. Internal links only point at URLs that exist (`/` sections, legal pages, published route slugs).

## Performance principles

Default to Server Components. Client islands: `Header` (menu), `BookingWidget`, route sticky CTA, and `HeroCarousel`. No Maps, no video, no icon packs. Prefer static HTML for crawlers. Route pages are generated at build time from the published catalog.

## Page hierarchy (intended)

```text
Homepage (/)
  ├── Service landers: /airport-taxi-bangalore, /outstation-taxi-bangalore
  ├── Route landers (explicit catalog entries only)
  ├── Legal: /privacy-policy, /terms-and-conditions
  └── Later (unique content only):
        /airport-taxi-bangalore
        /outstation-taxi-bangalore
        other reserved service URLs
            └── More route/location pages (not generated in bulk)
                └── Blog / supporting articles
```

Login / My bookings is mentioned in the footer as a future slot only.
