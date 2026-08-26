import Image from "next/image";
import Link from "next/link";
import { BookingWidget } from "@/components/booking/BookingWidget";
import { FaqItem } from "@/components/content/FaqItem";
import { breadcrumbJsonLd, faqJsonLd, JsonLd, serviceJsonLd } from "@/components/seo/JsonLd";
import { Breadcrumbs } from "@/components/ui/Breadcrumbs";
import { Button } from "@/components/ui/Button";
import { Container } from "@/components/ui/Container";
import { SectionHeading } from "@/components/ui/SectionHeading";
import { getRouteDestination, getRouteOrigin, getRoutesByType } from "@/content/seo/catalog";
import type { ServicePageContent } from "@/content/seo/types";

export function ServiceLandingPage({ service }: { service: ServicePageContent }) {
  const routes = getRoutesByType(service.routeType).filter((route) => route.indexable);
  const path = `/${service.slug}`;
  const crumbs = [
    { label: "Home", href: "/" },
    { label: service.h1 },
  ];

  return (
    <>
      <JsonLd
        data={breadcrumbJsonLd([
          { name: "Home", path: "/" },
          { name: service.h1, path },
        ])}
      />
      <JsonLd
        data={serviceJsonLd({
          name: service.h1,
          description: service.metaDescription,
          path,
          serviceType: service.routeType === "airport" ? "Airport taxi" : "Outstation taxi",
        })}
      />
      {service.faq.length > 0 ? <JsonLd data={faqJsonLd(service.faq)} /> : null}

      <main id="main">
        <div className="border-b border-line bg-paper-soft py-3">
          <Container>
            <Breadcrumbs items={crumbs} />
          </Container>
        </div>

        <section className="relative isolate min-h-[22rem] overflow-hidden bg-navy text-white md:min-h-[28rem]">
          <Image
            src={service.heroImage.src}
            alt={service.heroImage.alt}
            fill
            priority
            sizes="100vw"
            className="object-cover object-[70%_center]"
          />
          <div className="hero-scrim absolute inset-0" />
          <Container className="relative z-10 flex min-h-[22rem] flex-col justify-end pb-10 pt-16 md:min-h-[28rem] md:justify-center md:pb-16">
            <p className="text-xs font-semibold uppercase tracking-[0.22em] text-taxi">{service.heroEyebrow}</p>
            <h1 className="mt-3 max-w-3xl font-display text-3xl font-semibold leading-[1.15] tracking-tight sm:text-5xl">
              {service.h1}
            </h1>
            <p className="mt-4 max-w-xl text-base leading-relaxed text-white/80 sm:text-lg">{service.heroText}</p>
            <div className="mt-7 flex flex-wrap gap-3">
              <Button href="#book" variant="taxi" className="uppercase">
                Book a cab
              </Button>
              <Button href="/#contact" variant="secondary">
                Call now
              </Button>
            </div>
          </Container>
        </section>

        <section id="book" className="bg-paper-soft py-12 sm:py-16">
          <Container>
            <BookingWidget
              idPrefix={`${service.slug}-`}
              defaultTripType={service.routeType === "airport" ? "airport" : "one-way"}
              heading={service.routeType === "airport" ? "Book an airport taxi" : "Book an outstation taxi"}
              submitLabel="Request this trip"
            />
          </Container>
        </section>

        <section className="py-14 sm:py-20">
          <Container className="max-w-3xl">
            <SectionHeading eyebrow="The service" title="What this desk actually does" />
            <p className="mt-6 text-base leading-relaxed text-ink">{service.intro}</p>
          </Container>
        </section>

        <section className="border-y border-line bg-paper-soft py-14 sm:py-20">
          <Container className="grid gap-10 lg:grid-cols-3">
            {service.sections.map((block) => (
              <article key={block.heading}>
                <h2 className="font-display text-xl font-semibold text-navy">{block.heading}</h2>
                <p className="mt-3 text-sm leading-relaxed text-ink-muted">{block.body}</p>
              </article>
            ))}
          </Container>
        </section>

        {routes.length > 0 ? (
          <section className="py-14 sm:py-20">
            <Container>
              <SectionHeading eyebrow="Published corridors" title="Route pages with their own copy" />
              <ul className="mt-8 grid gap-3 sm:grid-cols-2">
                {routes.map((item) => (
                  <li key={item.slug}>
                    <Link
                      href={`/${item.slug}`}
                      className="flex items-center justify-between border border-line bg-paper px-4 py-3 text-sm font-medium text-navy hover:border-navy"
                    >
                      <span>
                        {getRouteOrigin(item).name} → {getRouteDestination(item).name}
                      </span>
                      <span aria-hidden>→</span>
                    </Link>
                  </li>
                ))}
              </ul>
            </Container>
          </section>
        ) : null}

        {service.faq.length > 0 ? (
          <section className="border-t border-line py-14 sm:py-20">
            <Container className="max-w-3xl">
              <SectionHeading eyebrow="Questions" title="Before you request the car" />
              <div className="mt-6 divide-y divide-line border-y border-line">
                {service.faq.map((item) => (
                  <FaqItem key={item.question} question={item.question} answer={item.answer} />
                ))}
              </div>
            </Container>
          </section>
        ) : null}
      </main>
    </>
  );
}
