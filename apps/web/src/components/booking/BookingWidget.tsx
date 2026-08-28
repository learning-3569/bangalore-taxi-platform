"use client";

import { FormEvent, Suspense, useMemo, useRef, useState } from "react";
import Link from "next/link";
import { usePathname, useRouter, useSearchParams } from "next/navigation";
import { useAuth } from "@/components/auth/AuthProvider";
import { Button } from "@/components/ui/Button";
import { SelectField, TextField } from "@/components/ui/Fields";
import { tripTypes, vehicleTypes } from "@/config/site";
import { BENGALURU_AIRPORT, type AirportJourneyType, type BookingServiceType } from "@/lib/airport-booking";
import { loginHref } from "@/lib/booking-intent";
import { problemMessage, type Booking } from "@/lib/bookings";

type BookingWidgetProps = {
  defaultPickup?: string;
  defaultDrop?: string;
  defaultServiceType?: BookingServiceType;
  defaultAirportJourneyType?: AirportJourneyType;
  heading?: string;
  submitLabel?: string;
  idPrefix?: string;
};

export function BookingWidget(props: BookingWidgetProps) {
  return <Suspense fallback={<div className="border border-line bg-paper p-5 text-sm text-ink-muted">Loading booking form…</div>}><BookingWidgetContent {...props} /></Suspense>;
}

