import Image from "next/image";
import Link from "next/link";
import { BookingWidget } from "@/components/booking/BookingWidget";
import { FaqItem } from "@/components/content/FaqItem";
import { HeroCarousel } from "@/components/hero/HeroCarousel";
import { Button } from "@/components/ui/Button";
import { Container } from "@/components/ui/Container";
import { SectionHeading } from "@/components/ui/SectionHeading";
import { media } from "@/config/media";
import {
  businessPlaceholders,
  exampleTestimonials,
  faqs,
  fleet,
  howItWorks,
  outstationDestinations,
  popularRoutes,
  services,
  trustItems,
  whyChooseUs,
} from "@/config/site";

export function Hero() {
  return (
    <div>
      <HeroCarousel />
      <div className="relative z-20 -mt-24 px-4 pb-6 sm:px-6 lg:px-8">
        <div className="mx-auto max-w-6xl" id="book">
          <BookingWidget />
        </div>
      </div>
    </div>
  );
}

export function TrustBar() {
  return (
    <section aria-label="Why travellers use this desk" className="border-y border-line bg-paper-soft">
      <Container className="grid gap-8 py-10 sm:grid-cols-2 lg:grid-cols-5 lg:gap-6">
        {trustItems.map((item) => (
          <p key={item.title} className="text-sm leading-relaxed text-ink-muted">
            <strong className="block font-display text-sm font-semibold text-navy">{item.title}</strong>
            <span className="mt-1 block">{item.text}</span>
          </p>
        ))}
      </Container>
    </section>
  );
}

export function Services() {
  const featured = services.find((item) => item.featured) ?? services[0];
  const rest = services.filter((item) => item !== featured);

  return (
    <section id="services" className="py-16 sm:py-24">
      <Container>
        <div className="flex flex-col justify-between gap-6 lg:flex-row lg:items-end">
          <SectionHeading
            eyebrow="Taxi services"
            title="A car for the trip you are actually making."
            description="Airport, city, and the highway out of Bengaluru. Linked sections on this page until each lander has its own copy."
          />
          <p className="max-w-sm text-sm text-ink-muted lg:text-right">
            Not six identical tiles. Airport work sits first because that is the search people type at 11pm.
          </p>
        </div>

        <article className="mt-10 grid overflow-hidden border border-line lg:grid-cols-[1.15fr_0.85fr]">
          <div className="relative min-h-64 lg:min-h-[26rem]">
            <Image
              src={featured.image.src}
              alt={featured.image.alt}
              fill
              className="object-cover transition duration-700 hover:scale-[1.03]"
              sizes="(min-width: 1024px) 55vw, 100vw"
            />
          </div>
          <div className="flex flex-col justify-center bg-navy p-8 text-white sm:p-10">
            <p className="text-xs font-semibold uppercase tracking-[0.18em] text-taxi">Lead service</p>
            <h3 className="mt-3 font-display text-3xl font-semibold">{featured.title}</h3>
            <p className="mt-3 text-sm leading-relaxed text-white/75">{featured.description}</p>
            <Button href={featured.href} variant="taxi" className="mt-6 w-fit uppercase">
              Book airport taxi
            </Button>
          </div>
        </article>

        <ul className="mt-4 grid gap-4 md:grid-cols-2 lg:grid-cols-3">
          {rest.map((item) => (
            <li key={item.title} className="group grid grid-rows-[10rem_1fr] overflow-hidden border border-line bg-paper">
              <div className="relative">
                <Image
                  src={item.image.src}
                  alt={item.image.alt}
                  fill
                  className="object-cover transition duration-500 group-hover:scale-[1.04]"
                  sizes="(min-width: 1024px) 30vw, 100vw"
                />
              </div>
              <div className="p-5">
                <h3 className="font-display text-lg font-semibold text-navy">{item.title}</h3>
                <p className="mt-2 text-sm leading-relaxed text-ink-muted">{item.description}</p>
                <a href={item.href} className="mt-3 inline-block text-sm font-semibold text-navy underline-offset-4 hover:underline">
                  View section
                </a>
              </div>
            </li>
          ))}
        </ul>
      </Container>
    </section>
  );
}

