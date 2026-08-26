# Public website (`apps/web`)

SEO-first Next.js App Router site for Bangalore Taxi.

```bash
npm install
npm run dev    # http://127.0.0.1:43121
npm test
npm run lint
npm run build
```

Set `NEXT_PUBLIC_SITE_URL` for canonical URLs and the sitemap origin. Do not hardcode the production domain until Phase 14.

The homepage booking form is UI-only. Route landers (`/[slug]`) are static, code-managed SEO pages. Do not add admin screens, auth, or payment here. UI notes: [docs/architecture/public-website-ui.md](../../docs/architecture/public-website-ui.md).
