import { describe, expect, it } from "vitest";
import { render, screen } from "@testing-library/react";
import { generateStaticParams } from "@/app/[slug]/page";
import sitemap from "@/app/sitemap";
import { AuthProvider } from "@/components/auth/AuthProvider";
import { RouteLandingPage } from "@/components/routes/RouteLandingPage";
import { ServiceLandingPage } from "@/components/routes/ServiceLandingPage";
import { breadcrumbJsonLd, faqJsonLd, serviceJsonLd } from "@/components/seo/JsonLd";
import { legalPagesArePlaceholders, popularRoutes } from "@/config/site";
import {
  getIndexableRenderedPaths,
  getIndexableRoutes,
  getPublishedRoute,
  getPublishedRoutes,
  getPublishedService,
  getRelatedRoutes,
  getReverseRoute,
  getRouteBySlug,
  getRoutesFromLocation,
  getRoutesToLocation,
  getServicePage,
  isReservedSlug,
  locationPages,
  locations,
  routePages,
  servicePages,
} from "@/content/seo/catalog";
import type { ServicePageContent } from "@/content/seo/types";
import { validateSeoCatalog } from "@/content/seo/validate";
import { isImplementedPublicPath } from "@/lib/paths";
import { getSitemapPaths } from "@/lib/public-paths";
import { createPageMetadata } from "@/lib/seo";

