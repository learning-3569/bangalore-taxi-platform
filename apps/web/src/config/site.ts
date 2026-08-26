import { media } from "@/config/media";

export const siteConfig = {
  name: "Bengaluru Cabs",
  shortName: "Bengaluru Cabs",
  locale: "en-IN",
  defaultTitle: "Bengaluru Cabs | Bangalore Taxi Booking for Airport, Local and Outstation",
  titleTemplate: "%s | Bengaluru Cabs",
  description:
    "Book a Bangalore taxi for airport transfers, city rides, and outstation trips. Advance cab booking with Bengaluru Cabs, a locally operated fleet.",
  tagline: "Your comfort, our priority.",
} as const;

/** Set via NEXT_PUBLIC_SITE_URL. Production host is chosen in Phase 14. */
export function getSiteUrl(): string {
  return process.env.NEXT_PUBLIC_SITE_URL ?? "http://127.0.0.1:43121";
}

/** Local and unset hosts must not be advertised as indexable production URLs. */
export function isPublicIndexable(): boolean {
  try {
    const { hostname } = new URL(getSiteUrl());
    return hostname !== "127.0.0.1" && hostname !== "localhost";
  } catch {
    return false;
  }
}

/** WhatsApp digits only, no +. Leave empty until the business confirms a number. */
export function getWhatsAppNumber(): string {
  return process.env.NEXT_PUBLIC_WHATSAPP_NUMBER?.replace(/\D/g, "") ?? "";
}

export const navItems = [
  { href: "/", label: "Home" },
  { href: "/#services", label: "Taxi Services" },
  { href: "/airport-taxi-bangalore", label: "Airport Taxi" },
  { href: "/outstation-taxi-bangalore", label: "Outstation" },
  { href: "/#fleet", label: "Our Cars" },
  { href: "/#about", label: "About Us" },
  { href: "/#contact", label: "Contact Us" },
] as const;

export const legalPages = [
  { href: "/privacy-policy", label: "Privacy Policy" },
  { href: "/terms-and-conditions", label: "Terms & Conditions" },
] as const;

export const legalAndHomePaths = ["/", "/privacy-policy", "/terms-and-conditions"] as const;

export const tripTypes = [
  { value: "one-way", label: "One Way" },
  { value: "round-trip", label: "Round Trip" },
  { value: "airport", label: "Airport Transfer" },
  { value: "local", label: "Local Ride" },
] as const;

export const vehicleTypes = [
  { value: "sedan", label: "Sedan" },
  { value: "suv", label: "SUV" },
  { value: "innova", label: "Innova Crysta" },
  { value: "premium", label: "Premium" },
] as const;

export const heroSlides = [
  {
    id: "airport",
    eyebrow: "Bangalore airport taxi",
    title: "Airport taxis, without the scramble.",
    text: "Timed pickups and drops at Kempegowda International Airport. You send the flight window; we plan the car.",
    image: media.heroAirport,
  },
  {
    id: "city",
    eyebrow: "Local taxi service",
    title: "Across Bangalore, on your clock.",
    text: "Whitefield, Electronic City, Koramangala, the stations — city rides booked before you step out.",
    image: media.heroCity,
  },
  {
    id: "outstation",
    eyebrow: "Outstation taxi",
    title: "Mysore, Coorg, and the long road.",
    text: "One-way or round-trip cars for weekends and work trips out of Bengaluru. No marketplace bidding.",
    image: media.heroOutstation,
  },
  {
    id: "premium",
    eyebrow: "Premium taxi service",
    title: "Arrive settled, not squeezed.",
    text: "Ask for a quieter cabin when the trip is a client meeting, a late flight, or grandparents in the back.",
    image: media.heroPremium,
  },
] as const;

export const trustItems = [
  { title: "24/7 desk", text: "Night arrivals and early departures are normal work, not exceptions." },
  { title: "Assigned drivers", text: "A car is confirmed after the desk reviews the request — not a random street hail." },
  { title: "Clean cars", text: "Sedan, SUV, Innova, and premium categories. Exact models follow live inventory." },
  { title: "On-time airport runs", text: "Built around BLR pickups and drops, not generic city hopping." },
  { title: "Fares when they are ready", text: "No invented “from ₹999” stickers. Price on request until the engine is live." },
] as const;

export const services = [
  {
    title: "Airport taxi",
    description: "Pickup and drop at Kempegowda International Airport, booked before you fly.",
    href: "/airport-taxi-bangalore",
    image: media.airportFeature,
    featured: true,
  },
  {
    title: "Outstation taxi",
    description: "Intercity cars from Bangalore for family weekends and work travel.",
    href: "/outstation-taxi-bangalore",
    image: media.outstationFeature,
    featured: false,
  },
  {
    title: "Local taxi",
    description: "Point-to-point city rides across Bengaluru neighbourhoods and business parks.",
    href: "/#services",
    image: media.localService,
    featured: false,
  },
  {
    title: "One way taxi",
    description: "Keep the outward leg when you do not need a return car sitting idle.",
    href: "/#book",
    image: media.heroOutstation,
    featured: false,
  },
  {
    title: "Corporate taxi",
    description: "Advance cars for office travel. Billing process will be published when it exists.",
    href: "/#contact",
    image: media.corporateService,
    featured: false,
  },
  {
    title: "Outstation packages",
    description: "Round-trip planning for Mysore, Coorg, Ooty and similar corridors — pages only when the copy is unique.",
    href: "/#outstation",
    image: media.heroCity,
    featured: false,
  },
] as const;

