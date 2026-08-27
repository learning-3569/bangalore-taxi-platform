export type BookingIntent = {
  next?: string;
  pickup?: string;
  drop?: string;
  tripType?: string;
};

export function loginHref(intent: BookingIntent = {}): string {
  const params = new URLSearchParams();
  if (intent.next) params.set("next", intent.next);
  if (intent.pickup) params.set("pickup", intent.pickup);
  if (intent.drop) params.set("drop", intent.drop);
  if (intent.tripType) params.set("tripType", intent.tripType);
  const query = params.toString();
  return query ? `/login?${query}` : "/login";
}

export function parseBookingIntent(searchParams: URLSearchParams): BookingIntent {
  return {
    next: searchParams.get("next") ?? undefined,
    pickup: searchParams.get("pickup") ?? undefined,
    drop: searchParams.get("drop") ?? undefined,
    tripType: searchParams.get("tripType") ?? undefined,
  };
}

export function continueHref(intent: BookingIntent): string {
  if (intent.next && intent.next.startsWith("/") && !intent.next.startsWith("//")) {
    if (intent.next.includes("#")) return intent.next;
    return `${intent.next}#book`;
  }
  return "/#book";
}

export function isValidIndianMobile(input: string): boolean {
  const digits = input.replace(/\D/g, "");
  if (digits.length === 10 && /^[6-9]/.test(digits)) return true;
  if (digits.length === 12 && digits.startsWith("91") && /^[6-9]/.test(digits[2] ?? "")) return true;
  if (digits.length === 11 && digits.startsWith("0") && /^[6-9]/.test(digits[1] ?? "")) return true;
  return false;
}