describe("route catalog", () => {
  it("publishes demonstration routes and keeps drafts unpublished", () => {
    expect(getIndexableRoutes()).toHaveLength(33);
    expect(getPublishedRoutes().some((page) => page.slug === "review-only-demo-route")).toBe(false);
    expect(getIndexableRoutes().some((page) => page.slug === "review-only-demo-route")).toBe(false);
    expect(getPublishedRoute("unpublished-demo-route")).toBeUndefined();
    expect(getPublishedRoute("review-only-demo-route")).toBeUndefined();
    expect(getRouteBySlug("unpublished-demo-route")?.published).toBe(false);
    expect(getRouteBySlug("review-only-demo-route")?.published).toBe(false);
    expect(getRouteBySlug("review-only-demo-route")?.indexable).toBe(false);
  });

  it("publishes exactly one canonical outbound route for all thirty curated airport localities", () => {
    const slugs = [
      "whitefield", "itpl", "hoodi", "kadugodi", "electronic-city", "hsr-layout",
      "singasandra", "bommanahalli", "bellandur", "sarjapur-road", "haralur",
      "kasavanahalli", "outer-ring-road", "marathahalli", "indiranagar", "koramangala",
      "jp-nagar", "jayanagar", "banashankari", "rajajinagar", "malleshwaram",
      "yeshwanthpur", "hebbal", "yelahanka", "manyata-tech-park", "kr-puram",
      "mahadevapura", "btm-layout", "mg-road", "sunkadakatte",
    ].map((locality) => `${locality}-to-bangalore-airport-taxi`);
    expect(slugs.map(getPublishedRoute).every(Boolean)).toBe(true);
    expect(slugs.every((slug) => getPublishedRoute(slug)?.direction === "to-airport")).toBe(true);
    expect(new Set(routePages.map((route) => route.slug)).size).toBe(routePages.length);
    expect(getPublishedRoute("bangalore-airport-to-whitefield-taxi")?.direction).toBe("from-airport");
    expect(getPublishedRoute("bangalore-airport-to-electronic-city-taxi")?.direction).toBe("from-airport");
  });

  it("keeps editorial aliases and target queries off the URL surface", () => {
    expect(locations.find((item) => item.id === "hsr-layout")?.aliases).toContain("HSR");
    expect(locations.find((item) => item.id === "manyata-tech-park")?.aliases).toContain("Manyata Embassy Business Park");
    expect(getRouteBySlug("hsr-to-airport-cab")).toBeUndefined();
    expect(getRouteBySlug("manyata-embassy-business-park-to-airport-taxi")).toBeUndefined();
    expect(getRouteBySlug("hoody-to-bangalore-airport-taxi")).toBeUndefined();
    expect(getRouteBySlug("btm-to-bangalore-airport-cab")).toBeUndefined();
    expect(getRouteBySlug("krishnarajapuram-to-airport-taxi")).toBeUndefined();
    expect(getRouteBySlug("mg-road-to-bangalore-airport-cab")).toBeUndefined();
    const route = getPublishedRoute("bangalore-airport-to-electronic-city-taxi")!;
    const { container } = render(<AuthProvider><RouteLandingPage route={route} /></AuthProvider>);
    expect(container).not.toHaveTextContent("airport to e-city cab");
  });

  it("keeps auditor language out of expanded route FAQs and ORR out of the index", () => {
    const expanded = [
      "itpl", "hoodi", "kadugodi", "singasandra", "bommanahalli", "haralur",
      "kasavanahalli", "outer-ring-road", "indiranagar", "jp-nagar", "jayanagar",
      "banashankari", "rajajinagar", "malleshwaram", "yeshwanthpur", "kr-puram",
      "mahadevapura", "btm-layout", "mg-road", "sunkadakatte",
    ].map((locality) => getPublishedRoute(`${locality}-to-bangalore-airport-taxi`)!);
    const visibleFaqs = expanded.flatMap((route) => route.faq.flatMap((item) => [item.question, item.answer])).join(" ");

    expect(expanded.every((route) => route.faq.length >= 2)).toBe(true);
    expect(visibleFaqs).not.toMatch(/canonical|duplicate URL|generated from a URL|route pages?/i);
    expect(getPublishedRoute("outer-ring-road-to-bangalore-airport-taxi")?.indexable).toBe(false);
  });

  it("does not generate the review fixture in production static params", () => {
    const slugs = generateStaticParams().map((entry) => entry.slug);
    expect(slugs).not.toContain("review-only-demo-route");
    expect(slugs).not.toContain("unpublished-demo-route");
    expect(isImplementedPublicPath("/review-only-demo-route")).toBe(false);
  });

  it("only links related routes that are published and indexable from customer pages", () => {
    for (const page of getIndexableRoutes()) {
      const related = getRelatedRoutes(page);
      expect(related.length).toBeGreaterThan(0);
      for (const item of related) {
        expect(item.published).toBe(true);
        expect(item.indexable).toBe(true);
        expect(isImplementedPublicPath(`/${item.slug}`)).toBe(true);
      }
    }
  });

  it("resolves a reverse route when both directions are published", () => {
    const outbound = getPublishedRoute("whitefield-to-bangalore-airport-taxi")!;
    const reverse = getReverseRoute(outbound);
    expect(reverse?.slug).toBe("bangalore-airport-to-whitefield-taxi");
    expect(getRoutesFromLocation("whitefield").some((page) => page.slug === outbound.slug)).toBe(true);
    expect(getRoutesToLocation("blr-airport").length).toBeGreaterThan(0);
  });

  it("gives every indexable page a unique H1 and metadata pair", () => {
    const h1s = getIndexableRoutes().map((page) => page.h1);
    const titles = getIndexableRoutes().map((page) => page.seoTitle);
    expect(new Set(h1s).size).toBe(h1s.length);
    expect(new Set(titles).size).toBe(titles.length);
  });

  it("keeps every curated airport route directional with the canonical BLR value", () => {
    const airportRoutes = getIndexableRoutes().filter((page) => page.routeType === "airport");
    expect(airportRoutes).toHaveLength(31);
    for (const page of airportRoutes) {
      const outbound = page.direction === "to-airport";
      expect(page.summary.from).toBe(outbound ? locations.find((location) => location.id === page.originId)?.name : "Kempegowda International Airport (BLR)");
      expect(page.summary.to).toBe(outbound ? "Kempegowda International Airport (BLR)" : locations.find((location) => location.id === page.destinationId)?.name);
    }
  });

  it("does not invent fares or numeric travel times in the summary", () => {
    for (const page of getIndexableRoutes()) {
      expect(page.farePlaceholder.toLowerCase()).toContain("request");
      expect(page.summary.durationNote.toLowerCase()).toMatch(/traffic|condition/);
      expect(page.summary.distanceNote).not.toMatch(/\d+\s*km/i);
      expect(page.summary.durationNote).not.toMatch(/\d+\s*(min|hour|hr)/i);
    }
  });
});

