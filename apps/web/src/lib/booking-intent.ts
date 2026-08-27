export type BookingIntent = {
  next?: string;
  pickup?: string;
  drop?: string;
  tripType?: string;
  travelDate?: string;
  pickupTime?: string;
  vehicleType?: string;
};

export function loginHref(intent: BookingIntent = {}): string {
  const params = new URLSearchParams();
  if (intent.next) params.set("next", intent.next);
  if (intent.pickup) params.set("pickup", intent.pickup);
  if (intent.drop) params.set("drop", intent.drop);
  if (intent.tripType) params.set("tripType", intent.tripType);
  if (intent.travelDate) params.set("travelDate", intent.travelDate);
  if (intent.pickupTime) params.set("pickupTime", intent.pickupTime);
  if (intent.vehicleType) params.set("vehicleType", intent.vehicleType);
  const query = params.toString();
  return query ? `/login?${query}` : "/login";
}

export function parseBookingIntent(searchParams: URLSearchParams): BookingIntent {
  return {
    next: bounded(searchParams.get("next"), 300),
    pickup: bounded(searchParams.get("pickup"), 500),
    drop: bounded(searchParams.get("drop"), 500),
    tripType: allowed(searchParams.get("tripType"), ["airport", "local", "outstation", "corporate"]),
    travelDate: /^\d{4}-\d{2}-\d{2}$/.test(searchParams.get("travelDate") ?? "") ? searchParams.get("travelDate")! : undefined,
    pickupTime: /^\d{2}:\d{2}$/.test(searchParams.get("pickupTime") ?? "") ? searchParams.get("pickupTime")! : undefined,
    vehicleType: allowed(searchParams.get("vehicleType"), ["sedan", "suv", "innova", "premium"]),
  };
}

export function continueHref(intent: BookingIntent): string {
  if (intent.next && intent.next.startsWith("/") && !intent.next.startsWith("//")) {
    const params = new URLSearchParams();
    for (const key of ["pickup", "drop", "tripType", "travelDate", "pickupTime", "vehicleType"] as const) {
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