function BookingWidgetContent({
  defaultPickup,
  defaultDrop,
  defaultServiceType = "airport",
  defaultAirportJourneyType = "drop",
  heading = "Book your cab",
  submitLabel = "Book now",
  idPrefix = "",
}: BookingWidgetProps) {
  const { user, authenticatedFetch } = useAuth();
  const router = useRouter();
  const pathname = usePathname();
  const search = useSearchParams();
  const restoredService = search.get("serviceType") as BookingServiceType | null;
  const initialService = restoredService && tripTypes.some(x => x.value === restoredService) ? restoredService : defaultServiceType;
  const restoredJourney = search.get("airportJourneyType") as AirportJourneyType | null;
  const initialJourney = restoredJourney && ["pickup", "drop", "round-trip"].includes(restoredJourney) ? restoredJourney : defaultAirportJourneyType;
  const initialPickup = initialService === "airport" && initialJourney === "pickup" ? BENGALURU_AIRPORT : search.get("pickup") ?? defaultPickup ?? "";
  const initialDrop = initialService === "airport" && initialJourney !== "pickup" ? BENGALURU_AIRPORT : search.get("drop") ?? defaultDrop ?? "";
  const [serviceType, setServiceType] = useState<BookingServiceType>(initialService);
  const [airportJourney, setAirportJourney] = useState<AirportJourneyType>(initialJourney);
  const [pickup, setPickup] = useState(initialPickup);
  const [drop, setDrop] = useState(initialDrop);
  const editablePickup = useRef(initialService === "airport" && initialJourney === "pickup" ? "" : initialPickup);
  const editableDrop = useRef(initialService === "airport" && initialJourney !== "pickup" ? "" : initialDrop);
  const [created, setCreated] = useState<Booking | null>(null);
  const [error, setError] = useState("");
  const [submitting, setSubmitting] = useState(false);
  const submitLock = useRef(false);
  const minDate = useMemo(() => new Date().toISOString().slice(0, 10), []);

  async function onSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const form = new FormData(event.currentTarget);
    if (!user) {
      router.push(
        loginHref({
          next: pathname || "/",
          pickup: String(form.get("pickup") ?? ""),
          drop: String(form.get("drop") ?? ""),
          serviceType,
          airportJourneyType: serviceType === "airport" ? airportJourney : undefined,
          travelDate: String(form.get("travelDate") ?? ""),
          pickupTime: String(form.get("pickupTime") ?? ""),
          vehicleType: String(form.get("vehicleType") ?? ""),
          returnDate: String(form.get("returnDate") ?? ""),
          returnTime: String(form.get("returnTime") ?? ""),
        }),
      );
      return;
    }
    if (submitLock.current) return;
    submitLock.current = true;
    setSubmitting(true);
    setError("");
    try {
      const response = await authenticatedFetch("/api/bookings", {
        method: "POST",
        headers: { "Content-Type": "application/json", "Idempotency-Key": crypto.randomUUID() },
        body: JSON.stringify({
          pickup: form.get("pickup"), drop: form.get("drop"), serviceType,
          airportJourneyType: serviceType === "airport" ? airportJourney : null,
          travelDate: form.get("travelDate"), pickupTime: form.get("pickupTime"), vehicleType: form.get("vehicleType"),
          returnDate: form.get("returnDate"), returnTime: form.get("returnTime"),
        }),
      });
      if (!response.ok) throw new Error(await problemMessage(response));
      setCreated(await response.json() as Booking);
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : "Could not create the booking request.");
    } finally { submitLock.current = false; setSubmitting(false); }
  }

  return (
    <form
      onSubmit={onSubmit}
      className="border border-line bg-paper p-4 shadow-[0_18px_50px_rgba(8,24,39,0.14)] sm:p-5"
      noValidate
    >
      <div className="flex items-end justify-between gap-3">
        <div>
          <p className="font-display text-lg font-semibold text-navy">{heading}</p>
          <p className="text-xs text-ink-muted">
            {user
              ? "Enter your trip details and submit your booking request. Our team will review availability and confirm your trip."
              : "Enter your trip details to request a cab. You'll verify your mobile number before completing your booking request."}
          </p>
        </div>
      </div>

      <div role="tablist" aria-label="Trip type" className="mt-4 grid grid-cols-2 gap-1 bg-paper-soft p-1 sm:grid-cols-4">
        {tripTypes.map((option) => {
          const active = serviceType === option.value;
          return (
            <button
              key={option.value}
              type="button"
              role="tab"
              aria-selected={active}
              className={`px-2 py-2 text-xs font-semibold uppercase tracking-wide transition sm:text-[11px] ${
                active ? "bg-navy text-white" : "text-ink-muted hover:text-navy"
              }`}
              onClick={() => {
                if (pickup !== BENGALURU_AIRPORT) editablePickup.current = pickup;
                if (drop !== BENGALURU_AIRPORT) editableDrop.current = drop;
                setServiceType(option.value);
                setPickup(option.value === "airport" && airportJourney === "pickup" ? BENGALURU_AIRPORT : editablePickup.current);
                setDrop(option.value === "airport" && airportJourney !== "pickup" ? BENGALURU_AIRPORT : editableDrop.current);
              }}
            >
              {option.label}
            </button>
          );
        })}
      </div>
      <input type="hidden" name="serviceType" value={serviceType} />

      {serviceType === "airport" ? (
        <div role="group" aria-label="Airport journey" className="mt-3 inline-grid w-full grid-cols-3 gap-1 rounded-md border border-line bg-paper p-1 sm:w-auto sm:min-w-80">
          {(["pickup", "drop", "round-trip"] as const).map((journey) => (
            <button
              key={journey}
              type="button"
              aria-pressed={airportJourney === journey}
              className={`rounded-sm px-3 py-2 text-sm font-semibold focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-brand ${airportJourney === journey ? "bg-navy text-white" : "text-navy hover:bg-paper-soft"}`}
              onClick={() => {
                if (pickup !== BENGALURU_AIRPORT) editablePickup.current = pickup;
                if (drop !== BENGALURU_AIRPORT) editableDrop.current = drop;
                setAirportJourney(journey);
                setPickup(journey === "pickup" ? BENGALURU_AIRPORT : editablePickup.current);
                setDrop(journey === "pickup" ? editableDrop.current : BENGALURU_AIRPORT);
              }}
            >
              {journey === "round-trip" ? "Round Trip" : journey === "pickup" ? "Pickup" : "Drop"}
            </button>
          ))}
        </div>
      ) : null}

      <div className="mt-4 grid gap-3 md:grid-cols-2 lg:grid-cols-5">
        <TextField
          id={`${idPrefix}pickup`}
          name="pickup"
          label="Pickup location"
          placeholder="Enter pickup location"
          autoComplete="street-address"
          required
          value={pickup}
          readOnly={serviceType === "airport" && airportJourney === "pickup"}
          aria-readonly={serviceType === "airport" && airportJourney === "pickup"}
          onChange={(event) => { setPickup(event.target.value); editablePickup.current = event.target.value; }}
        />
        <TextField
          id={`${idPrefix}drop`}
          name="drop"
          label="Drop location"
          placeholder="Enter destination"
          autoComplete="street-address"
          required
          value={drop}
          readOnly={serviceType === "airport" && airportJourney !== "pickup"}
          aria-readonly={serviceType === "airport" && airportJourney !== "pickup"}
          onChange={(event) => { setDrop(event.target.value); editableDrop.current = event.target.value; }}
        />
        <TextField id={`${idPrefix}date`} name="travelDate" label="Travel date" type="date" min={minDate} required defaultValue={search.get("travelDate") ?? undefined} />
        <TextField id={`${idPrefix}time`} name="pickupTime" label="Pickup time" type="time" required defaultValue={search.get("pickupTime") ?? undefined} />
        <SelectField id={`${idPrefix}vehicleType`} name="vehicleType" label="Vehicle type" required defaultValue={search.get("vehicleType") ?? "sedan"}>
          {vehicleTypes.map((option) => (
            <option key={option.value} value={option.value}>
              {option.label}
            </option>
          ))}
        </SelectField>
        {serviceType === "airport" && airportJourney === "round-trip" ? (
          <>
            <TextField id={`${idPrefix}returnDate`} name="returnDate" label="Return date" type="date" min={minDate} required defaultValue={search.get("returnDate") ?? undefined} />
            <TextField id={`${idPrefix}returnTime`} name="returnTime" label="Return time" type="time" required defaultValue={search.get("returnTime") ?? undefined} />
          </>
        ) : null}
      </div>
      <div className="mt-4 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <Button type="submit" variant="taxi" className="uppercase sm:min-w-40" disabled={submitting} aria-busy={submitting}>
          {submitting ? "Submitting…" : submitLabel}
        </Button>
        <p className="text-xs text-ink-muted">
          We use these details only to process your trip request.
        </p>
      </div>
      {error ? <p role="alert" className="mt-3 text-sm text-red-700">{error}</p> : null}
      {created ? (
        <div role="status" className="mt-3 text-sm text-navy">
          <p className="font-semibold">Booking request received</p>
          <p className="mt-1"><span className="text-ink-muted">Booking number:</span> {created.bookingNumber}</p>
          <p><span className="text-ink-muted">Status:</span> Pending confirmation</p>
          <Link href="/account/bookings" className="mt-2 inline-block font-semibold underline">View my bookings</Link>
        </div>
      ) : null}
    </form>
  );
}
