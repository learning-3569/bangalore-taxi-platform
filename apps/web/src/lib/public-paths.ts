import { legalAndHomePaths } from "@/config/site";
import { getGeneratedSeoPaths, getIndexableSeoPaths } from "@/content/seo/catalog";

export function getPublicPaths(): string[] {
  return [...legalAndHomePaths, ...getIndexableSeoPaths()];
}

export function getRenderablePaths(): string[] {
  return [...legalAndHomePaths, ...getGeneratedSeoPaths()];
}
