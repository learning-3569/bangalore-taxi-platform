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
    distanceNote: "Exact kilometres will appear when mapping is connected.",
    durationNote: "Approximate information — final journey time depends on traffic.",
  },
  intro:
    "Arrivals are a different job from a home-to-airport drop. Bags come off the belt late, international queues stretch, and you may walk out at an hour when Whitefield gates are quieter. This page is for people landing at Kempegowda International Airport and heading to an east-Bangalore home, hotel, or office. We do not sell a meet-and-greet product we have not defined, and we do not quote a fixed drive time.",
  pickupInformation: {
    heading: "Meeting you at the airport",
    body: "Share the flight number and whether you are arriving domestic or international so the desk can plan. Pickup is at the passenger terminal area used for taxis — we will publish a precise meeting instruction with the live booking flow. We do not currently operate an in-terminal lounge or a named Fast Track lane. If the flight is delayed, tell the desk; waiting rules will be written into policy later rather than invented here.",
  },
  destinationInformation: {
    heading: "Drop in Whitefield",
    body: "Give an apartment complex, hotel, or campus gate. Night arrivals into gated communities sometimes need a gate pass or a resident’s name — add that in the request notes when the form supports it. If you are going straight to an office park, mention the building so the driver is not sent to the wrong ITPL entrance.",
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
    { title: "Desk review", body: "The form is a preview. Live pickup requests will reach operations later." },
    { title: "Driver details", body: "Sent after the trip is accepted, on the launch messaging channel." },
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
        "Yes. This page is for inbound trips from Kempegowda International Airport to Whitefield. Precise kerb instructions will be published with live booking.",
    },
    {
      question: "What if my flight is delayed?",
      answer:
        "Share the flight number when you can. Waiting and delay handling will be part of the booking policy; we do not state a free-wait number here because it is not approved yet.",
    },
    {
      question: "Can I book a larger vehicle after a family landing?",
      answer:
        "Request an SUV or Innova Crysta on the form. Assignment still depends on what the desk has at that hour.",
    },
    {
      question: "How will I recognise the driver?",
      answer:
        "You will receive driver and vehicle details after the booking is accepted. Until messaging is live, this page cannot promise a particular app or SMS format.",
    },
    {
      question: "Is payment taken in the car?",
      answer:
        "Online payment is not on this website. How you settle a confirmed trip will be published with the booking launch. Do not assume a fare from this page.",
    },
  ],
  farePlaceholder: "Price on request",
};
