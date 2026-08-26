import type { MetadataRoute } from "next";
import { getSiteUrl, legalAndHomePaths } from "@/config/site";
import { getIndexableRoutes, getIndexableServices } from "@/content/seo/catalog";

export default function sitemap(): MetadataRoute.Sitemap {
  const origin = getSiteUrl();

  const core = legalAndHomePaths.map((path, index) => ({
    url: new URL(path, origin).toString(),
    lastModified: new Date(),
    changeFrequency: "weekly" as const,
    priority: index === 0 ? 1 : 0.3,
  }));

  const services = getIndexableServices().map((page) => ({
    url: new URL(`/${page.slug}`, origin).toString(),
    lastModified: new Date(page.lastUpdated),
    changeFrequency: "weekly" as const,
    priority: 0.8,
  }));

  const routes = getIndexableRoutes().map((page) => ({
    url: new URL(`/${page.slug}`, origin).toString(),
    lastModified: new Date(page.lastUpdated),
    changeFrequency: "weekly" as const,
    priority: 0.7,
  }));

  return [...core, ...services, ...routes];
}
