import { media } from "@/config/media";
import type { RoutePageContent } from "@/content/seo/types";

type AirportEditorial = {
  slug: string; originId: string; destinationId: string; direction: "to-airport" | "from-airport";
  locality: string; seoTitle: string; metaDescription: string; h1: string; heroText: string;
  intro: string; pickupHeading: string; pickupBody: string; destinationHeading: string; destinationBody: string;
  guidanceHeading: string; guidanceBody: string; vehicleNotes: RoutePageContent["vehicleNotes"];
  faq: RoutePageContent["faq"]; relatedSlugs: readonly string[]; targetQueries: readonly string[];
};

function airportRoute(route: AirportEditorial): RoutePageContent {
  const inbound = route.direction === "from-airport";
  return {
    slug: route.slug, published: true, indexable: true, lastUpdated: "2026-08-27",
    originId: route.originId, destinationId: route.destinationId, routeType: "airport", direction: route.direction,
    seoTitle: route.seoTitle, metaDescription: route.metaDescription, h1: route.h1,
    heroEyebrow: inbound ? "Airport pickup" : "Airport drop", heroText: route.heroText,
    heroImage: inbound ? media.airportFeature : media.heroAirport,
    primaryCtaLabel: inbound ? "Book this pickup" : "Book this route",
    bookingHeading: inbound ? `Book Airport → ${route.locality} taxi` : `Book ${route.locality} → Airport taxi`,
    bookingSubmitLabel: inbound ? "Request this pickup" : "Request this drop", defaultTripType: "airport",
    summary: {
      from: inbound ? "Kempegowda International Airport (BLR)" : route.locality,
      to: inbound ? route.locality : "Kempegowda International Airport (BLR)", tripType: inbound ? "Airport pickup" : "Airport drop",
      vehicleCategories: "Sedan, SUV, Innova Crysta, Premium", distanceNote: "Distance varies with the route taken.",
      durationNote: "Travel time depends on traffic, pickup access, and airport conditions.",
    },
    intro: route.intro,
    pickupInformation: { heading: route.pickupHeading, body: route.pickupBody },
    destinationInformation: { heading: route.destinationHeading, body: route.destinationBody },
    travelGuidance: { heading: route.guidanceHeading, body: route.guidanceBody },
    whyChoose: [
      { title: "Advance request", body: "The desk reviews the route and requested time before any car is confirmed." },
      { title: "Clear meeting point", body: route.pickupBody },
      { title: "Luggage-aware choice", body: "Choose a vehicle category for the passengers and bags you actually have." },
    ],
    howBookingWorks: [
      { title: "Enter the route", body: `${route.locality}, airport direction, date, and pickup time.` },
      { title: "Choose the vehicle", body: "Request Sedan, SUV, Innova Crysta, or Premium." },
      { title: "Pending review", body: "Submission creates a booking request, not a confirmed trip." },
      { title: "Receive details", body: "Driver and vehicle details follow only after operations accepts the request." },
    ],
    vehicleNotes: route.vehicleNotes, relatedSlugs: route.relatedSlugs,
    parentServiceId: "airport-taxi-bangalore", faq: route.faq, farePlaceholder: "Price on request",
    targetQueries: route.targetQueries,
  };
}

export const hsrLayoutToAirport = airportRoute({
  slug: "hsr-layout-to-bangalore-airport-taxi", originId: "hsr-layout", destinationId: "blr-airport", direction: "to-airport", locality: "HSR Layout",
  seoTitle: "HSR Layout to Bangalore Airport Taxi", metaDescription: "Request an advance airport taxi from HSR Layout to BLR, with pickup details reviewed before confirmation.",
  h1: "HSR Layout to Bangalore Airport taxi", heroText: "Start from the correct HSR sector or apartment gate with a flight-ready airport drop request.",
  intro: "HSR Layout has numbered sectors, busy high streets, and residential lanes where a vague pickup can put a car on the wrong side of a divider. An airport request should name the sector and nearest usable gate. From there, the route joins larger south-east corridors whose traffic changes sharply by time of day, so this page offers planning guidance rather than a promised duration.",
  pickupHeading: "Name the HSR sector", pickupBody: "Include the sector, apartment or office name, and a gate that a cab can legally reach. A café landmark helps only when it identifies the correct side of the road.",
  destinationHeading: "Flight drop at BLR", destinationBody: "The booking endpoint is Kempegowda International Airport. Confirm your airline terminal independently and keep check-in and security time outside the road buffer.",
  guidanceHeading: "Plan beyond the neighbourhood exit", guidanceBody: "Silk Board-side congestion can affect the first part of the trip before the airport corridor begins. For several check-in cases, choose an SUV or Innova rather than counting only passenger seats.",
  vehicleNotes: [{ category: "Sedan", note: "Good for a solo traveller or couple leaving an HSR residence." }, { category: "SUV", note: "Useful when airport luggage fills more space than the passenger count suggests." }],
  faq: [{ question: "Can the cab collect me inside an HSR apartment complex?", answer: "Provide the accessible gate and tower. Entry depends on the property’s security process, so do not rely on the locality name alone." }, { question: "Is an HSR airport request confirmed immediately?", answer: "No. Submission creates a pending request. Operations confirms it after reviewing availability." }, { question: "Is the fare shown online?", answer: "No. Price on request; this page does not publish a fabricated route fare." }],
  relatedSlugs: ["koramangala-to-bangalore-airport-taxi", "bellandur-to-bangalore-airport-taxi", "electronic-city-to-bangalore-airport-taxi"],
  targetQueries: ["hsr layout to airport cab", "hsr to bangalore airport taxi", "hsr airport drop"],
});

