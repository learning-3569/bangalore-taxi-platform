import { media } from "@/config/media";
import type { RoutePageContent } from "@/content/seo/types";

export const bangaloreToCoorg: RoutePageContent = {
  slug: "bangalore-to-coorg-taxi",
  published: true,
  indexable: true,
  lastUpdated: "2026-08-26",
  originId: "bangalore",
  destinationId: "coorg",
  routeType: "outstation",
  direction: "outstation-outbound",
  seoTitle: "Bangalore to Coorg Taxi",
  metaDescription:
    "Outstation taxi from Bangalore to Coorg (Kodagu). Hill-road cars for weekends and family travel with Bengaluru Cabs. Fare on request.",
  h1: "Bangalore to Coorg taxi",
  heroEyebrow: "Hill-country outstation",
  heroText:
    "A car from Bangalore into Kodagu for a weekend in the hills — packed for winding roads, not for an airport terminal.",
  heroImage: media.outstationFeature,
  primaryCtaLabel: "Book this route",
  bookingHeading: "Book Bangalore → Coorg taxi",
  bookingSubmitLabel: "Request this trip",
  defaultTripType: "round-trip",
  summary: {
    from: "Bangalore",
    to: "Coorg (Kodagu)",
    tripType: "Outstation — typically round-trip",
    vehicleCategories: "Sedan, SUV, Innova Crysta, Premium",
    distanceNote: "Exact kilometres will appear when mapping is connected.",
    durationNote: "Approximate information — final journey time depends on traffic and hill-road conditions.",
  },
  intro:
    "Coorg is a longer, hillier outstation than Mysore. Weekends fill with families, and the last stretch is not a straight highway. This page is for people who want a dedicated car into Kodagu — often staying a night or two — rather than an airport transfer with the place names swapped. We do not sell a monsoon “guarantee,” a homestay partnership, or a fixed drive time. Weather and ghat traffic change the day; the desk assigns a car, not a brochure schedule.",
  pickupInformation: {
    heading: "Leaving Bangalore",
    body: "Most groups leave early so more of the day is spent in Coorg rather than on the approach. Pickup is wherever you specify in Bangalore. If you are meeting relatives at a second stop inside the city, say so; extra city legs belong in the request, not as a surprise for the driver.",
  },
  destinationInformation: {
    heading: "Where in Coorg",
    body: "Kodagu is not a single gate. Madikeri, a specific resort, or a family estate are different drops. Write the stay name and a pin if you have one. We do not include sightseeing circuits unless you request the car for that duty as round-trip or multi-day — and those rules will be written later, not implied here.",
  },
  travelGuidance: {
    heading: "Hills, bags, and the return",
    body: "Round-trip is the common pattern: the same car brings you back after the stay. One-way is possible if you continue elsewhere. Choose a taller cabin if anyone is prone to motion on ghats, and pack bags so they can be lashed in a boot rather than stacked on laps. Night driving in the hills is a planning choice you make with the desk — we do not advertise it as a thrill product.",
  },
  whyChoose: [
    {
      title: "Written for the hills",
      body: "Ghat roads and stay pins, not terminal meeting points.",
    },
    {
      title: "Weekend groups",
      body: "Innova and SUV requests are normal when the cabin is full.",
    },
    {
      title: "Return planned on purpose",
      body: "Round-trip is a first-class option on this corridor, not an afterthought.",
    },
  ],
  howBookingWorks: [
    { title: "Set the stay", body: "Bangalore pickup, Coorg drop pin, dates, and one-way or round-trip." },
    { title: "Choose a category", body: "SUV or Innova is the usual family ask for this road." },
    { title: "Desk review", body: "Preview form; live outstation intake later." },
    { title: "Driver details", body: "After the trip is accepted." },
  ],
  vehicleNotes: [
    { category: "Sedan", note: "Possible for two adults with light bags; less ideal if the group grows." },
    { category: "SUV", note: "A solid default for couples plus luggage on hill roads." },
    { category: "Innova Crysta", note: "Typical for a full family weekend." },
    { category: "Premium", note: "When you want a calmer cabin and can accept a smaller boot." },
  ],
  relatedSlugs: ["bangalore-to-mysore-taxi", "electronic-city-to-bangalore-airport-taxi"],
  parentServiceId: "outstation-taxi-bangalore",
  faq: [
    {
      question: "Is Bangalore to Coorg booked as a round-trip?",
      answer:
        "Often yes, because most travellers return to Bangalore after the stay. Choose round-trip when the same car should come back with you. Choose one-way if you are not returning with us.",
    },
    {
      question: "Can I book only the onward taxi?",
      answer:
        "Yes — use one-way. Tell us the Coorg drop clearly so the driver is not sent to a generic town centre.",
    },
    {
      question: "What vehicle is better on hill roads?",
      answer:
        "Many families request an SUV or Innova Crysta for seat height and luggage. A sedan can still be requested for a pair with light bags.",
    },
    {
      question: "Do you include sightseeing in Coorg?",
      answer:
        "Not automatically. Local running, if offered later, will have its own rules. This page is the Bangalore–Coorg car, not a tour package.",
    },
    {
      question: "Why is there no travel time listed?",
      answer:
        "Hill traffic and weather vary. We will show curated or mapped times only when a source exists. Until then we state that journey time depends on conditions.",
    },
  ],
  farePlaceholder: "Price on request",
};
