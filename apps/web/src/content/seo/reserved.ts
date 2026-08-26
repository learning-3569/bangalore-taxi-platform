/** App Router and future product paths that CMS slugs must never collide with. */
export const reservedPublicSlugs = [
  "privacy-policy",
  "terms-and-conditions",
  "book",
  "account",
  "login",
  "api",
  "admin",
  "blog",
  "cars",
  "about",
  "contact",
  "faq",
  "sitemap.xml",
  "robots.txt",
  "airport-taxi-bangalore",
  "outstation-taxi-bangalore",
  "bangalore-taxi",
  "bangalore-cab-booking",
  "bangalore-airport-taxi",
  "one-way-taxi-bangalore",
  "round-trip-taxi-bangalore",
  "corporate-taxi-bangalore",
] as const;

/**
 * When adding a new static App Router folder (about, contact, …), append its
 * first path segment here so `[slug]` cannot generate a colliding SEO page.
 */
export type ReservedPublicSlug = (typeof reservedPublicSlugs)[number];
