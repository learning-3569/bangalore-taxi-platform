import type { MetadataRoute } from "next";
import { getSiteUrl } from "@/config/site";
import { getIndexableRoutes, getIndexableServices } from "@/content/seo/catalog";
import { getSitemapPaths } from "@/lib/public-paths";

function lastModifiedForPath(path: string): Date | undefined {
  if (path === "/") return undefined;
  const slug = path.slice(1);
  const service = getIndexableServices().find((page) => page.slug === slug);
  if (service) return new Date(service.lastUpdated);
  const route = getIndexableRoutes().find((page) => page.slug === slug);
  if (route) return new Date(route.lastUpdated);
  return undefined;
}

export default function sitemap(): MetadataRoute.Sitemap {
  const origin = getSiteUrl();
  return getSitemapPaths().map((path) => {
    const isHome = path === "/";
    const isService = getIndexableServices().some((page) => `/${page.slug}` === path);
    return {
      url: new URL(path, origin).toString(),
      lastModified: lastModifiedForPath(path),
      changeFrequency: "weekly" as const,
      priority: isHome ? 1 : isService ? 0.8 : 0.7,
    };
  });
}
