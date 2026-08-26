import {
  Airport,
  Contact,
  Faq,
  FinalCta,
  Fleet,
  Hero,
  HowItWorks,
  Outstation,
  PopularRoutes,
  Reviews,
  SeoCopy,
  Services,
  TrustBar,
  WhyChooseUs,
} from "@/components/home/sections";
import { faqJsonLd, JsonLd, localBusinessJsonLd, organizationJsonLd, websiteJsonLd } from "@/components/seo/JsonLd";
import { faqs } from "@/config/site";
import { createPageMetadata } from "@/lib/seo";

export const metadata = createPageMetadata({
  title: "Bangalore Taxi Booking | Airport, Local and Outstation Cabs",
  description:
    "Book a Bangalore taxi for Kempegowda Airport transfers, city rides, and outstation trips. Advance cab booking with a locally operated fleet.",
  path: "/",
});

export default function HomePage() {
  return (
    <>
      <JsonLd data={websiteJsonLd()} />
      <JsonLd data={organizationJsonLd()} />
      <JsonLd data={localBusinessJsonLd()} />
      <JsonLd data={faqJsonLd(faqs)} />
      <main id="main">
        <Hero />
        <TrustBar />
        <Services />
        <Airport />
        <Outstation />
        <Fleet />
        <WhyChooseUs />
        <PopularRoutes />
        <HowItWorks />
        <Reviews />
        <Faq />
        <SeoCopy />
        <FinalCta />
        <Contact />
      </main>
    </>
  );
}
