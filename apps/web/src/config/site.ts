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

function envFlagEnabled(name: string): boolean {
  const value = process.env[name]?.trim().toLowerCase();
  return value === "true" || value === "1";
}

/**
 * Crawlers may index only when explicitly enabled.
 * Development, testing, staging, and preview hosts stay noindex unless INDEX_PUBLIC=true.
 * Do not infer this from hostname.
 */
export function isPublicIndexable(): boolean {
  return envFlagEnabled("INDEX_PUBLIC") || envFlagEnabled("NEXT_PUBLIC_INDEX_PUBLIC");
}

/** Flip to false when approved Privacy/Terms copy replaces LegalPlaceholder. */
export const legalPagesArePlaceholders = true;

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
  { title: "Assigned drivers", text: "A car is assigned after the desk reviews your request — not a random street hail." },
  { title: "Clean cars", text: "Sedan, SUV, Innova, and premium categories. Exact models depend on availability." },
  { title: "On-time airport runs", text: "Built around BLR pickups and drops, not generic city hopping." },
  { title: "Fares on request", text: "No invented “from ₹999” stickers. Price on request for each trip." },
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
    description: "Advance cars for office travel. Billing is arranged with your booking.",
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
    description: "A roomy option for outstation groups.",
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
    title: "Submit your request",
    body: "You'll verify your mobile number, then send the request. Status stays pending confirmation until our team accepts it.",
  },
  {
    step: "4",
    title: "Ride when assigned",
    body: "Driver and car details are shared after operations accept the trip — not when you first submit the request.",
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
    body: "Categories you can request now; named models depend on availability.",
  },
  {
    title: "Honest fares",
    body: "Price on request. We don't publish invented starting prices.",
  },
  {
    title: "A real Bangalore desk",
    body: "Local operation for city, airport, and outstation — not a pan-India app shell.",
  },
] as const;

export const faqs = [
  {
    question: "How can I book a Bangalore taxi?",
    answer:
      "Enter your trip details to request a cab. You'll verify your mobile number before completing your booking request. Our team then reviews availability.",
  },
  {
    question: "Do you provide airport taxi service?",
    answer:
      "Yes — scheduled pickups and drops for Kempegowda International Airport. Share your flight window with the request so the desk can plan.",
  },
  {
    question: "Can I book a taxi in advance?",
    answer:
      "Yes. Request the trip ahead of time. A submitted request is pending confirmation until operations accept it.",
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
      "After operations accept the booking, we share driver and vehicle details. Submitting a request is not the same as a confirmed trip.",
  },
  {
    question: "Can I book without creating an account?",
    answer:
      "You'll verify your mobile number with OTP before completing a booking request. Guest checkout isn't available.",
  },
] as const;

export const businessPlaceholders = {
  phone: "Public phone number pending business confirmation",
  email: "Public email pending business confirmation",
  address: "Registered address pending business confirmation",
  hours: "Operating hours pending business confirmation",
  whatsapp: "WhatsApp number pending business confirmation",
} as const;
