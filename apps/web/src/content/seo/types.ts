export type LocationType = "city" | "locality" | "airport" | "outstation" | "landmark";

export type LocationContent = {
  id: string;
  name: string;
  slug: string;
  alternateName?: string;
  type: LocationType;
  city: string;
  state: string;
  country: string;
  airportCode?: string;
  latitude?: number;
  longitude?: number;
  published: boolean;
};

export type ParentServiceId = "airport-taxi-bangalore" | "outstation-taxi-bangalore";

export type RouteType = "airport" | "outstation";
export type RouteDirection = "to-airport" | "from-airport" | "outstation-outbound" | "outstation-inbound";
export type TripTypeValue = "one-way" | "round-trip" | "airport" | "local";

export type ImageRef = {
  src: string;
  alt: string;
};

export type FaqItem = {
  question: string;
  answer: string;
};

export type ContentSection = {
  heading: string;
  body: string;
};

export type VehicleNote = {
  category: string;
  note: string;
};

/**
 * Code-managed SEO route lander. CMS (later) should map to the same shape.
 * Origin/destination are location catalog ids — not free-text duplicates.
 */
export type RoutePageContent = {
  slug: string;
  published: boolean;
  indexable: boolean;
  lastUpdated: string;
  originId: string;
  destinationId: string;
  routeType: RouteType;
  direction: RouteDirection;
  seoTitle: string;
  metaDescription: string;
  h1: string;
  heroEyebrow: string;
  heroText: string;
  heroImage: ImageRef;
  primaryCtaLabel: string;
  bookingHeading: string;
  bookingSubmitLabel: string;
  defaultTripType: TripTypeValue;
  summary: {
    from: string;
    to: string;
    tripType: string;
    vehicleCategories: string;
    distanceNote: string;
    durationNote: string;
  };
  intro: string;
  pickupInformation: ContentSection;
  destinationInformation: ContentSection;
  travelGuidance: ContentSection;
  whyChoose: readonly { title: string; body: string }[];
  howBookingWorks: readonly { title: string; body: string }[];
  vehicleNotes: readonly VehicleNote[];
  relatedSlugs: readonly string[];
  parentServiceId: ParentServiceId;
  faq: readonly FaqItem[];
  farePlaceholder: string;
};

export type ServicePageContent = {
  slug: ParentServiceId;
  published: boolean;
  indexable: boolean;
  lastUpdated: string;
  seoTitle: string;
  metaDescription: string;
  h1: string;
  heroEyebrow: string;
  heroText: string;
  heroImage: ImageRef;
  intro: string;
  sections: readonly ContentSection[];
  faq: readonly FaqItem[];
  routeType: RouteType;
};

/** Locality service landers — none published. Locations in the catalog are not pages. */
export type LocationPageContent = {
  slug: string;
  published: boolean;
  indexable: boolean;
  lastUpdated: string;
  localityId: string;
  seoTitle: string;
  metaDescription: string;
  h1: string;
  intro: string;
};
