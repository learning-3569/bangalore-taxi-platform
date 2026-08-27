import { notFound } from "next/navigation";
import { ServiceLandingPage } from "@/components/routes/ServiceLandingPage";
import { getPublishedService } from "@/content/seo/catalog";
import { createPageMetadata } from "@/lib/seo";

const slug = "airport-taxi-bangalore" as const;

export function generateMetadata() {
  const service = getPublishedService(slug);
  if (!service) return { robots: { index: false, follow: false } };
  return createPageMetadata({
    title: service.seoTitle,
    description: service.metaDescription,
    path: `/${service.slug}`,
    image: service.heroImage.src,
    indexable: service.indexable,
  });
}

export default function AirportTaxiBangalorePage() {
  const service = getPublishedService(slug);
  if (!service) notFound();
  return <ServiceLandingPage service={service} />;
}