describe("catalog validation", () => {
  it("accepts the live catalog", () => {
    expect(validateSeoCatalog({ locations, routes: routePages, services: servicePages, locationPages })).toEqual([]);
  });

  it("detects duplicate slugs, unknown locations, and reserved collisions", () => {
    const sample = getIndexableRoutes()[0];
    const duplicate = validateSeoCatalog({
      locations,
      services: servicePages,
      routes: [sample, { ...sample }],
    });
    expect(duplicate.some((error) => error.includes("Duplicate slug"))).toBe(true);

    const unknown = validateSeoCatalog({
      locations,
      services: servicePages,
      routes: [{ ...sample, originId: "not-a-place", slug: "unique-unknown-origin" }],
    });
    expect(unknown.some((error) => error.includes("unknown origin"))).toBe(true);

    const reserved = validateSeoCatalog({
      locations,
      services: servicePages,
      routes: [{ ...sample, slug: "airport-taxi-bangalore" }],
    });
    expect(reserved.some((error) => error.includes("reserved"))).toBe(true);

    const serviceIndexable = validateSeoCatalog({
      locations,
      routes: routePages,
      services: [{ ...servicePages[0], published: false, indexable: true, slug: "airport-taxi-bangalore" }],
    });
    expect(serviceIndexable.some((error) => error.includes("indexable requires published"))).toBe(true);

    const reservedService = validateSeoCatalog({
      locations,
      routes: [],
      services: [{ ...servicePages[0], slug: "login" } as ServicePageContent],
    });
    expect(reservedService.some((error) => error.includes("reserved"))).toBe(true);

    const locationPage = validateSeoCatalog({
      locations,
      routes: [],
      services: servicePages,
      locationPages: [
        {
          slug: "taxi-service-whitefield",
          published: true,
          indexable: true,
          lastUpdated: "2026-08-26",
          localityId: "not-a-place",
          seoTitle: "Test",
          metaDescription: "Test",
          h1: "Test",
          intro: "Test",
        },
      ],
    });
    expect(locationPage.some((error) => error.includes("unknown locality"))).toBe(true);
    expect(locationPage.some((error) => error.includes("not generated"))).toBe(true);

    const reservedLocation = validateSeoCatalog({
      locations,
      routes: [],
      services: servicePages,
      locationPages: [
        {
          slug: "privacy-policy",
          published: false,
          indexable: false,
          lastUpdated: "2026-08-26",
          localityId: "whitefield",
          seoTitle: "Test",
          metaDescription: "Test",
          h1: "Test",
          intro: "Test",
        },
      ],
    });
    expect(reservedLocation.some((error) => error.includes("reserved"))).toBe(true);
  });

  it("protects reserved public slugs from the dynamic route table", () => {
    expect(isReservedSlug("airport-taxi-bangalore")).toBe(true);
    expect(isReservedSlug("outstation-taxi-bangalore")).toBe(true);
    expect(isReservedSlug("privacy-policy")).toBe(true);
    expect(getIndexableRoutes().every((page) => !isReservedSlug(page.slug))).toBe(true);
  });
});

