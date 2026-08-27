import { locationPages, reviewOnlyDemoRoute, unpublishedDemoRoute } from "@/content/seo/drafts";
import { locations } from "@/content/seo/locations";
import { reservedPublicSlugs } from "@/content/seo/reserved";
import { airportToWhitefield } from "@/content/seo/routes/bangalore-airport-to-whitefield";
import { bangaloreToCoorg } from "@/content/seo/routes/bangalore-to-coorg";
import { bangaloreToMysore } from "@/content/seo/routes/bangalore-to-mysore";
import { electronicCityToAirport } from "@/content/seo/routes/electronic-city-to-bangalore-airport";
import { koramangalaToAirport } from "@/content/seo/routes/koramangala-to-bangalore-airport";
import { whitefieldToAirport } from "@/content/seo/routes/whitefield-to-bangalore-airport";
import { airportTaxiService, outstationTaxiService } from "@/content/seo/services";
import type {
  LocationContent,
  ParentServiceId,
  RoutePageContent,
  RouteType,
  ServicePageContent,
} from "@/content/seo/types";
import { assertSeoCatalogValid } from "@/content/seo/validate";

export { locationPages, locations };

export const servicePages: readonly ServicePageContent[] = [airportTaxiService, outstationTaxiService];

export const routePages: readonly RoutePageContent[] = [
  whitefieldToAirport,
  airportToWhitefield,
  electronicCityToAirport,
  koramangalaToAirport,
  bangaloreToMysore,
  bangaloreToCoorg,
  unpublishedDemoRoute,
  reviewOnlyDemoRoute,
];

assertSeoCatalogValid({ locations, routes: routePages, services: servicePages, locationPages });

const RELATED_LIMIT = 4;

export function isReservedSlug(slug: string): boolean {
  return (reservedPublicSlugs as readonly string[]).includes(slug);
}

export function getLocation(id: string): LocationContent | undefined {
  return locations.find((item) => item.id === id);
}

export function requireLocation(id: string): LocationContent {
  const location = getLocation(id);
  if (!location) throw new Error(`Unknown location id: ${id}`);
  return location;
}

export function getRouteOrigin(route: RoutePageContent): LocationContent {
  return requireLocation(route.originId);
}

export function getRouteDestination(route: RoutePageContent): LocationContent {
  return requireLocation(route.destinationId);
}

export function getServicePage(id: ParentServiceId): ServicePageContent {
  const page = servicePages.find((item) => item.slug === id);
  if (!page) throw new Error(`Unknown parent service: ${id}`);
  return page;
}

export function getPublishedService(id: ParentServiceId): ServicePageContent | undefined {
  const page = servicePages.find((item) => item.slug === id);
  if (!page?.published) return undefined;
  return page;
}

export function getPublishedServices(): ServicePageContent[] {
  return servicePages.filter((page) => page.published);
}

export function getIndexableServices(): ServicePageContent[] {
  return getPublishedServices().filter((page) => page.indexable);
}

export function getRouteBySlug(slug: string): RoutePageContent | undefined {
  return routePages.find((page) => page.slug === slug);
}

export function getPublishedRoutes(): RoutePageContent[] {
  return routePages.filter((page) => page.published && !isReservedSlug(page.slug));
}

export function getIndexableRoutes(): RoutePageContent[] {
  return getPublishedRoutes().filter((page) => page.indexable);
}

export function getPublishedRoute(slug: string): RoutePageContent | undefined {
  const page = getRouteBySlug(slug);
  if (!page?.published) return undefined;
  return page;
}

export function getRoutesByType(routeType: RouteType): RoutePageContent[] {
  return getPublishedRoutes().filter((page) => page.routeType === routeType);
}

export function getRoutesFromLocation(locationId: string): RoutePageContent[] {
  return getPublishedRoutes().filter((page) => page.originId === locationId);
}

export function getRoutesToLocation(locationId: string): RoutePageContent[] {
  return getPublishedRoutes().filter((page) => page.destinationId === locationId);
}

export function getReverseRoute(route: RoutePageContent): RoutePageContent | undefined {
  return getPublishedRoutes().find(
    (page) => page.originId === route.destinationId && page.destinationId === route.originId && page.slug !== route.slug,
  );
}

export function getRelatedRoutes(page: RoutePageContent): RoutePageContent[] {
  const picked: RoutePageContent[] = [];
  const seen = new Set<string>([page.slug]);

  function add(candidate: RoutePageContent | undefined) {
    if (!candidate || seen.has(candidate.slug) || picked.length >= RELATED_LIMIT) return;
    if (page.indexable && !candidate.indexable) return;
    seen.add(candidate.slug);
    picked.push(candidate);
  }

  for (const slug of page.relatedSlugs) add(getPublishedRoute(slug));
  add(getReverseRoute(page));
  for (const item of getRoutesFromLocation(page.originId)) add(item);
  for (const item of getRoutesToLocation(page.destinationId)) add(item);
  for (const item of getRoutesByType(page.routeType)) add(item);
  return picked;
}

/**
 * Canonical sitemap / indexable URL list: home plus published+indexable pages that the app actually renders.
 * Location catalog records and unpublished location landers are never included.
 */
export function getIndexableRenderedPaths(): string[] {
  return [
    "/",
    ...getIndexableServices().map((page) => `/${page.slug}`),
    ...getIndexableRoutes().map((page) => `/${page.slug}`),
  ];
}

/** Same as getIndexableRenderedPaths. */
export function getIndexableSeoPaths(): string[] {
  return getIndexableRenderedPaths();
}

/** Crawlable HTML the app will generate for published services and routes. */
export function getGeneratedSeoPaths(): string[] {
  return [...getPublishedServices().map((page) => `/${page.slug}`), ...getPublishedRoutes().map((page) => `/${page.slug}`)];
}
