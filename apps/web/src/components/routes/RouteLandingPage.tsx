import Image from "next/image";
import Link from "next/link";
import { BookingWidget } from "@/components/booking/BookingWidget";
import { FaqItem } from "@/components/content/FaqItem";
import { RouteStickyCta } from "@/components/routes/RouteStickyCta";
import { breadcrumbJsonLd, faqJsonLd, JsonLd, serviceJsonLd } from "@/components/seo/JsonLd";
import { Breadcrumbs } from "@/components/ui/Breadcrumbs";
import { Button } from "@/components/ui/Button";
import { Container } from "@/components/ui/Container";
import { SectionHeading } from "@/components/ui/SectionHeading";
import { fleet } from "@/config/site";
import { getRelatedRoutes, getRouteDestination, getRouteOrigin, getServicePage } from "@/content/seo/catalog";
import type { RoutePageContent } from "@/content/seo/types";

export function RouteLandingPage({ route }: { route: RoutePageContent }) {
  const related = getRelatedRoutes(route);
  const origin = getRouteOrigin(route);
  const destination = getRouteDestination(route);
  const parent = getServicePage(route.parentServiceId);
  const parentLabel = route.routeType === "airport" ? "Airport taxi" : "Outstation taxi";
  const visualCrumbs = [
    { label: "Home", href: "/" },
    { label: parentLabel, href: `/${parent.slug}` },
    { label: `${origin.name} to ${destination.name}` },
  ];
  const schemaCrumbs = [
    { name: "Home", path: "/" },
    { name: parentLabel, path: `/${parent.slug}` },
    { name: `${origin.name} to ${destination.name}`, path: `/${route.slug}` },
  ];
  const path = `/${route.slug}`;

  return (
    <>
      <JsonLd data={breadcrumbJsonLd(schemaCrumbs)} />
      <JsonLd
        data={serviceJsonLd({
          name: route.h1,
          description: route.metaDescription,
          path,
          serviceType: route.routeType === "airport" ? "Airport taxi" : "Outstation taxi",
        })}
      />
      {route.faq.length > 0 ? <JsonLd data={faqJsonLd(route.faq)} /> : null}

      <main id="main" className="pb-24 md:pb-0">
        <div className="border-b border-line bg-paper-soft py-3">
          <Container>
            <Breadcrumbs items={visualCrumbs} />
          </Container>
        </div>

        <section className="relative isolate min-h-[22rem] overflow-hidden bg-navy text-white md:min-h-[28rem]">
          <Image
            src={route.heroImage.src}
            alt={route.heroImage.alt}
            fill
            priority
            sizes="100vw"
            className="object-cover object-[70%_center]"
          />
          <div className="hero-scrim absolute inset-0" />
          <Container className="relative z-10 flex min-h-[22rem] flex-col justify-end pb-10 pt-16 md:min-h-[28rem] md:justify-center md:pb-16">
            <p className="text-xs font-semibold uppercase tracking-[0.22em] text-taxi">{route.heroEyebrow}</p>
            <h1 className="mt-3 max-w-3xl font-display text-3xl font-semibold leading-[1.15] tracking-tight sm:text-5xl">
              {route.h1}
            </h1>
            <p className="mt-4 max-w-xl text-base leading-relaxed text-white/80 sm:text-lg">{route.heroText}</p>
            <div className="mt-7 flex flex-wrap gap-3">
              <Button href="#book" variant="taxi" className="uppercase">
                {route.primaryCtaLabel}
              </Button>
              <Button href="/#contact" variant="secondary">
                Call now
              </Button>
            </div>
          </Container>
        </section>

        <section className="border-b border-line bg-paper py-10">
          <Container>
            <SectionHeading eyebrow="Route summary" title={`${origin.name} to ${destination.name}`} />
            <dl className="mt-8 grid gap-px overflow-hidden border border-line bg-line sm:grid-cols-2 lg:grid-cols-3">
              {[
                ["From", route.summary.from],
                ["To", route.summary.to],
                ["Trip type", route.summary.tripType],
                ["Vehicles", route.summary.vehicleCategories],
                ["Distance", route.summary.distanceNote],
                ["Travel time", route.summary.durationNote],
                ["Fare", route.farePlaceholder],
              ].map(([label, value]) => (
                <div key={label} className="bg-paper p-4">
                  <dt className="text-xs font-semibold uppercase tracking-[0.14em] text-ink-muted">{label}</dt>
                  <dd className="mt-1 text-sm leading-relaxed text-navy">{value}</dd>
                </div>
              ))}
            </dl>
          </Container>
        </section>

        <section id="book" className="bg-paper-soft py-12 sm:py-16">
          <Container>
            <BookingWidget
              idPrefix={`${route.slug}-`}
              defaultPickup={origin.name}
              defaultDrop={destination.name}
              defaultTripType={route.defaultTripType}
              heading={route.bookingHeading}
              submitLabel={route.bookingSubmitLabel}
            />
          </Container>
        </section>

        <section className="py-14 sm:py-20">
          <Container className="max-w-3xl">
            <SectionHeading eyebrow="About this route" title="How this trip actually works" />
            <p className="mt-6 text-base leading-relaxed text-ink">{route.intro}</p>
          </Container>
        </section>

        <section className="border-y border-line bg-paper-soft py-14 sm:py-20">
          <Container className="grid gap-10 lg:grid-cols-3">
            {[route.pickupInformation, route.destinationInformation, route.travelGuidance].map((block) => (
              <article key={block.heading}>
                <h2 className="font-display text-xl font-semibold text-navy">{block.heading}</h2>
                <p className="mt-3 text-sm leading-relaxed text-ink-muted">{block.body}</p>
              </article>
            ))}
          </Container>
        </section>

        <section className="py-14 sm:py-20">
          <Container>
            <SectionHeading eyebrow="Vehicles" title="Choose a category, not a fantasy model" />
            <ul className="mt-8 grid gap-6 sm:grid-cols-2">
              {route.vehicleNotes.map((note) => {
                const image = fleet.find((item) => item.name === note.category)?.image;
                return (
                  <li key={note.category} className="border border-line bg-paper p-4">
                    {image ? (
                      <div className="relative mb-4 aspect-[16/9] overflow-hidden bg-paper-soft">
                        <Image src={image.src} alt={image.alt} fill sizes="(max-width: 768px) 100vw, 50vw" className="object-cover" />
                      </div>
                    ) : null}
                    <h3 className="font-display text-lg font-semibold text-navy">{note.category}</h3>
                    <p className="mt-2 text-sm leading-relaxed text-ink-muted">{note.note}</p>
                  </li>
                );
              })}
            </ul>
          </Container>
        </section>

        <section className="bg-navy py-14 text-white sm:py-20">
          <Container>
            <SectionHeading eyebrow="Why book this corridor" title="What the desk actually does" invert />
            <ul className="mt-10 grid gap-6 sm:grid-cols-3">
              {route.whyChoose.map((item) => (
                <li key={item.title}>
                  <h3 className="font-display text-lg font-semibold">{item.title}</h3>
                  <p className="mt-2 text-sm leading-relaxed text-white/70">{item.body}</p>
                </li>
              ))}
            </ul>
          </Container>
        </section>

        <section className="py-14 sm:py-20">
          <Container>
            <SectionHeading eyebrow="How booking works" title="Request, review, then a car" />
            <ol className="mt-10 grid gap-8 sm:grid-cols-2 lg:grid-cols-4">
              {route.howBookingWorks.map((item, index) => (
                <li key={item.title}>
                  <p className="font-display text-4xl font-semibold text-taxi/80">{String(index + 1).padStart(2, "0")}</p>
                  <h3 className="mt-2 font-display text-lg font-semibold text-navy">{item.title}</h3>
                  <p className="mt-2 text-sm leading-relaxed text-ink-muted">{item.body}</p>
                </li>
              ))}
            </ol>
          </Container>
        </section>

        {related.length > 0 ? (
          <section className="border-y border-line bg-paper-soft py-14 sm:py-20">
            <Container>
              <SectionHeading eyebrow="Related routes" title="Other corridors with real pages" />
              <ul className="mt-8 grid gap-3 sm:grid-cols-2">
                {related.map((item) => (
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
              <p className="mt-6 text-sm text-ink-muted">
                Looking for the wider service?{" "}
                <Link href={`/${parent.slug}`} className="font-medium text-navy underline">
                  {parent.h1}
                </Link>
                .
              </p>
            </Container>
          </section>
        ) : null}

        {route.faq.length > 0 ? (
          <section className="py-14 sm:py-20">
            <Container className="max-w-3xl">
              <SectionHeading eyebrow="Questions" title="Before you request the car" />
              <div className="mt-6 divide-y divide-line border-y border-line">
                {route.faq.map((item) => (
                  <FaqItem key={item.question} question={item.question} answer={item.answer} />
                ))}
              </div>
            </Container>
          </section>
        ) : null}

        <section className="bg-navy py-14 text-white sm:py-16">
          <Container className="flex flex-col items-start justify-between gap-6 sm:flex-row sm:items-center">
            <div>
              <h2 className="font-display text-2xl font-semibold">Ready to request this car?</h2>
              <p className="mt-2 max-w-lg text-sm text-white/70">
                The form above is a preview. Phone and OTP login, and live desk booking, come in later phases.
              </p>
            </div>
            <Button href="#book" variant="taxi" className="uppercase">
              {route.primaryCtaLabel}
            </Button>
          </Container>
        </section>
      </main>
      <RouteStickyCta bookHref="#book" bookLabel={route.primaryCtaLabel} />
    </>
  );
}
