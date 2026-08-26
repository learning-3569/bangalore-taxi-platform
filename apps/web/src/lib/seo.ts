import type { Metadata } from "next";
import { getSiteUrl, isPublicIndexable, siteConfig } from "@/config/site";

export function createPageMetadata({
  title,
  description,
  path,
  image,
  indexable = true,
}: {
  title: string;
  description: string;
  path: string;
  image?: string;
  indexable?: boolean;
}): Metadata {
  const url = new URL(path, getSiteUrl()).toString();
  const ogImage = image ?? "/images/hero-airport.jpg";
  const allowIndex = isPublicIndexable() && indexable;
  return {
    title,
    description,
    alternates: { canonical: path },
    openGraph: {
      type: "website",
      locale: "en_IN",
      siteName: siteConfig.name,
      title,
      description,
      url,
      images: [{ url: ogImage }],
    },
    twitter: {
      card: "summary_large_image",
      title,
      description,
      images: [ogImage],
    },
    robots: { index: allowIndex, follow: allowIndex },
  };
}
