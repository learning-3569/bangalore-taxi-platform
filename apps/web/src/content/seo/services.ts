import { media } from "@/config/media";
import type { ServicePageContent } from "@/content/seo/types";

export const airportTaxiService: ServicePageContent = {
  slug: "airport-taxi-bangalore",
  published: true,
  indexable: true,
  lastUpdated: "2026-08-26",
  routeType: "airport",
  seoTitle: "Airport Taxi Bangalore",
  metaDescription:
    "Advance airport taxi in Bangalore for Kempegowda International Airport pickups and drops. Book a sedan, SUV, or Innova with Bengaluru Cabs.",
  h1: "Airport taxi in Bangalore",
  heroEyebrow: "Kempegowda International Airport",
  heroText:
    "Timed cars for BLR pickups and drops — from east-side tech parks, inner neighbourhoods, and south Bangalore campuses.",
  heroImage: media.heroAirport,
  intro:
    "Airport work is a different brief from a city hop. Bags, flight windows, and terminal kerbs matter more than a neighbourhood shortcut. Bengaluru Cabs takes the request in advance; a dispatcher reviews it; a car is assigned when operations confirm. This page is the parent for locality-to-airport and airport-to-locality landers that have their own unique copy. Fares are not listed. We do not claim airport concessions or airline partnerships we do not have.",
  sections: [
    {
      heading: "Drops to the airport",
      body: "Leave from a home, hotel, or office with a car already planned around check-in. The useful advice is buffer for Bangalore traffic, the right boot for luggage, and a pin the driver can actually stop at. Dedicated pages exist where that story is specific — Whitefield, Electronic City, Koramangala — rather than a template with the locality name swapped.",
    },
    {
      heading: "Pickups after you land",
      body: "Arrivals are about meeting you after baggage, not guessing a city average. Share the flight number when you can. Precise kerb instructions and waiting rules will be published with the live booking flow. Until then, treat this as the service overview, not a meet-and-greet product sheet.",
    },
    {
      heading: "How to request the car",
      body: "Use the booking form with pickup, drop, date, and time. Online submit is a preview today. Phone verification will be required as part of the booking flow when accounts launch. Guest checkout is not part of V1.",
    },
  ],
  faq: [
    {
      question: "Do you cover both pickup and drop at Bangalore Airport?",
      answer:
        "Yes. This service is for drops to Kempegowda International Airport and pickups after landing. Specific locality pages go deeper where we have unique copy.",
    },
    {
      question: "Can I book an airport taxi in advance?",
      answer:
        "Advance request is the model. The form is a preview until booking APIs go live. A dispatcher will accept the trip before a driver is assigned.",
    },
    {
      question: "Will I need to verify my phone?",
      answer:
        "Phone verification will be required as part of the booking flow. That login is not on this website yet. Guest booking is not offered in V1.",
    },
  ],
};

export const outstationTaxiService: ServicePageContent = {
  slug: "outstation-taxi-bangalore",
  published: true,
  indexable: true,
  lastUpdated: "2026-08-26",
  routeType: "outstation",
  seoTitle: "Outstation Taxi Bangalore",
  metaDescription:
    "Outstation taxi from Bangalore for Mysore, Coorg, and similar corridors. One-way or round-trip cars with Bengaluru Cabs. Fare on request.",
  h1: "Outstation taxi from Bangalore",
  heroEyebrow: "Intercity cars",
  heroText:
    "Highway and hill-road trips out of Bangalore — one-way or with a planned return — assigned by a local desk, not a street hail.",
  heroImage: media.heroOutstation,
  intro:
    "Outstation work is not an airport drop with the city name changed. Duty hours, luggage for a full cabin, and whether the car should wait for a return all belong in the request. We publish a route page only when there is something distinct to say — Mysore is a highway corridor; Coorg is hill country. Other destinations stay listed until they have unique copy. No invented hours or package prices.",
  sections: [
    {
      heading: "One-way and round-trip",
      body: "Choose one-way when you are not coming back with the same car. Choose round-trip when the group returns together. Waiting, night halt, and kilometre rules will come from the pricing engine — they are not printed here.",
    },
    {
      heading: "Vehicles for the road",
      body: "Sedan suits a pair with light bags. Families usually ask for an SUV or Innova Crysta. Premium is a quieter cabin when the trip is work. Exact models depend on live inventory.",
    },
    {
      heading: "How to request the car",
      body: "The form is a preview. Phone verification will be required as part of the booking flow later. Do not expect a live confirmation SMS from this page.",
    },
  ],
  faq: [
    {
      question: "Which outstation routes have their own pages?",
      answer:
        "Bangalore to Mysore and Bangalore to Coorg are published. Other corridors appear when each has unique content — not as a bulk list of city names.",
    },
    {
      question: "Can I book only the onward journey?",
      answer: "Yes. Use one-way on the form. Use round-trip when the same car should return with you.",
    },
    {
      question: "Are fares shown here?",
      answer: "No. Price on request until the pricing engine is live.",
    },
  ],
};
