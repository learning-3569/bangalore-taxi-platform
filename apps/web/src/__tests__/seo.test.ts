import { describe, expect, it } from "vitest";
import { legalPages, navItems, services } from "@/config/site";
import { isImplementedPublicPath } from "@/lib/paths";
import { createPageMetadata } from "@/lib/seo";
import { faqJsonLd, localBusinessJsonLd } from "@/components/seo/JsonLd";
import robots from "@/app/robots";
import sitemap from "@/app/sitemap";

describe("seo foundation", () => {
  it("creates title, description, canonical, and social metadata", () => {
    const metadata = createPageMetadata({
      title: "Test title",
      description: "Test description",
      path: "/",
    });
    expect(metadata.title).toBe("Test title");
    expect(metadata.description).toBe("Test description");
    expect(metadata.alternates?.canonical).toBe("/");
    expect(metadata.openGraph?.title).toBe("Test title");
    expect(metadata.twitter?.title).toBe("Test title");
  });

  it("lists only real public URLs in the sitemap", () => {
    const entries = sitemap();
    const urls = entries.map((entry) => entry.url);
    expect(urls.some((url) => url.endsWith("/") || url.endsWith("43121"))).toBe(true);
    expect(urls.some((url) => url.includes("/privacy-policy"))).toBe(true);
    expect(urls.some((url) => url.includes("/terms-and-conditions"))).toBe(true);
    expect(urls.some((url) => url.includes("/unpublished-demo-route"))).toBe(false);
  });

  it("does not allow crawlers on the local development host", () => {
    const result = robots();
    const rules = Array.isArray(result.rules) ? result.rules : [result.rules];
    expect(rules[0]?.disallow).toBe("/");
    expect(result.sitemap).toMatch(/sitemap.xml$/);
  });

  it("does not emit ratings in local business JSON-LD", () => {
    const data = JSON.stringify(localBusinessJsonLd());
    expect(data).not.toMatch(/AggregateRating|reviewRating|"ratingValue"/);
    expect(data).not.toMatch(/telephone|streetAddress/);
  });

  it("keeps FAQ JSON-LD aligned with question count", () => {
    const faqs = [{ question: "Q", answer: "A" }];
    const data = faqJsonLd(faqs);
    expect(data.mainEntity).toHaveLength(1);
  });
});

describe("internal links", () => {
  it("only points at implemented public paths", () => {
    const hrefs = [...navItems, ...legalPages, ...services].map((item) => item.href);
    for (const href of hrefs) {
      expect(isImplementedPublicPath(href), href).toBe(true);
    }
  });
});