export const bellandurToAirport = airportRoute({
  slug: "bellandur-to-bangalore-airport-taxi", originId: "bellandur", destinationId: "blr-airport", direction: "to-airport", locality: "Bellandur",
  seoTitle: "Bellandur to Bangalore Airport Taxi", metaDescription: "Advance Bellandur to Bangalore Airport taxi requests for homes, offices, and hotels around the ORR corridor.",
  h1: "Bellandur to Bangalore Airport taxi", heroText: "Coordinate an ORR-side pickup before beginning the northbound airport run.",
  intro: "Bellandur pickups can mean an apartment near the lake, an office campus on Outer Ring Road, or a hotel reached from a service lane. Those are different meeting points even when they share a postcode. A useful airport plan starts with the exact access road and enough time for the ORR segment before the longer run toward BLR.",
  pickupHeading: "ORR and service-road access", pickupBody: "State whether the entrance faces the main carriageway or a service road, and include the campus or apartment gate. Large offices may have a separate cab bay.",
  destinationHeading: "Airport terminal drop", destinationBody: "The fixed destination is Kempegowda International Airport (BLR). Terminal access follows airport signs and normal public kerb rules; no special privilege is claimed.",
  guidanceHeading: "Leave room for ORR variability", guidanceBody: "Weekday office peaks can slow Bellandur before the route clears the tech corridor. An Innova can be more practical than a sedan when colleagues share the trip with several laptop and check-in bags.",
  vehicleNotes: [{ category: "Sedan", note: "A practical choice for one or two travellers from a Bellandur apartment." }, { category: "Innova Crysta", note: "More comfortable for an office group carrying mixed work and flight luggage." }],
  faq: [{ question: "Can pickup be from a Bellandur tech park?", answer: "Yes. Give the campus and authorised cab gate so the meeting point is unambiguous." }, { question: "Do you guarantee an ORR travel time?", answer: "No. Traffic changes with office peaks, incidents, and weather; plan a suitable buffer." }, { question: "When is the booking confirmed?", answer: "Only after operations accepts the pending request and shares confirmation." }],
  relatedSlugs: ["marathahalli-to-bangalore-airport-taxi", "sarjapur-road-to-bangalore-airport-taxi", "whitefield-to-bangalore-airport-taxi"],
  targetQueries: ["bellandur to airport taxi", "bellandur airport cab", "bellandur airport drop"],
});

