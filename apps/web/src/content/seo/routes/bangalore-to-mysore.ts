import { media } from "@/config/media";
import type { RoutePageContent } from "@/content/seo/types";

export const bangaloreToMysore: RoutePageContent = {
  slug: "bangalore-to-mysore-taxi",
  published: true,
  indexable: true,
  lastUpdated: "2026-08-26",
  originId: "bangalore",
  destinationId: "mysore",
  routeType: "outstation",
  direction: "outstation-outbound",
  seoTitle: "Bangalore to Mysore Taxi",
  metaDescription:
    "Outstation taxi from Bangalore to Mysore. One-way or round-trip cars for family and work travel with Bengaluru Cabs. Fare on request.",
  h1: "Bangalore to Mysore taxi",
  heroEyebrow: "Outstation",
  heroText:
    "A dedicated car from Bangalore to Mysuru for a weekend, a family function, or a work day — one-way or with a planned return.",
  heroImage: media.heroOutstation,
  primaryCtaLabel: "Book this route",
  bookingHeading: "Book Bangalore → Mysore taxi",
  bookingSubmitLabel: "Request this trip",
  defaultTripType: "one-way",
  summary: {
    from: "Bangalore",
    to: "Mysore (Mysuru)",
    tripType: "Outstation — one-way or round-trip",
    vehicleCategories: "Sedan, SUV, Innova Crysta, Premium",
    distanceNote: "Exact kilometres will appear when mapping is connected.",
    durationNote: "Approximate information — final journey time depends on traffic.",
  },
  intro:
    "Mysore is a regular outstation ask from Bangalore: highway miles, not an airport kerb. People book it for family visits, palace-weekend trips, and office days that should not depend on a bus timetable. This page is about that intercity car, including whether you need the vehicle only on the way down or also for the return. It is not a recycled airport-drop article. We do not publish a highway hour-count or a package price here.",
  pickupInformation: {
    heading: "Leaving Bangalore",
    body: "Pickup can be a home, hotel, or office anywhere in the city you describe. Early starts are common so you arrive in Mysore with the day still ahead. If several family members leave from different neighbourhoods, say so — that is a routing problem for the desk, not something to improvise at 5 a.m.",
  },
  destinationInformation: {
    heading: "Arriving in Mysore",
    body: "Give a hotel, residence, or a central drop you actually want. We do not run a sightseeing itinerary product on this page. If you need the car to wait for a same-day return, that is a round-trip request with different duty expectations — choose round-trip on the form when you mean it. Local Mysore sightseeing hours, if offered later, will be a written policy, not an implied free add-on.",
  },
  travelGuidance: {
    heading: "One-way versus round-trip",
    body: "One-way makes sense when you stay over or travel back another way. Round-trip makes sense when the same group returns the same day or the next morning with the same car. Highway stops for tea or meals are normal; we do not sell a timed “express” product. Choose SUV or Innova when elders, children, and luggage share the cabin for the full highway stretch.",
  },
  whyChoose: [
    {
      title: "Intercity, not airport copy",
      body: "Duty, luggage, and return planning are the brief — not a terminal kerb.",
    },
    {
      title: "Family and work both fit",
      body: "Request the cabin that matches who is travelling, not a default sedan.",
    },
    {
      title: "Desk-assigned car",
      body: "Operations accept the trip before a driver is attached.",
    },
  ],
  howBookingWorks: [
    { title: "Set the cities", body: "Bangalore pickup, Mysore drop, date, time, one-way or round-trip." },
    { title: "Choose a category", body: "Highway comfort matters more than it does on a short city hop." },
    { title: "Desk review", body: "Form preview now; outstation requests reach the desk in a later phase." },
    { title: "Driver details", body: "After acceptance, on the launch messaging channel." },
  ],
  vehicleNotes: [
    { category: "Sedan", note: "Two or three adults with modest luggage on the highway." },
    { category: "SUV", note: "Families who want more seat height and boot space." },
    { category: "Innova Crysta", note: "The usual group ask for Bangalore–Mysore." },
    { category: "Premium", note: "When the trip is a client day or you want a quieter cabin." },
  ],
  relatedSlugs: ["bangalore-to-coorg-taxi", "whitefield-to-bangalore-airport-taxi"],
  parentServiceId: "outstation-taxi-bangalore",
  faq: [
    {
      question: "Can I book a one-way taxi from Bangalore to Mysore?",
      answer:
        "Yes. Choose one-way when you do not need the same car back. Round-trip is the right tab when the car should stay with you for the return.",
    },
    {
      question: "Do you offer round-trip cars?",
      answer:
        "You can request round-trip on the form. Waiting, night halt, and kilometre rules will come from the pricing engine — they are not listed here.",
    },
    {
      question: "What vehicle is comfortable for a family?",
      answer:
        "SUV or Innova Crysta is the usual family request. Sedan is enough for a couple with light bags.",
    },
    {
      question: "Can I book this for a business day trip?",
      answer:
        "Yes. Share pickup time and whether you need the car in Mysore during the day (round-trip) or only the outbound leg.",
    },
    {
      question: "Are fares shown on this page?",
      answer:
        "No. Price on request until the pricing engine is live. We do not invent a starting fare.",
    },
  ],
  farePlaceholder: "Price on request",
};