export function Airport() {
  return (
    <section id="airport" className="bg-paper-soft py-16 sm:py-24">
      <Container className="grid items-center gap-10 lg:grid-cols-2 lg:gap-16">
        <div className="relative min-h-72 overflow-hidden lg:min-h-[28rem]">
          <Image
            src={media.airportFeature.src}
            alt={media.airportFeature.alt}
            fill
            className="object-cover"
            sizes="(min-width: 1024px) 50vw, 100vw"
          />
        </div>
        <div>
          <SectionHeading
            eyebrow="Kempegowda International Airport"
            title="Bangalore airport taxi, booked before you fly."
            description="Pickup at arrivals or drop at departures. Share the flight window in the form so the desk can plan — kerbside guessing is how people miss bags."
          />
          <ul className="mt-8 space-y-4 border-l-2 border-taxi pl-5 text-sm leading-relaxed text-ink-muted">
            <li>
              <strong className="text-navy">Airport pickup.</strong> Terminal, flight window, and a car that is already on the roster.
            </li>
            <li>
              <strong className="text-navy">Airport drop.</strong> Whitefield, Electronic City, Koramangala and the rest of the city — early departures included.
            </li>
            <li>
              <strong className="text-navy">Advance booking.</strong> Request → desk confirmation → driver details. Live SLAs when operations go online.
            </li>
          </ul>
          <Button href="/#book" variant="primary" className="mt-8 uppercase">
            Book airport taxi
          </Button>
          <p className="mt-3 text-xs text-ink-muted">
            Dedicated URL later: /airport-taxi-bangalore — only when that page has unique copy.
          </p>
        </div>
      </Container>
    </section>
  );
}

export function Outstation() {
  return (
    <section id="outstation" className="py-16 sm:py-24">
      <Container className="grid items-center gap-10 lg:grid-cols-2 lg:gap-16">
        <div className="lg:order-2">
          <div className="relative min-h-72 overflow-hidden lg:min-h-[28rem]">
            <Image
              src={media.outstationFeature.src}
              alt={media.outstationFeature.alt}
              fill
              className="object-cover"
              sizes="(min-width: 1024px) 50vw, 100vw"
            />
          </div>
        </div>
        <div className="lg:order-1">
          <SectionHeading
            eyebrow="Outstation taxi from Bangalore"
            title="The highway when a flight is the wrong tool."
            description="Mysore, Coorg, Chennai, Ooty, Hyderabad. One-way or round-trip. Route pages only when each destination has something worth saying."
          />
          <p className="mt-6 flex flex-wrap gap-2">
            {["One way", "Round trip", "Family travel", "Weekend trips", "Business travel"].map((item) => (
              <span key={item} className="border border-line px-3 py-1.5 text-xs font-semibold uppercase tracking-wide text-navy">
                {item}
              </span>
            ))}
          </p>
          <p className="mt-6 text-sm text-ink-muted">
            Frequent asks: {outstationDestinations.join(", ")}.
          </p>
          <Button href="/#book" variant="outline" className="mt-8 uppercase">
            Explore outstation taxi
          </Button>
        </div>
      </Container>
    </section>
  );
}

