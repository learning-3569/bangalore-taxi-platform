import { media } from "@/config/media";
import type { RoutePageContent } from "@/content/seo/types";

export const electronicCityToAirport: RoutePageContent = {
  slug: "electronic-city-to-bangalore-airport-taxi",
  published: true,
  indexable: true,
  lastUpdated: "2026-08-26",
  originId: "electronic-city",
  destinationId: "blr-airport",
  routeType: "airport",
  direction: "to-airport",
  seoTitle: "Electronic City to Bangalore Airport Taxi",
  metaDescription:
    "Taxi from Electronic City to Kempegowda International Airport. Advance airport drop across south Bangalore with Bengaluru Cabs.",
  h1: "Electronic City to Bangalore Airport taxi",
  heroEyebrow: "South Bangalore airport drop",
  heroText:
    "Campus gates in Electronic City Phase 1 or Phase 2, timed for a flight at BLR — a long northbound run, not a neighbourhood hop.",
  heroImage: media.heroAirport,
  primaryCtaLabel: "Book this route",
  bookingHeading: "Book Electronic City → Airport taxi",
  bookingSubmitLabel: "Request this drop",
  defaultTripType: "airport",
  summary: {
    from: "Electronic City",
    to: "Kempegowda International Airport (BLR)",
    tripType: "Airport drop",
    vehicleCategories: "Sedan, SUV, Innova Crysta, Premium",
    distanceNote: "Distance varies with the route taken.",
    durationNote: "Approximate information — final journey time depends on traffic.",
  },
  intro:
    "Electronic City sits on Bangalore’s south side. Reaching Kempegowda International Airport from here crosses a large part of the city, so the useful advice is about buffers and pickup clarity, not a single “usual time.” Phase 1 and Phase 2 campuses use different gates; a driver sent to the wrong phase wastes the only buffer you had. Bengaluru Cabs treats this as an airport-drop product: request early, the desk reviews it, then a car is assigned when the trip is accepted.",
  pickupInformation: {
    heading: "Pickup in Electronic City",
    body: "Name the phase, building, and gate. Many offices have cab lay-bys that are not the same as the visitor parking. If you are leaving from a hostel or apartment on Hosur Road rather than a campus, say so — it changes where the car should wait. We will not invent a “standard Electronic City pickup point.”",
  },
  destinationInformation: {
    heading: "Drop at Bangalore Airport",
    body: "The drop is the passenger terminal for your airline. We do not have unpublished airport concessions to advertise. Allow time after the car drop for check-in and security. If you have an unusually early flight, mention it so the desk is not treating this like a midday city ride.",
  },
  travelGuidance: {
    heading: "South-to-north planning",
    body: "Hosur Road and the connectors toward the airport corridor get slow in weekday peaks. A delayed start in Electronic City is harder to recover than a delayed start in Whitefield because the remaining distance is still long. Pack as you would for any airport run; if the group includes colleagues with large cases, request an SUV instead of squeezing a sedan.",
  },
  whyChoose: [
    {
      title: "Phase-aware pickup",
      body: "You tell us Phase 1, Phase 2, or Hosur Road — we do not assume one campus.",
    },
    {
      title: "Longer-city buffer",
      body: "This corridor needs more schedule humility than a short east-side hop.",
    },
    {
      title: "Same desk as other airport work",
      body: "It is still a BLR drop, reviewed by operations before assignment.",
    },
  ],
  howBookingWorks: [
    { title: "Pin the campus", body: "Electronic City pickup details, airport drop, date, and time." },
    { title: "Choose a category", body: "Sedan for light bags; SUV or Innova when the boot must work." },
    { title: "Desk review", body: "Electronic City drops stay pending confirmation until operations accept the request." },
    { title: "Driver details", body: "Shared after acceptance — not at the moment you submit the form." },
  ],
  vehicleNotes: [
    { category: "Sedan", note: "Fine for a couple of cabin bags and a south-Bangalore start." },
    { category: "SUV", note: "Better for colleagues travelling together with check-in luggage." },
    { category: "Innova Crysta", note: "Ask when you want a taller cabin for the longer city crossing." },
    { category: "Premium", note: "Useful for a client flight leaving from the south side." },
  ],
  relatedSlugs: [
    "whitefield-to-bangalore-airport-taxi",
    "koramangala-to-bangalore-airport-taxi",
    "bangalore-airport-to-whitefield-taxi",
  ],
  parentServiceId: "airport-taxi-bangalore",
  faq: [
    {
      question: "Can I book an airport taxi from Electronic City in advance?",
      answer:
        "Yes. Request the drop in advance. You'll verify your mobile number before completing the booking request. Our team reviews availability before a driver is assigned.",
    },
    {
      question: "Do you pick up from both Phase 1 and Phase 2?",
      answer:
        "We can pick up anywhere in Electronic City you describe. Write the phase and gate so the driver is not sent to the wrong campus.",
    },
    {
      question: "Is this a short hop compared with Whitefield?",
      answer:
        "No. Electronic City to BLR is a long city crossing. Leave more buffer than you would for a neighbourhood ride. We still do not publish a minute count.",
    },
    {
      question: "Can I choose a larger car for office colleagues?",
      answer:
        "Request SUV or Innova Crysta. The desk assigns from available cars at the requested time.",
    },
    {
      question: "How do fares work on this route?",
      answer:
        "Fares are not listed. Price on request. Anything you see as “from ₹…” on another site is not our rate card.",
    },
  ],
  farePlaceholder: "Price on request",
};
