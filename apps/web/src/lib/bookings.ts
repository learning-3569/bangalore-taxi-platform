export type BookingHistory = { status: string; statusLabel: string; createdAt: string; reason?: string | null };
export type Booking = {
  id: string; bookingNumber: string; pickup: string; drop: string; pickupAt: string; pickupTimezone: string;
  pickupLocalDate: string; tripType: string; vehicleType: string; vehicleTypeName: string; status: string;
  statusLabel: string; customerNotes?: string | null; createdAt: string; canCancel: boolean; history: BookingHistory[];
};

export async function problemMessage(response: Response): Promise<string> {
  try { const body = await response.json() as { detail?: string; title?: string }; return body.detail ?? body.title ?? "Request failed."; }
  catch { return "Request failed. Please try again."; }
}

export function pickupDisplay(booking: Booking): string {
  return new Intl.DateTimeFormat("en-IN", { dateStyle: "medium", timeStyle: "short", timeZone: "Asia/Kolkata" }).format(new Date(booking.pickupAt));
}