export const sarjapurRoadToAirport = airportRoute({
  slug: "sarjapur-road-to-bangalore-airport-taxi", originId: "sarjapur-road", destinationId: "blr-airport", direction: "to-airport", locality: "Sarjapur Road",
  seoTitle: "Sarjapur Road to Bangalore Airport Taxi", metaDescription: "Book an advance taxi request from Sarjapur Road communities and offices to Kempegowda International Airport.",
  h1: "Sarjapur Road to Bangalore Airport taxi", heroText: "Set a precise community or office pickup on the long Sarjapur Road corridor before heading to BLR.",
  intro: "Sarjapur Road is a corridor, not a single junction. A pickup near Iblur, a large residential township, and a location farther toward Dommasandra need different access instructions. Sharing the property name and nearest connecting road prevents a misplaced start and helps the desk review an airport drop realistically.",
  pickupHeading: "Pin the right part of Sarjapur Road", pickupBody: "Add the community, office, or hotel name and its main gate. If navigation commonly uses a nearby crossroad, include it without replacing the actual address.",
  destinationHeading: "BLR passenger drop", destinationBody: "The route ends at Kempegowda International Airport (BLR). Choose your terminal from airline information rather than assuming the taxi desk tracks flights.",
  guidanceHeading: "A corridor with several bottlenecks", guidanceBody: "The trip may encounter local junction traffic before reaching a faster northbound section. Families with prams or multiple large cases should request boot space deliberately.",
  vehicleNotes: [{ category: "SUV", note: "Helpful for a family leaving a gated community with full-size cases." }, { category: "Premium", note: "An option for a business traveller starting from a Sarjapur Road office." }],
  faq: [{ question: "Where should I set pickup on Sarjapur Road?", answer: "Use the exact property and gate. Sarjapur Road covers too much ground for the road name alone." }, { question: "Does the website track my flight?", answer: "No. Enter the pickup time you need; flight tracking is not part of this booking form." }, { question: "Can I request a larger vehicle?", answer: "Yes. Choose SUV or Innova Crysta, subject to confirmation and fleet availability." }],
  relatedSlugs: ["bellandur-to-bangalore-airport-taxi", "hsr-layout-to-bangalore-airport-taxi", "electronic-city-to-bangalore-airport-taxi"],
  targetQueries: ["sarjapur road to airport cab", "sarjapur road airport taxi", "airport drop sarjapur road"],
});

export const marathahalliToAirport = airportRoute({
  slug: "marathahalli-to-bangalore-airport-taxi", originId: "marathahalli", destinationId: "blr-airport", direction: "to-airport", locality: "Marathahalli",
  seoTitle: "Marathahalli to Bangalore Airport Taxi", metaDescription: "Request a Marathahalli to Bangalore Airport taxi with clear bridge, service-road, apartment, or office pickup details.",
  h1: "Marathahalli to Bangalore Airport taxi", heroText: "Avoid bridge-side pickup confusion with a precise Marathahalli meeting point for your BLR drop.",
  intro: "Marathahalli’s bridge, junction, service roads, and nearby tech campuses create several places that people casually call the same area. For an airport trip, the useful detail is which side of the junction the car can reach. Once that is settled, the request can be reviewed with the flight schedule and normal traffic uncertainty in mind.",
  pickupHeading: "Choose the reachable side", pickupBody: "Name the apartment, hotel, office, or service road and specify the side of the bridge. Do not ask a driver to stop where traffic rules make pickup unsafe.",
  destinationHeading: "Airport drop, not a vague north-Bangalore stop", destinationBody: "The form fixes the destination to Kempegowda International Airport (BLR), keeping the route unambiguous through OTP and submission.",
  guidanceHeading: "Junction first, airport road later", guidanceBody: "A delay around the Marathahalli junction can consume planned buffer early. A sedan suits light luggage; choose an SUV when several people are joining from one office or apartment.",
  vehicleNotes: [{ category: "Sedan", note: "Suitable for a direct pickup with light luggage." }, { category: "SUV", note: "Better for a shared office departure or several check-in bags." }],
  faq: [{ question: "Can I request pickup near Marathahalli Bridge?", answer: "Use a safe, reachable landmark and the correct side of the bridge. The driver cannot stop in a prohibited traffic lane." }, { question: "Is the airport endpoint editable?", answer: "No. Airport Drop keeps Kempegowda International Airport (BLR) fixed." }, { question: "Does a request assign a driver immediately?", answer: "No. It remains pending until operations confirms the trip." }],
  relatedSlugs: ["whitefield-to-bangalore-airport-taxi", "bellandur-to-bangalore-airport-taxi", "hsr-layout-to-bangalore-airport-taxi"],
  targetQueries: ["marathahalli to airport taxi", "marathahalli airport cab", "marathahalli airport drop"],
});

