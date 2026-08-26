import { legalAndHomePaths } from "@/config/site";
import { getRenderablePaths } from "@/lib/public-paths";

/** Path portion of an href. Hash-only links are treated as the homepage. */
export function pathFromHref(href: string): string {
  const path = href.split("#")[0];
  return path === "" ? "/" : path;
}

export function isImplementedPublicPath(href: string): boolean {
  const path = pathFromHref(href);
  if (path === "/") return true;
  if ((legalAndHomePaths as readonly string[]).includes(path)) return true;
  return getRenderablePaths().includes(path);
}
