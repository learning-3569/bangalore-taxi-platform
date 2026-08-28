"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/components/auth/AuthProvider";
import { Button } from "@/components/ui/Button";
import { pickupDisplay, problemMessage, type Booking } from "@/lib/bookings";

export function BookingDetails({ id }: { id: string }) {
  const auth = useAuth(); const router = useRouter();
  const [booking, setBooking] = useState<Booking | null>(null); const [error, setError] = useState(""); const [cancelling, setCancelling] = useState(false);
  useEffect(() => {
    if (!auth.ready) return;
    if (!auth.user) { router.replace(`/login?next=${encodeURIComponent(`/account/bookings/${id}`)}`); return; }
    void auth.authenticatedFetch(`/api/bookings/${id}`).then(async response => {
      if (!response.ok) throw new Error(await problemMessage(response)); setBooking(await response.json() as Booking);
    }).catch(reason => setError(reason instanceof Error ? reason.message : "Could not load booking."));
  }, [auth, id, router]);
  async function cancel() {
    if (!booking || !window.confirm("Cancel this booking request?")) return;
    setCancelling(true); setError("");
    try { const response = await auth.authenticatedFetch(`/api/bookings/${id}/cancel`, { method: "POST" }); if (!response.ok) throw new Error(await problemMessage(response)); setBooking(await response.json() as Booking); }
    catch (reason) { setError(reason instanceof Error ? reason.message : "Could not cancel the booking request."); }
    finally { setCancelling(false); }
  }
  if (!auth.ready || !booking && !error) return <p role="status">Loading booking details…</p>;
  if (!booking) return <p role="alert" className="text-red-700">{error}</p>;
  return <div className="border border-line bg-paper p-5">
    <div className="flex flex-wrap justify-between gap-2"><h1 className="font-display text-2xl font-semibold text-navy">{booking.bookingNumber}</h1><strong>{booking.statusLabel}</strong></div>
    <dl className="mt-5 grid gap-3 text-sm">
      <div><dt className="text-ink-muted">Route</dt><dd>{booking.pickup} → {booking.drop}</dd></div>
      <div><dt className="text-ink-muted">Pickup</dt><dd>{pickupDisplay(booking)}</dd></div>
      {booking.returnAt ? <div><dt className="text-ink-muted">Return</dt><dd>{new Intl.DateTimeFormat("en-IN", { dateStyle: "medium", timeStyle: "short", timeZone: "Asia/Kolkata" }).format(new Date(booking.returnAt))}</dd></div> : null}
      <div><dt className="text-ink-muted">Requested vehicle</dt><dd>{booking.vehicleTypeName}</dd></div>
      {booking.customerNotes ? <div><dt className="text-ink-muted">Notes</dt><dd>{booking.customerNotes}</dd></div> : null}
      <div><dt className="text-ink-muted">Request created</dt><dd>{new Date(booking.createdAt).toLocaleString("en-IN")}</dd></div>
    </dl>
    {booking.status === "driver_assigned" ? <section className="mt-6 border-t border-line pt-5"><h2 className="font-display text-lg font-semibold">Assigned for your trip</h2><dl className="mt-3 grid gap-3 text-sm sm:grid-cols-2"><div><dt className="text-ink-muted">Driver</dt><dd>{booking.assignedDriverName ?? "Assigned"}</dd></div><div><dt className="text-ink-muted">Vehicle</dt><dd>{[booking.assignedVehicleTypeName, booking.assignedVehicleRegistration].filter(Boolean).join(" · ")}</dd></div></dl><p className="mt-3 text-sm text-ink-muted">The driver and vehicle are assigned. This does not mean the driver is en route yet.</p></section> : null}
    <h2 className="mt-6 font-display text-lg font-semibold">Status history</h2>
    <ol className="mt-2 grid gap-2">{booking.history.map((entry, index) => <li key={`${entry.createdAt}-${index}`} className="border-l-2 border-taxi pl-3"><strong>{entry.statusLabel}</strong><p className="text-xs text-ink-muted">{new Date(entry.createdAt).toLocaleString("en-IN")}{entry.reason ? ` · ${entry.reason}` : ""}</p></li>)}</ol>
    {error ? <p role="alert" className="mt-3 text-sm text-red-700">{error}</p> : null}
    {booking.canCancel ? <div className="mt-6"><Button type="button" variant="outline" disabled={cancelling} onClick={() => void cancel()}>{cancelling ? "Cancelling…" : "Cancel request"}</Button></div> : null}
  </div>;
}