describe("route landing pages", () => {
  it("renders unique H1, breadcrumbs, booking prefill, and FAQ for outbound Whitefield", () => {
    const route = getPublishedRoute("whitefield-to-bangalore-airport-taxi");
    expect(route).toBeDefined();
    render(
      <AuthProvider>
        <RouteLandingPage route={route!} />
      </AuthProvider>,
    );
    expect(screen.getByRole("heading", { level: 1, name: route!.h1 })).toBeInTheDocument();
    expect(screen.getByRole("navigation", { name: "Breadcrumb" })).toBeInTheDocument();
    expect(screen.getByRole("link", { name: "Airport taxi" })).toHaveAttribute("href", "/airport-taxi-bangalore");
    expect(screen.getByDisplayValue("Whitefield")).toBeInTheDocument();
    expect(screen.getByDisplayValue("Kempegowda International Airport (BLR)")).toHaveAttribute("readonly");
    expect(screen.getByRole("link", { name: /bangalore airport → whitefield/i })).toBeInTheDocument();
    expect(screen.getByText(/can i book a whitefield to airport taxi in advance/i)).toBeInTheDocument();
    const bookCtas = screen.getAllByRole("link", { name: route!.primaryCtaLabel });
    expect(bookCtas[0]).toHaveAttribute("href", expect.stringContaining("/login?"));
    expect(bookCtas[0]).toHaveAttribute("href", expect.stringContaining("pickup=Whitefield"));
    expect(bookCtas[0]).toHaveAttribute("href", expect.stringContaining("serviceType=airport"));
    expect(bookCtas[0]).toHaveAttribute("href", expect.stringContaining("airportJourneyType=drop"));
    expect(bookCtas[0]).toHaveAttribute("href", expect.stringContaining("whitefield-to-bangalore-airport-taxi"));
  });

  it("renders inbound airport copy that is not a reversed outbound page", () => {
    const outbound = getPublishedRoute("whitefield-to-bangalore-airport-taxi")!;
    const inbound = getPublishedRoute("bangalore-airport-to-whitefield-taxi")!;
    render(
      <AuthProvider>
        <RouteLandingPage route={inbound} />
      </AuthProvider>,
    );
    expect(screen.getByRole("heading", { level: 1, name: inbound.h1 })).toBeInTheDocument();
    expect(inbound.intro).not.toBe(outbound.intro);
    expect(inbound.pickupInformation.body).not.toBe(outbound.pickupInformation.body);
    expect(screen.getByText(/arrivals are a different job/i)).toBeInTheDocument();
    expect(screen.getByDisplayValue("Kempegowda International Airport (BLR)")).toHaveAttribute("readonly");
    const bookCtas = screen.getAllByRole("link", { name: inbound.primaryCtaLabel });
    expect(bookCtas[0]).toHaveAttribute("href", expect.stringContaining("serviceType=airport"));
    expect(bookCtas[0]).toHaveAttribute("href", expect.stringContaining("airportJourneyType=pickup"));
  });

  it("renders an outstation lander with round-trip context", () => {
    const route = getPublishedRoute("bangalore-to-mysore-taxi")!;
    render(
      <AuthProvider>
        <RouteLandingPage route={route} />
      </AuthProvider>,
    );
    expect(screen.getByRole("heading", { level: 1, name: route.h1 })).toBeInTheDocument();
    expect(screen.getByText(/one-way versus round-trip/i)).toBeInTheDocument();
  });
});

describe("parent service pages", () => {
  it("gates parent services on the published flag", () => {
    expect(getPublishedService("airport-taxi-bangalore")?.published).toBe(true);
    expect(getPublishedService("outstation-taxi-bangalore")?.published).toBe(true);
    expect(getIndexableRenderedPaths()).toContain("/airport-taxi-bangalore");
    expect(getIndexableRenderedPaths()).toContain("/outstation-taxi-bangalore");
  });

  it("renders airport and outstation service landers with unique H1s and route links", () => {
    const airport = getServicePage("airport-taxi-bangalore");
    const outstation = getServicePage("outstation-taxi-bangalore");
    const { unmount } = render(
      <AuthProvider>
        <ServiceLandingPage service={airport} />
      </AuthProvider>,
    );
    expect(screen.getByRole("heading", { level: 1, name: airport.h1 })).toBeInTheDocument();
    expect(
      screen.getByRole("link", { name: /whitefield to bangalore airport taxi/i }),
    ).toHaveAttribute("href", "/whitefield-to-bangalore-airport-taxi");
    expect(screen.getByRole("heading", { level: 2, name: "East Bangalore" })).toBeInTheDocument();
    expect(screen.getByRole("heading", { level: 2, name: "South-East Bangalore" })).toBeInTheDocument();
    expect(screen.getByRole("heading", { level: 2, name: "South Bangalore" })).toBeInTheDocument();
    expect(screen.getByRole("heading", { level: 2, name: "Central / West" })).toBeInTheDocument();
    expect(screen.getByRole("heading", { level: 2, name: "North Bangalore" })).toBeInTheDocument();
    unmount();
    render(
      <AuthProvider>
        <ServiceLandingPage service={outstation} />
      </AuthProvider>,
    );
    expect(screen.getByRole("heading", { level: 1, name: outstation.h1 })).toBeInTheDocument();
    expect(outstation.h1).not.toBe(airport.h1);
    expect(screen.getByRole("link", { name: /bangalore → mysore/i })).toBeInTheDocument();
  });
});

