# ADR-002: Next.js for the public website

## Status

Accepted (Phase 0)

## Context

Organic Google search is the primary customer acquisition channel. The public site must be crawlable, indexable, fast on mobile, and capable of stable URLs, metadata, sitemaps, and structured data.

A purely client-rendered SPA often delivers empty or weak HTML to crawlers and is harder to keep fast on low-end phones.

## Decision

Use Next.js (App Router) with TypeScript and Tailwind CSS for the public website. Use SSR and static generation where they fit. Use a **separate** Next.js app for the admin portal so internal UI is not mixed into the public information architecture.

This decision improves the *technical* SEO foundation. It does not guarantee search ranking.

## Consequences

- Metadata, canonical URLs, `robots.txt`, and sitemaps are first-class.
- SEO landing pages can be statically generated or ISR'd from a future CMS.
- Two Node applications to deploy (web and admin).
- Developers must not turn marketing pages into client-only islands.

## Alternatives considered

- Create React App / Vite SPA: weaker default SEO story.
- ASP.NET Razor for the public site: possible, but Next.js is specified and fits the SEO + React admin skill mix.
- A single Next.js app with `/admin`: rejected; risk of indexing leaks and coupled releases.
