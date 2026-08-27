import { media } from "@/config/media";
import type { RoutePageContent } from "@/content/seo/types";

export const airportToWhitefield: RoutePageContent = {
  slug: "bangalore-airport-to-whitefield-taxi",
  published: true,
  indexable: true,
  lastUpdated: "2026-08-26",
  originId: "blr-airport",
  destinationId: "whitefield",
  routeType: "airport",
  direction: "from-airport",
  seoTitle: "Bangalore Airport to Whitefield Taxi",
  metaDescription:
    "Airport pickup at Kempegowda International Airport for drops into Whitefield. Advance cab request with Bengaluru Cabs — fare on request.",
  h1: "Bangalore Airport to Whitefield taxi",
  heroEyebrow: "Airport pickup",
  heroText:
    "Land at BLR and continue to Whitefield with a car arranged around your arrival, not a queue you discover after baggage.",
  heroImage: media.airportFeature,
  primaryCtaLabel: "Book this pickup",
  bookingHeading: "Book Airport → Whitefield taxi",
  bookingSubmitLabel: "Request this pickup",
  defaultTripType: "airport",
  summary: {
    from: "Kempegowda International Airport (BLR)",
    to: "Whitefield",
    tripType: "Airport pickup",
    vehicleCategories: "Sedan, SUV, Innova Crysta, Premium",
    distanceNote: "Distance varies with the route taken.",
    durationNote: "Approximate information — final journey time depends on traffic.",
  },
  intro:
    "Arrivals are a different job from a home-to-airport drop. Bags come off the belt late, international queues stretch, and you may walk out at an hour when Whitefield gates are quieter. This page is for people landing at Kempegowda International Airport and heading to an east-Bangalore home, hotel, or office. We do not sell a meet-and-greet product we have not defined, and we do not quote a fixed drive time.",
  pickupInformation: {
    heading: "Meeting you at the airport",
    body: "Share the flight number and whether you are arriving domestic or international so the desk can plan. Pickup is at the passenger terminal area used for taxis. Meeting instructions are shared with an accepted request. We do not operate an in-terminal lounge or a named Fast Track lane. If the flight is delayed, tell the desk; waiting is handled with your trip, not as a published free-wait number here.",
  },
  destinationInformation: {
    heading: "Drop in Whitefield",
    body: "Give an apartment complex, hotel, or campus gate. Night arrivals into gated communities sometimes need a gate pass or a resident’s name — include that in your request. If you are going straight to an office park, mention the building so the driver is not sent to the wrong ITPL entrance.",
  },
  travelGuidance: {
    heading: "After you land",
    body: "Collect luggage before you look for the car. Late-night and early-morning arrivals are common on this corridor. If you have oversize bags or a child seat request, say so up front; we only assign what the fleet actually has. Traffic toward Whitefield can thicken on weekday evenings even if the airport road felt empty when you left the terminal.",
  },
  whyChoose: [
    {
      title: "Arrival-led planning",
      body: "The request is built around landing, not a generic “city taxi” timeslot.",
    },
    {
      title: "Luggage is part of the brief",
      body: "Tell us about check-in bags so the desk does not send a tight sedan by default.",
    },
    {
      title: "East Bangalore drop",
      body: "Whitefield gates and campuses are a normal destination for this desk.",
    },
  ],
  howBookingWorks: [
    { title: "Flight and drop", body: "Airport pickup, Whitefield destination, date, and expected landing window." },
    { title: "Choose a category", body: "Match boot space to the bags coming off the belt." },
    { title: "Desk review", body: "Airport pickups stay pending confirmation until operations accept the request." },
    { title: "Driver details", body: "Shared after the trip is accepted — not when you first submit the request." },
  ],
  vehicleNotes: [
    { category: "Sedan", note: "Works for a solo traveller with a suitcase and a backpack." },
    { category: "SUV", note: "Use when two or three large cases need to lie flat." },
    { category: "Innova Crysta", note: "Comfortable for a family landing together." },
    { category: "Premium", note: "Ask when you want a quieter cabin after a long haul." },
  ],
  relatedSlugs: [
    "whitefield-to-bangalore-airport-taxi",
    "electronic-city-to-bangalore-airport-taxi",
    "koramangala-to-bangalore-airport-taxi",
  ],
  parentServiceId: "airport-taxi-bangalore",
  faq: [
    {
      question: "Do you pick up at Bangalore Airport for Whitefield?",
      answer:
        "Yes. This page is for inbound trips from Kempegowda International Airport to Whitefield. Meeting instructions are shared with an accepted request.",
    },
    {
      question: "What if my flight is delayed?",
      answer:
        "Share the flight number when you can. Waiting and delay handling are set with your accepted trip; we do not publish a free-wait number here.",
    },
    {
      question: "Can I book a larger vehicle after a family landing?",
      answer:
        "Request an SUV or Innova Crysta on the form. Assignment still depends on what the desk has at that hour.",
    },
    {
      question: "How will I recognise the driver?",
      answer:
        "You receive driver and vehicle details after the booking is accepted. A submitted request is still pending confirmation until then.",
    },
    {
      question: "Is payment taken in the car?",
      answer:
        "Online payment is not taken on this website. How you settle an accepted trip is shared with that booking. Do not assume a fare from this page.",
    },
  ],
  farePlaceholder: "Price on request",
};