describe("route metadata and sitemap", () => {
  it("creates self-canonical metadata and respects noindex", () => {
    const indexed = createPageMetadata({
      title: "Whitefield to Bangalore Airport Taxi",
      description: "Advance taxi from Whitefield.",
      path: "/whitefield-to-bangalore-airport-taxi",
    });
    expect(indexed.alternates?.canonical).toBe("/whitefield-to-bangalore-airport-taxi");
    const hidden = createPageMetadata({
      title: "Review only",
      description: "Not for Google.",
      path: "/review-only-demo-route",
      indexable: false,
    });
    expect(hidden.robots).toEqual({ index: false, follow: false });
    expect(legalPagesArePlaceholders).toBe(true);
    const legal = createPageMetadata({
      title: "Privacy Policy",
      description: "Placeholder",
      path: "/privacy-policy",
      indexable: !legalPagesArePlaceholders,
    });
    expect(legal.robots).toEqual({ index: false, follow: false });
  });

  it("self-canonicalizes every published route without cab or alias alternatives", () => {
    for (const route of getIndexableRoutes()) {
      const metadata = createPageMetadata({
        title: route.seoTitle,
        description: route.metaDescription,
        path: `/${route.slug}`,
      });
      expect(metadata.alternates?.canonical).toBe(`/${route.slug}`);
    }
  });

  it("includes indexable routes and services and excludes drafts, fixtures, and legal placeholders", () => {
    const urls = sitemap().map((entry) => entry.url);
    const paths = getSitemapPaths();
    expect(paths).toEqual(getIndexableRenderedPaths());
    expect(urls.some((url) => url.includes("/whitefield-to-bangalore-airport-taxi"))).toBe(true);
    expect(urls.some((url) => url.includes("/airport-taxi-bangalore"))).toBe(true);
    expect(urls.some((url) => url.includes("/outstation-taxi-bangalore"))).toBe(true);
    expect(urls.some((url) => url.includes("/unpublished-demo-route"))).toBe(false);
    expect(urls.some((url) => url.includes("/review-only-demo-route"))).toBe(false);
    expect(urls.some((url) => url.includes("/privacy-policy"))).toBe(false);
    expect(urls.some((url) => url.includes("/terms-and-conditions"))).toBe(false);
    expect(urls.some((url) => url.includes("/taxi-service-"))).toBe(false);
    expect(getSitemapPaths()).toHaveLength(1 + getIndexableRoutes().length + 2);
  });

  it("emits parseable BreadcrumbList, Service, and FAQ JSON-LD without ratings", () => {
    const route = getIndexableRoutes()[0];
    const blobs = [
      breadcrumbJsonLd([
        { name: "Home", path: "/" },
        { name: "Airport taxi", path: "/airport-taxi-bangalore" },
        { name: route.h1, path: `/${route.slug}` },
      ]),
      serviceJsonLd({
        name: route.h1,
        description: route.metaDescription,
        path: `/${route.slug}`,
        serviceType: "Airport taxi",
      }),
      faqJsonLd(route.faq),
    ].map((data) => JSON.stringify(data));
    for (const blob of blobs) {
      expect(() => JSON.parse(blob)).not.toThrow();
      expect(blob).not.toMatch(/AggregateRating|reviewRating|"price"/);
    }
  });
});

describe("homepage route links", () => {
  it("only links popular routes that exist", () => {
    for (const route of popularRoutes) {
      if ("href" in route && route.href) {
        expect(isImplementedPublicPath(route.href), route.href).toBe(true);
      }
    }
  });

  it("does not expose unpublished catalog slugs as public paths", () => {
    expect(isImplementedPublicPath("/unpublished-demo-route")).toBe(false);
    expect(routePages.some((page) => page.slug === "unpublished-demo-route")).toBe(true);
  });
});
