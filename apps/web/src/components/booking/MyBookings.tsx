"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/components/auth/AuthProvider";
import { pickupDisplay, problemMessage, type Booking } from "@/lib/bookings";

export function MyBookings() {
  const auth = useAuth();
  const router = useRouter();
  const [items, setItems] = useState<Booking[] | null>(null);
  const [error, setError] = useState("");

  useEffect(() => {
    if (!auth.ready) return;
    if (!auth.user) { router.replace("/login?next=%2Faccount%2Fbookings"); return; }
    void auth.authenticatedFetch("/api/bookings").then(async response => {
      if (!response.ok) throw new Error(await problemMessage(response));
      setItems(await response.json() as Booking[]);
    }).catch(reason => setError(reason instanceof Error ? reason.message : "Could not load bookings."));
  }, [auth, router]);

  if (!auth.ready || items === null && !error) return <p role="status">Loading your bookings…</p>;
  if (error) return <p role="alert" className="text-red-700">{error}</p>;
  if (!items?.length) return <div><p>You have no booking requests yet.</p><Link className="font-semibold underline" href="/#book">Request a taxi</Link></div>;
  return <div className="grid gap-4">
    {items.map(item => <Link key={item.id} href={`/account/bookings/${item.id}`} className="border border-line bg-paper p-4 hover:border-navy">
      <div className="flex flex-wrap justify-between gap-2"><strong>{item.bookingNumber}</strong><span>{item.statusLabel}</span></div>
      <p className="mt-2">{item.pickup} → {item.drop}</p>
      <p className="mt-1 text-sm text-ink-muted">{pickupDisplay(item)} · {item.vehicleTypeName}</p>
    </Link>)}
  </div>;
}