export const hebbalToAirport = airportRoute({
  slug: "hebbal-to-bangalore-airport-taxi", originId: "hebbal", destinationId: "blr-airport", direction: "to-airport", locality: "Hebbal",
  seoTitle: "Hebbal to Bangalore Airport Taxi", metaDescription: "Advance taxi requests from Hebbal homes, hotels, and offices to Bangalore Airport, with price on request.",
  h1: "Hebbal to Bangalore Airport taxi", heroText: "Start north of the city core with a pickup that distinguishes Hebbal’s flyover, service roads, and neighbourhood gates.",
  intro: "Hebbal is closer to the airport corridor than many south-Bangalore localities, but the flyover and its service roads still make meeting-point accuracy important. A hotel lobby, apartment entrance, and roadside landmark are not interchangeable. This page focuses on getting that start right without promising a traffic-free run.",
  pickupHeading: "Flyover and service-road clarity", pickupBody: "Provide the property name and reachable entrance. If you are near the flyover, identify the service road and direction rather than using the junction as a pickup pin.",
  destinationHeading: "Continue to Kempegowda Airport", destinationBody: "The airport remains the fixed drop. Check the airline terminal and leave time for airport processing after the taxi arrives.",
  guidanceHeading: "Shorter city exposure is not a guarantee", guidanceBody: "Hebbal avoids some cross-city segments, but junction traffic and airport-road conditions still vary. A premium sedan may suit a business departure; an SUV is more useful for family luggage.",
  vehicleNotes: [{ category: "Premium", note: "A quieter option for a business pickup from a Hebbal hotel or office." }, { category: "SUV", note: "Choose for family groups carrying several suitcases." }],
  faq: [{ question: "Is Hebbal always quick to the airport?", answer: "No fixed time is promised. The corridor is direct, but junction and airport-road traffic still change." }, { question: "Can pickup be from a Hebbal service road?", answer: "Yes, when you provide a safe and precise property entrance or landmark." }, { question: "Are fares listed for this route?", answer: "No. Price on request after you submit the trip details." }],
  relatedSlugs: ["manyata-tech-park-to-bangalore-airport-taxi", "yelahanka-to-bangalore-airport-taxi", "whitefield-to-bangalore-airport-taxi"],
  targetQueries: ["hebbal to bangalore airport taxi", "hebbal airport cab", "airport drop hebbal"],
});

export const yelahankaToAirport = airportRoute({
  slug: "yelahanka-to-bangalore-airport-taxi", originId: "yelahanka", destinationId: "blr-airport", direction: "to-airport", locality: "Yelahanka",
  seoTitle: "Yelahanka to Bangalore Airport Taxi", metaDescription: "Request a Yelahanka to Kempegowda Airport taxi from New Town, Old Town, apartments, or hotels.",
  h1: "Yelahanka to Bangalore Airport taxi", heroText: "Tell the desk whether your pickup is in Yelahanka New Town, Old Town, or a nearby development before the airport run.",
  intro: "Yelahanka is already on Bangalore’s northern side, yet its old town, new town, and surrounding developments cover distinct pickup areas. The airport journey is easier to plan when the request names the neighbourhood and gate rather than assuming every Yelahanka address sits on the same approach road.",
  pickupHeading: "Old Town, New Town, or beyond", pickupBody: "Include the part of Yelahanka, property name, and main entrance. Newer developments may have multiple gates with different access rules.",
  destinationHeading: "Fixed BLR destination", destinationBody: "Airport Drop keeps Kempegowda International Airport (BLR) fixed while you provide the actual Yelahanka pickup.",
  guidanceHeading: "North Bangalore still needs a buffer", guidanceBody: "Being north of the city reduces some cross-city exposure but does not remove local junction or airport approach delays. Select the vehicle for luggage, not for an assumed short ride.",
  vehicleNotes: [{ category: "Sedan", note: "Works for a straightforward New Town pickup with modest luggage." }, { category: "Innova Crysta", note: "Useful for extended families leaving a larger residential community." }],
  faq: [{ question: "Do you cover Yelahanka New Town and Old Town?", answer: "Yes. State which area and give the exact gate or property." }, { question: "Can I leave the airport drop field unchanged?", answer: "It is fixed automatically to Kempegowda International Airport (BLR)." }, { question: "Is online payment required?", answer: "No online payment is taken on this website." }],
  relatedSlugs: ["hebbal-to-bangalore-airport-taxi", "manyata-tech-park-to-bangalore-airport-taxi", "whitefield-to-bangalore-airport-taxi"],
  targetQueries: ["yelahanka to airport taxi", "yelahanka airport cab", "yelahanka airport drop"],
});

