import { media } from "@/config/media";
import type { RoutePageContent } from "@/content/seo/types";

export const koramangalaToAirport: RoutePageContent = {
  slug: "koramangala-to-bangalore-airport-taxi",
  published: true,
  indexable: true,
  lastUpdated: "2026-08-26",
  originId: "koramangala",
  destinationId: "blr-airport",
  routeType: "airport",
  direction: "to-airport",
  seoTitle: "Koramangala to Bangalore Airport Taxi",
  metaDescription:
    "Airport taxi from Koramangala to Kempegowda International Airport. Advance drop from inner south Bangalore with Bengaluru Cabs.",
  h1: "Koramangala to Bangalore Airport taxi",
  heroEyebrow: "Inner-city airport drop",
  heroText:
    "Leave a Koramangala stay, office, or residence with a timed car for BLR — through inner-ring traffic first, then the airport road.",
  heroImage: media.heroCity,
  primaryCtaLabel: "Book this route",
  bookingHeading: "Book Koramangala → Airport taxi",
  bookingSubmitLabel: "Request this drop",
  defaultTripType: "airport",
  summary: {
    from: "Koramangala",
    to: "Kempegowda International Airport (BLR)",
    tripType: "Airport drop",
    vehicleCategories: "Sedan, SUV, Innova Crysta, Premium",
    distanceNote: "Exact kilometres will appear when mapping is connected.",
    durationNote: "Approximate information — final journey time depends on traffic.",
  },
  intro:
    "Koramangala is an inner south neighbourhood: hotels, residences, and offices packed onto a grid that slows in the evening. An airport drop from here is not the same brief as Whitefield (ORR-first) or Electronic City (a long south-to-north haul). The first kilometres are local streets and the inner ring; only then do you join the airport approach. We write that down so you plan the pickup pin carefully — a 5th Block address is not interchangeable with a Forum-side hotel without a note.",
  pickupInformation: {
    heading: "Pickup in Koramangala",
    body: "Give the block, building, or hotel name. One-way streets and evening restaurant traffic make vague “Koramangala” pins expensive in minutes. If the stay is a homestay without a clear gate, share a landmark the driver can actually stop at. We do not keep a secret Koramangala stand; the car comes to your point.",
  },
  destinationInformation: {
    heading: "Drop at Bangalore Airport",
    body: "Passenger terminals at Kempegowda International Airport. No unpublished airport privilege. After the drop, check-in and security are yours to schedule. If you are connecting from a late dinner in the neighbourhood, say so — the desk should know this is not a 5 a.m. tech-park departure.",
  },
  travelGuidance: {
    heading: "Inner-city then airport road",
    body: "Weekend evenings in Koramangala can stall before you ever see the airport highway. Weekday mornings are a different pattern. Luggage from a hotel checkout is often two large cases; mention that so a sedan is not assumed. We still will not print a drive-time number until a mapping source exists.",
  },
  whyChoose: [
    {
      title: "Neighbourhood-aware pickup",
      body: "Blocks and hotels matter here more than a campus gate code.",
    },
    {
      title: "Evening traffic honesty",
      body: "Inner south Bangalore can be the slow part of an airport run.",
    },
    {
      title: "Same assignment desk",
      body: "Reviewed request, then a car — not a street hail outside a café.",
    },
  ],
  howBookingWorks: [
    { title: "Name the block", body: "Koramangala pickup, airport drop, date, and time." },
    { title: "Choose a category", body: "Hotel checkouts with extra bags often need an SUV." },
    { title: "Desk review", body: "Preview only until booking APIs exist." },
    { title: "Driver details", body: "After acceptance, via the launch messaging channel." },
  ],
  vehicleNotes: [
    { category: "Sedan", note: "Typical for a couple leaving a hotel with cabin luggage." },
    { category: "SUV", note: "Preferable after a longer stay with more bags." },
    { category: "Innova Crysta", note: "Ask for a small group leaving the same address." },
    { category: "Premium", note: "A calmer cabin if the flight is a work trip." },
  ],
  relatedSlugs: [
    "whitefield-to-bangalore-airport-taxi",
    "electronic-city-to-bangalore-airport-taxi",
    "bangalore-airport-to-whitefield-taxi",
  ],
  parentServiceId: "airport-taxi-bangalore",
  faq: [
    {
      question: "Can I book a taxi from Koramangala to the airport in advance?",
      answer:
        "Yes. Use this page’s form as a preview of the request. Live desk submission comes in a later phase.",
    },
    {
      question: "Do you pick up from hotels in Koramangala?",
      answer:
        "Yes, if you give a hotel name or a stoppable point. We do not list a partner hotel programme.",
    },
    {
      question: "Is evening pickup a problem?",
      answer:
        "Evening congestion in the neighbourhood is real. Leave more buffer than a late-night run. We still do not publish minutes.",
    },
    {
      question: "Can I book a larger vehicle for my family?",
      answer:
        "Request SUV or Innova Crysta. Availability is confirmed when operations accept the trip.",
    },
    {
      question: "Will I get OTP login on this page?",
      answer:
        "Not in this phase. Phone and OTP authentication will arrive with the customer booking flow. This lander does not create an account.",
    },
  ],
  farePlaceholder: "Price on request",
};
