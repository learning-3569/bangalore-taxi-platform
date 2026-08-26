import { ServiceLandingPage } from "@/components/routes/ServiceLandingPage";
import { getServicePage } from "@/content/seo/catalog";
import { createPageMetadata } from "@/lib/seo";

const service = getServicePage("outstation-taxi-bangalore");

export const metadata = createPageMetadata({
  title: service.seoTitle,
  description: service.metaDescription,
  path: `/${service.slug}`,
  image: service.heroImage.src,
  indexable: service.indexable,
});

export default function OutstationTaxiBangalorePage() {
  return <ServiceLandingPage service={service} />;
}
