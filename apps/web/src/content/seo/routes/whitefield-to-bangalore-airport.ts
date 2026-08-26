import { media } from "@/config/media";
import type { RoutePageContent } from "@/content/seo/types";

export const whitefieldToAirport: RoutePageContent = {
  slug: "whitefield-to-bangalore-airport-taxi",
  published: true,
  indexable: true,
  lastUpdated: "2026-08-26",
  originId: "whitefield",
  destinationId: "blr-airport",
  routeType: "airport",
  direction: "to-airport",
  seoTitle: "Whitefield to Bangalore Airport Taxi",
  metaDescription:
    "Advance taxi from Whitefield to Kempegowda International Airport. Book a sedan, SUV, or Innova for your flight drop with Bengaluru Cabs.",
  h1: "Whitefield to Bangalore Airport taxi",
  heroEyebrow: "Airport drop",
  heroText:
    "Leave from ITPL, Hope Farm, or a Whitefield hotel with a car already assigned to your flight window — not a last-minute kerb hunt.",
  heroImage: media.heroAirport,
  primaryCtaLabel: "Book this route",
  bookingHeading: "Book Whitefield → Airport taxi",
  bookingSubmitLabel: "Request this drop",
  defaultTripType: "airport",
  summary: {
    from: "Whitefield",
    to: "Kempegowda International Airport (BLR)",
    tripType: "Airport drop",
    vehicleCategories: "Sedan, SUV, Innova Crysta, Premium",
    distanceNote: "Exact kilometres will appear when mapping is connected.",
    durationNote: "Approximate information — final journey time depends on traffic.",
  },
  intro:
    "Whitefield sits on Bangalore’s east side. A flight drop from here is mostly about leaving enough buffer for Outer Ring Road and airport-approach congestion, not about memorising a published minute-count. Bengaluru Cabs takes the request in advance, a dispatcher reviews it, and a car is assigned when operations confirm the trip. Fares are not listed on this page; ask for a quote when booking opens.",
  pickupInformation: {
    heading: "Pickup in Whitefield",
    body: "Share a pin, apartment name, or tech-park gate. Whitefield campuses often have separate visitor and cab entries — tell us which gate the driver should use. If you are leaving from a hotel, the lobby name is enough. We do not currently publish a guaranteed waiting window at the pickup; that rule will come with the live booking policy.",
  },
  destinationInformation: {
    heading: "Drop at Bangalore Airport",
    body: "Drops use Kempegowda International Airport’s passenger terminals. We do not claim special kerb rights, fast-track access, or airline partnerships. Follow the terminal signs for your airline. Build extra time for security and check-in; those are airport processes, not taxi ones.",
  },
  travelGuidance: {
    heading: "Planning the departure",
    body: "East Bangalore traffic is heavier on weekday mornings and on Friday evenings. Early flights mean leaving when the city is quieter, but you should still treat ORR incidents as possible. If you are travelling with check-in bags plus a laptop backpack, say so when you request the car so the desk can match boot space. Night drops are ordinary work for this desk, not a surcharge we invent here — any night rule will come from the pricing engine later.",
  },
  whyChoose: [
    {
      title: "Assigned, not hailed",
      body: "A dispatcher reviews the Whitefield pickup before a driver is attached.",
    },
    {
      title: "Flight window first",
      body: "You tell us the departure time; we do not guess it from a generic city average.",
    },
    {
      title: "Vehicle for the bags",
      body: "Request a sedan for two people or an SUV/Innova when the boot needs to work harder.",
    },
  ],
  howBookingWorks: [
    { title: "Send the route", body: "Whitefield pickup, airport drop, date, and time." },
    { title: "Choose a category", body: "Sedan, SUV, Innova Crysta, or Premium — subject to availability." },
    { title: "Desk review", body: "Online submit is a preview today. Live requests will reach operations later." },
    { title: "Driver details", body: "After acceptance, vehicle and driver information will go out on the channel we publish at launch." },
  ],
  vehicleNotes: [
    { category: "Sedan", note: "Sensible default for one or two travellers with cabin bags." },
    { category: "SUV", note: "Better when you have family or several check-in cases." },
    { category: "Innova Crysta", note: "Ask for this category when the group wants a taller cabin." },
    { category: "Premium", note: "Quieter cabin for a client flight or a very early start." },
  ],
  relatedSlugs: [
    "bangalore-airport-to-whitefield-taxi",
    "electronic-city-to-bangalore-airport-taxi",
    "koramangala-to-bangalore-airport-taxi",
  ],
  parentServiceId: "airport-taxi-bangalore",
  faq: [
    {
      question: "Can I book a Whitefield to airport taxi in advance?",
      answer:
        "Yes — advance request is the intended model. The form on this page is a preview until booking APIs go live. A dispatcher will then accept the trip before a driver is assigned.",
    },
    {
      question: "What vehicle should I choose for airport luggage?",
      answer:
        "Two cabin bags usually fit a sedan. Families or several large check-in cases should request an SUV or Innova Crysta. Exact boot layouts depend on the car assigned from live inventory.",
    },
    {
      question: "Do you provide early-morning airport drops from Whitefield?",
      answer:
        "Early departures are a normal airport-drop request. Share the pickup time with the flight window so the desk can plan. We do not publish a cut-off time on this page.",
    },
    {
      question: "How will I receive driver details?",
      answer:
        "After operations accept the booking, driver and car details will be sent on the channel we confirm at launch (SMS or WhatsApp). That channel is not connected yet.",
    },
    {
      question: "Will I need to log in to book?",
      answer:
        "Customer booking will later use phone number and OTP. Guest checkout is not part of V1. This page does not collect an account today.",
    },
  ],
  farePlaceholder: "Price on request",
};