export const manyataTechParkToAirport = airportRoute({
  slug: "manyata-tech-park-to-bangalore-airport-taxi", originId: "manyata-tech-park", destinationId: "blr-airport", direction: "to-airport", locality: "Manyata Tech Park",
  seoTitle: "Manyata Tech Park to Bangalore Airport Taxi", metaDescription: "Airport taxi requests from Manyata Tech Park and Embassy business-campus gates to Kempegowda Airport.",
  h1: "Manyata Tech Park to Bangalore Airport taxi", heroText: "Move from the correct Manyata campus gate to BLR with an advance request built around your office departure.",
  intro: "Manyata Tech Park is a large business campus, and “main gate” may not identify the entrance closest to your building. Colleagues leaving together also bring a different luggage mix from a residential pickup. The booking should include the building, gate, and expected time everyone can meet outside—not merely the campus name.",
  pickupHeading: "Building and gate inside Manyata", pickupBody: "Give the office tower or company building and the permitted cab gate. Security and internal circulation can add time before the airport journey begins.",
  destinationHeading: "Office departure to BLR", destinationBody: "The destination is fixed to Kempegowda International Airport. Airline terminal and check-in planning remain the passenger’s responsibility.",
  guidanceHeading: "Coordinate the travelling group", guidanceBody: "If colleagues leave from different towers, choose one reachable meeting point. An Innova or SUV may be more appropriate when work bags and suitcases travel together.",
  vehicleNotes: [{ category: "Innova Crysta", note: "Practical for colleagues sharing a campus pickup with varied luggage." }, { category: "Premium", note: "Suitable for a client or executive airport departure from the business park." }],
  faq: [{ question: "Can the taxi enter Manyata Tech Park?", answer: "Entry depends on current campus security. Provide the permitted cab gate and building so pickup instructions are clear." }, { question: "Can several colleagues share one booking?", answer: "Yes. Choose a vehicle category with enough seats and luggage room, subject to confirmation." }, { question: "Is Manyata Embassy Business Park a separate route page?", answer: "No. It is an editorial alias for this same canonical Manyata Tech Park route." }],
  relatedSlugs: ["hebbal-to-bangalore-airport-taxi", "yelahanka-to-bangalore-airport-taxi", "marathahalli-to-bangalore-airport-taxi"],
  targetQueries: ["manyata tech park to airport taxi", "manyata airport cab", "manyata embassy business park airport taxi"],
});

export const airportToElectronicCity = airportRoute({
  slug: "bangalore-airport-to-electronic-city-taxi", originId: "blr-airport", destinationId: "electronic-city", direction: "from-airport", locality: "Electronic City",
  seoTitle: "Bangalore Airport to Electronic City Taxi", metaDescription: "Request an airport pickup from BLR to Electronic City Phase 1, Phase 2, hotels, and residential addresses.",
  h1: "Bangalore Airport to Electronic City taxi", heroText: "Continue from a BLR arrival to the correct Electronic City phase with luggage and destination details already recorded.",
  intro: "An arrival for Electronic City means a long transfer across Bangalore after baggage collection. The destination must distinguish Phase 1, Phase 2, a hotel on Hosur Road, or a residential address beyond a campus gate. This reverse route has distinct search and customer intent from an outbound flight drop, so it receives one dedicated canonical page.",
  pickupHeading: "Pickup after arrival at BLR", pickupBody: "Airport Pickup fixes Kempegowda International Airport (BLR) as the starting point. Meeting instructions follow after the request is accepted; flight tracking is not silently assumed.",
  destinationHeading: "Identify the Electronic City phase", destinationBody: "Give the phase, building, hotel, or apartment gate. For office campuses, add the entrance used by visitor cabs rather than only a company name.",
  guidanceHeading: "Plan for the full city crossing", guidanceBody: "The transfer continues through multiple city corridors after the airport road. Late arrivals may face different conditions from business-hour landings; no fixed duration is advertised. Choose luggage capacity for the bags collected at BLR.",
  vehicleNotes: [{ category: "SUV", note: "Useful for several passengers arriving with check-in luggage." }, { category: "Premium", note: "A quieter cabin for a long transfer after a business flight." }],
  faq: [{ question: "Does this page cover both Electronic City phases?", answer: "Yes. Enter Phase 1 or Phase 2 and the exact destination gate." }, { question: "Will the driver track my flight automatically?", answer: "No flight-tracking feature is promised. Share timing changes with the booking desk." }, { question: "Is there a separate E-City URL?", answer: "No. E-City is an alias; this is the single canonical airport-to-Electronic-City page." }],
  relatedSlugs: ["electronic-city-to-bangalore-airport-taxi", "bangalore-airport-to-whitefield-taxi", "hsr-layout-to-bangalore-airport-taxi"],
  targetQueries: ["bangalore airport to electronic city taxi", "electronic city airport pickup", "airport to e-city cab"],
});

export const priorityAirportRoutes: readonly RoutePageContent[] = [
  hsrLayoutToAirport, bellandurToAirport, sarjapurRoadToAirport, marathahalliToAirport,
  hebbalToAirport, yelahankaToAirport, manyataTechParkToAirport, airportToElectronicCity,
];
