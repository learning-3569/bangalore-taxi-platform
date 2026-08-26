import type { MetadataRoute } from "next";
import { getSiteUrl, isPublicIndexable } from "@/config/site";

export default function robots(): MetadataRoute.Robots {
  const siteUrl = getSiteUrl();
  if (!isPublicIndexable()) {
    return {
      rules: [{ userAgent: "*", disallow: "/" }],
      sitemap: `${siteUrl}/sitemap.xml`,
      host: siteUrl,
    };
  }
  return {
    rules: [{ userAgent: "*", allow: "/" }],
    sitemap: `${siteUrl}/sitemap.xml`,
    host: siteUrl,
  };
}
