export const BENGALURU_AIRPORT = "Kempegowda International Airport (BLR)";

export type BookingServiceType = "airport" | "outstation" | "hourly" | "local";
export type AirportJourneyType = "pickup" | "drop" | "round-trip";

export function airportJourneyType(direction: string): AirportJourneyType {
  return direction === "from-airport" ? "pickup" : "drop";
}