export const fleet = [
  {
    name: "Sedan",
    seats: "4 passengers",
    luggage: "2 medium bags",
    description: "Airport drops and city rides with a small group.",
    image: media.fleetSedan,
    fare: "Price on request",
  },
  {
    name: "SUV",
    seats: "6–7 passengers",
    luggage: "Family luggage",
    description: "Extra height and boot space for families.",
    image: media.fleetSuv,
    fare: "Price on request",
  },
  {
    name: "Innova Crysta",
    seats: "6–7 passengers",
    luggage: "Group luggage",
    description: "The usual ask for outstation groups. Model is a category placeholder until the live list is confirmed.",
    image: media.fleetInnova,
    fare: "Price on request",
  },
  {
    name: "Premium",
    seats: "4 passengers",
    luggage: "2–3 bags",
    description: "A quieter cabin for business and late-night airport runs.",
    image: media.fleetPremium,
    fare: "Price on request",
  },
] as const;

export const popularRoutes = [
  { from: "Whitefield", to: "Bangalore Airport", href: "/whitefield-to-bangalore-airport-taxi" },
  { from: "Bangalore Airport", to: "Whitefield", href: "/bangalore-airport-to-whitefield-taxi" },
  { from: "Electronic City", to: "Bangalore Airport", href: "/electronic-city-to-bangalore-airport-taxi" },
  { from: "Koramangala", to: "Bangalore Airport", href: "/koramangala-to-bangalore-airport-taxi" },
  { from: "Bangalore", to: "Mysore", href: "/bangalore-to-mysore-taxi" },
  { from: "Bangalore", to: "Coorg", href: "/bangalore-to-coorg-taxi" },
  { from: "Bangalore", to: "Ooty" },
] as const;

export const outstationDestinations = [
  "Mysore",
  "Coorg",
  "Chennai",
  "Ooty",
  "Hyderabad",
] as const;

export const howItWorks = [
  {
    step: "1",
    title: "Enter your details",
    body: "Pickup, drop, date, time, and trip type.",
  },
  {
    step: "2",
    title: "Choose your car",
    body: "Sedan, SUV, Innova Crysta, or Premium.",
  },
  {
    step: "3",
    title: "Confirm the request",
    body: "The form is a preview today. It will reach the desk when booking APIs go live.",
  },
  {
    step: "4",
    title: "Ride when assigned",
    body: "Driver and car details follow after operations accept the trip. That message is not live yet.",
  },
] as const;

export const whyChooseUs = [
  {
    title: "Verified assignment",
    body: "A dispatcher reviews the request before a driver is attached.",
  },
  {
    title: "Airport-first planning",
    body: "BLR pickups and drops are a core product, not a side link.",
  },
  {
    title: "Clean, current cars",
    body: "Categories you can request now; named models when inventory is confirmed.",
  },
  {
    title: "Clear fare rules later",
    body: "No fake starting prices. The rate card will come from the pricing engine.",
  },
  {
    title: "A real Bangalore desk",
    body: "Local operation for city, airport, and outstation — not a pan-India app shell.",
  },
] as const;

export const exampleTestimonials = [
  {
    quote:
      "Sample layout only: “The airport pickup was on time and the car was clean.” Replace with a permissioned comment.",
    attribution: "Placeholder — not a live review",
  },
  {
    quote:
      "Sample layout only: “Mysore day trip, clear timing, no haggling at the kerb.” Replace with a verified quote.",
    attribution: "Placeholder — not a live review",
  },
  {
    quote:
      "Sample layout only: “Booked the night before an early flight.” Do not treat these as ratings.",
    attribution: "Placeholder — not a live review",
  },
] as const;

export const faqs = [
  {
    question: "How can I book a Bangalore taxi?",
    answer:
      "Use the booking form with pickup, drop, date, and time. Online submission to the desk will be enabled in a later phase. Until then, the form is a preview of the request flow.",
  },
  {
    question: "Do you provide airport taxi service?",
    answer:
      "Yes — scheduled pickups and drops for Kempegowda International Airport. Meeting points and waiting rules will be published with the live booking flow.",
  },
  {
    question: "Can I book a taxi in advance?",
    answer:
      "That is the intended model: request, desk confirmation, then driver details. Timelines will be stated when booking opens.",
  },
  {
    question: "Do you provide outstation taxis?",
    answer:
      "Yes, for journeys out of Bangalore such as Mysore, Coorg, Chennai, and Ooty. Dedicated route pages appear only when each has unique content.",
  },
  {
    question: "Can I choose my vehicle?",
    answer:
      "You can request Sedan, SUV, Innova Crysta, or Premium. Assignment depends on availability at the requested time.",
  },
  {
    question: "How will I receive driver details?",
    answer:
      "After a booking is accepted, driver and vehicle details will go out on the channel we publish at launch (SMS or WhatsApp). That channel is not connected yet.",
  },
  {
    question: "Can I book without creating an account?",
    answer:
      "Guest booking is not part of V1. Phone verification will be required as part of the booking flow. That step is not on this website yet.",
  },
] as const;

export const businessPlaceholders = {
  phone: "Public phone number pending business confirmation",
  email: "Public email pending business confirmation",
  address: "Registered address pending business confirmation",
  hours: "Operating hours pending business confirmation",
  whatsapp: "WhatsApp number pending business confirmation",
} as const;
