import { getSiteUrl, siteConfig } from "@/config/site";

export function JsonLd({ data }: { data: object }) {
  return (
    <script
      type="application/ld+json"
      dangerouslySetInnerHTML={{ __html: JSON.stringify(data) }}
    />
  );
}

export function websiteJsonLd() {
  return {
    "@context": "https://schema.org",
    "@type": "WebSite",
    name: siteConfig.name,
    url: getSiteUrl(),
    inLanguage: "en-IN",
    description: siteConfig.description,
  };
}

export function organizationJsonLd() {
  return {
    "@context": "https://schema.org",
    "@type": "Organization",
    name: siteConfig.name,
    url: getSiteUrl(),
    areaServed: {
      "@type": "City",
      name: "Bengaluru",
    },
  };
}

/** LocalBusiness without telephone, address, or ratings until the business confirms NAP. */
export function localBusinessJsonLd() {
  return {
    "@context": "https://schema.org",
    "@type": "TaxiService",
    name: siteConfig.name,
    url: getSiteUrl(),
    areaServed: {
      "@type": "City",
      name: "Bengaluru",
      containedInPlace: {
        "@type": "State",
        name: "Karnataka",
      },
    },
    serviceType: [
      "Airport taxi",
      "Local taxi",
      "Outstation taxi",
    ],
  };
}

export function breadcrumbJsonLd(items: readonly { name: string; path: string }[]) {
  const origin = getSiteUrl();
  return {
    "@context": "https://schema.org",
    "@type": "BreadcrumbList",
    itemListElement: items.map((item, index) => ({
      "@type": "ListItem",
      position: index + 1,
      name: item.name,
      item: new URL(item.path, origin).toString(),
    })),
  };
}

export function serviceJsonLd({
  name,
  description,
  path,
  serviceType,
}: {
  name: string;
  description: string;
  path: string;
  serviceType: string;
}) {
  return {
    "@context": "https://schema.org",
    "@type": "Service",
    name,
    description,
    serviceType,
    url: new URL(path, getSiteUrl()).toString(),
    provider: {
      "@type": "Organization",
      name: siteConfig.name,
      url: getSiteUrl(),
    },
    areaServed: {
      "@type": "City",
      name: "Bengaluru",
    },
  };
}

export function faqJsonLd(faqs: readonly { question: string; answer: string }[]) {
  return {
    "@context": "https://schema.org",
    "@type": "FAQPage",
    mainEntity: faqs.map((item) => ({
      "@type": "Question",
      name: item.question,
      acceptedAnswer: {
        "@type": "Answer",
        text: item.answer,
      },
    })),
  };
}