export function Fleet() {
  return (
    <section id="fleet" className="border-y border-line bg-navy py-16 text-white sm:py-24">
      <Container>
        <SectionHeading
          invert
          eyebrow="Our cars"
          title="Choose the cabin that fits the people and the bags."
          description="Categories, not a live stock list. Named models replace these placeholders when inventory is confirmed. Fares stay “price on request” until the engine is live."
        />
        <div className="mt-10 grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
          {fleet.map((vehicle) => (
            <article key={vehicle.name} className="bg-navy-mid">
              <div className="relative aspect-[4/3]">
                <Image
                  src={vehicle.image.src}
                  alt={vehicle.image.alt}
                  fill
                  className="object-cover"
                  sizes="(min-width: 1280px) 22vw, 50vw"
                />
              </div>
              <div className="p-5">
                <div className="flex items-baseline justify-between gap-2">
                  <h3 className="font-display text-xl font-semibold">{vehicle.name}</h3>
                  <p className="text-[11px] uppercase tracking-wide text-taxi">{vehicle.fare}</p>
                </div>
                <p className="mt-1 text-xs text-white/55">
                  {vehicle.seats} · {vehicle.luggage}
                </p>
                <p className="mt-2 text-sm leading-relaxed text-white/70">{vehicle.description}</p>
                <Button href="/#book" variant="taxi" className="mt-4 w-full uppercase">
                  Request this type
                </Button>
              </div>
            </article>
          ))}
        </div>
      </Container>
    </section>
  );
}

export function WhyChooseUs() {
  return (
    <section id="about" className="bg-navy py-16 text-white sm:py-24">
      <Container>
        <SectionHeading
          invert
          eyebrow="Safety and the desk"
          title="Your timing. Our roster."
          description="Plain reasons to use a Bangalore operator. No “best in city” line, no invented years in business."
        />
        <ol className="mt-12 grid gap-px bg-white/10 sm:grid-cols-2 lg:grid-cols-5">
          {whyChooseUs.map((item, index) => (
            <li key={item.title} className="bg-navy p-6">
              <p className="font-display text-3xl text-taxi">{String(index + 1).padStart(2, "0")}</p>
              <h3 className="mt-4 font-display text-lg font-semibold">{item.title}</h3>
              <p className="mt-2 text-sm leading-relaxed text-white/65">{item.body}</p>
            </li>
          ))}
        </ol>
      </Container>
    </section>
  );
}

