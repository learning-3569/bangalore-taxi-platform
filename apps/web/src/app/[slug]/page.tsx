import { notFound } from "next/navigation";
import { RouteLandingPage } from "@/components/routes/RouteLandingPage";
import { getPublishedRoute, getPublishedRoutes } from "@/content/seo/catalog";
import { createPageMetadata } from "@/lib/seo";

type PageProps = {
  params: Promise<{ slug: string }>;
};

export const dynamicParams = false;

export function generateStaticParams() {
  return getPublishedRoutes().map((page) => ({ slug: page.slug }));
}

export async function generateMetadata({ params }: PageProps) {
  const { slug } = await params;
  const route = getPublishedRoute(slug);
  if (!route) return {};
  return createPageMetadata({
    title: route.seoTitle,
    description: route.metaDescription,
    path: `/${route.slug}`,
    image: route.heroImage.src,
    indexable: route.indexable,
  });
}

export default async function SeoSlugPage({ params }: PageProps) {
  const { slug } = await params;
  const route = getPublishedRoute(slug);
  if (!route) notFound();
  return <RouteLandingPage route={route} />;
}
