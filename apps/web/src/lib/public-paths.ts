import { legalAndHomePaths, legalPages, legalPagesArePlaceholders } from "@/config/site";
import { getGeneratedSeoPaths, getIndexableRenderedPaths } from "@/content/seo/catalog";

/** Canonical sitemap paths: rendered, published, indexable pages only. */
export function getSitemapPaths(): string[] {
  const paths = getIndexableRenderedPaths();
  if (legalPagesArePlaceholders) return paths;
  return [...paths, ...legalPages.map((page) => page.href)];
}

export function getPublicPaths(): string[] {
  return getSitemapPaths();
}

export function getRenderablePaths(): string[] {
  return [...legalAndHomePaths, ...getGeneratedSeoPaths()];
}