export function PopularRoutes() {
  return (
    <section id="routes" className="py-16 sm:py-24">
      <Container>
        <SectionHeading
          eyebrow="Popular routes"
          title="Corridors we expect to run often."
          description="Published corridors link through. The rest stay as labels until they have unique pages."
        />
        <div className="mt-8 overflow-x-auto border border-line">
          <table className="w-full min-w-[32rem] text-left text-sm">
            <thead className="bg-paper-soft text-xs uppercase tracking-wider text-ink-muted">
              <tr>
                <th className="px-4 py-3 font-semibold">Route</th>
                <th className="px-4 py-3 font-semibold">Fare</th>
                <th className="px-4 py-3 font-semibold"> </th>
              </tr>
            </thead>
            <tbody>
              {popularRoutes.map((route) => (
                <tr key={`${route.from}-${route.to}`} className="border-t border-line">
                  <td className="px-4 py-4 font-medium text-navy">
                    {"href" in route && route.href ? (
                      <Link href={route.href} className="hover:underline">
                        {route.from} → {route.to}
                      </Link>
                    ) : (
                      <>
                        {route.from} → {route.to}
                      </>
                    )}
                  </td>
                  <td className="px-4 py-4 text-ink-muted">Price on request</td>
                  <td className="px-4 py-4 text-right">
                    <a
                      href={"href" in route && route.href ? `${route.href}#book` : "/#book"}
                      className="text-xs font-bold uppercase tracking-wide text-navy hover:text-taxi-deep"
                    >
                      Book →
                    </a>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </Container>
    </section>
  );
}

export function HowItWorks() {
  return (
    <section className="border-y border-line bg-paper-soft py-16 sm:py-20">
      <Container>
        <SectionHeading eyebrow="How it works" title="Four steps. No app store required." />
        <ol className="mt-10 grid gap-8 sm:grid-cols-2 lg:grid-cols-4">
          {howItWorks.map((item) => (
            <li key={item.step} className="relative pr-4">
              <p className="font-display text-5xl font-semibold text-taxi/80">{item.step}</p>
              <h3 className="mt-2 font-display text-lg font-semibold text-navy">{item.title}</h3>
              <p className="mt-2 text-sm leading-relaxed text-ink-muted">{item.body}</p>
            </li>
          ))}
        </ol>
      </Container>
    </section>
  );
}

export function Reviews() {
  return (
    <section id="reviews" className="py-16 sm:py-24">
      <Container className="max-w-4xl">
        <SectionHeading
          eyebrow="What riders say"
          title="Layout for real comments — later."
          description="These are typesetting samples, not Google stars. They will be replaced with permissioned quotes."
        />
        <div className="mt-10 space-y-10">
          {exampleTestimonials.map((item) => (
            <blockquote key={item.attribution} className="border-l-4 border-taxi pl-6">
              <p className="font-display text-xl leading-snug text-navy sm:text-2xl">{item.quote}</p>
              <footer className="mt-3 text-xs font-semibold uppercase tracking-[0.16em] text-ink-muted">
                {item.attribution}
              </footer>
            </blockquote>
          ))}
        </div>
      </Container>
    </section>
  );
}

export function Faq() {
  return (
    <section id="faq" className="bg-paper-soft py-16 sm:py-24">
      <Container className="max-w-3xl">
        <SectionHeading eyebrow="FAQ" title="Straight answers while policy is still being written." />
        <div className="mt-8 divide-y divide-line border-y border-line">
          {faqs.map((item) => (
            <FaqItem key={item.question} {...item} />
          ))}
        </div>
      </Container>
    </section>
  );
}

export function SeoCopy() {
  return (
    <section className="py-16 sm:py-20">
      <Container className="max-w-3xl text-[17px] leading-relaxed text-ink-muted">
        <h2 className="font-display text-2xl font-semibold text-navy">Taxi and cab booking in Bangalore</h2>
        <p className="mt-4">
          People type Bangalore taxi, Bangalore cab booking, and Bangalore airport taxi when they need a car that
          will actually be there — not a street gamble after a delayed flight. This site is the public desk for
          advance taxi booking with a Bengaluru fleet: Kempegowda Airport, city points, and outstation roads toward
          Mysore and Coorg.
        </p>
        <p className="mt-4">
          A request carries pickup, drop, date, time, trip type, and vehicle category. The desk confirms, then
          shares driver details. There is no payment on this website yet. Fares will come from the server when
          pricing is live.
        </p>
      </Container>
    </section>
  );
}

export function FinalCta() {
  return (
    <section className="relative isolate overflow-hidden py-20 text-white sm:py-28">
      <Image
        src={media.finalCta.src}
        alt={media.finalCta.alt}
        fill
        className="object-cover"
        sizes="100vw"
      />
      <div className="absolute inset-0 bg-navy/80" />
      <Container className="relative z-10 flex flex-col items-start justify-between gap-8 lg:flex-row lg:items-center">
        <div>
          <p className="text-xs font-semibold uppercase tracking-[0.2em] text-taxi">Ready when you are</p>
          <h2 className="mt-3 max-w-xl font-display text-3xl font-semibold sm:text-4xl">
            Ready for a quieter ride across Bangalore?
          </h2>
          <p className="mt-3 max-w-lg text-sm text-white/75">
            Book the cab in a few fields. Confirmation messaging switches on with the booking API.
          </p>
        </div>
        <div className="flex flex-wrap gap-3">
          <Button href="/#book" variant="taxi" className="uppercase">
            Book a cab
          </Button>
          <Button href="/#contact" variant="secondary">
            Call us
          </Button>
        </div>
      </Container>
    </section>
  );
}

export function Contact() {
  return (
    <section id="contact" className="py-16 sm:py-20">
      <Container className="max-w-2xl">
        <SectionHeading
          eyebrow="Contact"
          title="Talk to the booking desk"
          description={`${businessPlaceholders.phone}. ${businessPlaceholders.email}. ${businessPlaceholders.address}. Publish live numbers here only after the business confirms them.`}
        />
      </Container>
    </section>
  );
}
