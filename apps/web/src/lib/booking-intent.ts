export type BookingIntent = {
  next?: string;
  pickup?: string;
  drop?: string;
  tripType?: string;
  serviceType?: string;
  airportJourneyType?: string;
  travelDate?: string;
  pickupTime?: string;
  vehicleType?: string;
  returnDate?: string;
  returnTime?: string;
};

export function loginHref(intent: BookingIntent = {}): string {
  const params = new URLSearchParams();
  if (intent.next) params.set("next", intent.next);
  if (intent.pickup) params.set("pickup", intent.pickup);
  if (intent.drop) params.set("drop", intent.drop);
  if (intent.tripType) params.set("tripType", intent.tripType);
  if (intent.serviceType) params.set("serviceType", intent.serviceType);
  if (intent.airportJourneyType) params.set("airportJourneyType", intent.airportJourneyType);
  if (intent.travelDate) params.set("travelDate", intent.travelDate);
  if (intent.pickupTime) params.set("pickupTime", intent.pickupTime);
  if (intent.vehicleType) params.set("vehicleType", intent.vehicleType);
  if (intent.returnDate) params.set("returnDate", intent.returnDate);
  if (intent.returnTime) params.set("returnTime", intent.returnTime);
  const query = params.toString();
  return query ? `/login?${query}` : "/login";
}

export function parseBookingIntent(searchParams: URLSearchParams): BookingIntent {
  return {
    next: bounded(searchParams.get("next"), 300),
    pickup: bounded(searchParams.get("pickup"), 500),
    drop: bounded(searchParams.get("drop"), 500),
    serviceType: allowed(searchParams.get("serviceType"), ["airport", "outstation", "hourly", "local"]),
    airportJourneyType: allowed(searchParams.get("airportJourneyType"), ["pickup", "drop", "round-trip"]),
    travelDate: /^\d{4}-\d{2}-\d{2}$/.test(searchParams.get("travelDate") ?? "") ? searchParams.get("travelDate")! : undefined,
    pickupTime: /^\d{2}:\d{2}$/.test(searchParams.get("pickupTime") ?? "") ? searchParams.get("pickupTime")! : undefined,
    vehicleType: allowed(searchParams.get("vehicleType"), ["sedan", "suv", "innova", "premium"]),
    returnDate: /^\d{4}-\d{2}-\d{2}$/.test(searchParams.get("returnDate") ?? "") ? searchParams.get("returnDate")! : undefined,
    returnTime: /^\d{2}:\d{2}$/.test(searchParams.get("returnTime") ?? "") ? searchParams.get("returnTime")! : undefined,
  };
}

export function continueHref(intent: BookingIntent): string {
  if (intent.next && intent.next.startsWith("/") && !intent.next.startsWith("//")) {
    const params = new URLSearchParams();
    for (const key of ["pickup", "drop", "serviceType", "airportJourneyType", "travelDate", "pickupTime", "vehicleType", "returnDate", "returnTime"] as const) {
      if (intent[key]) params.set(key, intent[key]!);
    }
    const base = intent.next.split("#")[0];
    return `${base}${params.size ? `?${params}` : ""}#book`;
  }
  return "/#book";
}

function bounded(value: string | null, max: number): string | undefined {
  const trimmed = value?.trim();
  return trimmed && trimmed.length <= max ? trimmed : undefined;
}

function allowed(value: string | null, values: string[]): string | undefined {
  return value && values.includes(value) ? value : undefined;
}

export function isValidIndianMobile(input: string): boolean {
  const digits = input.replace(/\D/g, "");
  if (digits.length === 10 && /^[6-9]/.test(digits)) return true;
  if (digits.length === 12 && digits.startsWith("91") && /^[6-9]/.test(digits[2] ?? "")) return true;
  if (digits.length === 11 && digits.startsWith("0") && /^[6-9]/.test(digits[1] ?? "")) return true;
  return false;
}
