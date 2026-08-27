import { parentServiceSlugs, reservedPublicSlugs } from "@/content/seo/reserved";
import type {
  LocationContent,
  LocationPageContent,
  RoutePageContent,
  ServicePageContent,
} from "@/content/seo/types";

const reserved = reservedPublicSlugs as readonly string[];
const allowedServiceReserved = parentServiceSlugs as readonly string[];

export function validateSeoCatalog({
  locations,
  routes,
  services,
  locationPages = [],
}: {
  locations: readonly LocationContent[];
  routes: readonly RoutePageContent[];
  services: readonly ServicePageContent[];
  locationPages?: readonly LocationPageContent[];
}): string[] {
  const errors: string[] = [];
  const locationIds = new Map<string, LocationContent>();
  for (const location of locations) {
    if (locationIds.has(location.id)) errors.push(`Duplicate location id: ${location.id}`);
    locationIds.set(location.id, location);
  }

  const slugs = new Map<string, string>();
  function claimSlug(slug: string, kind: string) {
    if (reserved.includes(slug)) {
      if (kind === "route") {
        errors.push(`Route slug collides with reserved path: ${slug}`);
      } else if (kind === "location-page") {
        errors.push(`Location page slug collides with reserved path: ${slug}`);
      } else if (kind === "service" && !allowedServiceReserved.includes(slug)) {
        errors.push(`Service slug collides with reserved path: ${slug}`);
      }
    }
    const owner = slugs.get(slug);
    if (owner) errors.push(`Duplicate slug "${slug}" (${owner} vs ${kind})`);
    else slugs.set(slug, kind);
  }

  for (const service of services) {
    claimSlug(service.slug, "service");
    if (service.indexable && !service.published) {
      errors.push(`${service.slug}: indexable requires published`);
    }
  }

  for (const page of locationPages) {
    claimSlug(page.slug, "location-page");
    if (!locationIds.has(page.localityId)) {
      errors.push(`${page.slug}: unknown locality "${page.localityId}"`);
    }
    if (page.indexable && !page.published) {
      errors.push(`${page.slug}: indexable requires published`);
    }
    if (page.published) {
      errors.push(`${page.slug}: location landers are not generated; keep unpublished`);
    }
  }

  for (const route of routes) {
    claimSlug(route.slug, "route");
    if (!locationIds.has(route.originId)) errors.push(`${route.slug}: unknown origin "${route.originId}"`);
    if (!locationIds.has(route.destinationId)) errors.push(`${route.slug}: unknown destination "${route.destinationId}"`);
    if (route.originId === route.destinationId) errors.push(`${route.slug}: origin and destination are the same`);
    if (route.indexable && !route.published) errors.push(`${route.slug}: indexable requires published`);
    if (route.indexable) {
      if (!route.seoTitle.trim()) errors.push(`${route.slug}: missing seoTitle`);
      if (!route.metaDescription.trim()) errors.push(`${route.slug}: missing metaDescription`);
      if (!route.h1.trim()) errors.push(`${route.slug}: missing h1`);
    }
    for (const related of route.relatedSlugs) {
      if (related === route.slug) errors.push(`${route.slug}: related slug points at itself`);
      if (!routes.some((item) => item.slug === related)) errors.push(`${route.slug}: related slug does not exist: ${related}`);
    }
  }

  return errors;
}

export function assertSeoCatalogValid(input: {
  locations: readonly LocationContent[];
  routes: readonly RoutePageContent[];
  services: readonly ServicePageContent[];
  locationPages?: readonly LocationPageContent[];
}) {
  const errors = validateSeoCatalog(input);
  if (errors.length > 0) {
    throw new Error(`SEO catalog invalid:\n- ${errors.join("\n- ")}`);
  }
}
