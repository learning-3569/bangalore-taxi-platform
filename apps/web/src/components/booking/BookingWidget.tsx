"use client";

import { FormEvent, useMemo, useState } from "react";
import { usePathname, useRouter } from "next/navigation";
import { useAuth } from "@/components/auth/AuthProvider";
import { Button } from "@/components/ui/Button";
import { SelectField, TextField } from "@/components/ui/Fields";
import { tripTypes, vehicleTypes } from "@/config/site";
import type { TripTypeValue } from "@/content/seo/types";
import { loginHref } from "@/lib/booking-intent";

type BookingWidgetProps = {
  defaultPickup?: string;
  defaultDrop?: string;
  defaultTripType?: TripTypeValue;
  heading?: string;
  submitLabel?: string;
  idPrefix?: string;
};

export function BookingWidget({
  defaultPickup,
  defaultDrop,
  defaultTripType = "airport",
  heading = "Book your cab",
  submitLabel = "Book now",
  idPrefix = "",
}: BookingWidgetProps) {
  const { user } = useAuth();
  const router = useRouter();
  const pathname = usePathname();
  const [trip, setTrip] = useState<TripTypeValue>(defaultTripType);
  const [submitted, setSubmitted] = useState(false);
  const minDate = useMemo(() => new Date().toISOString().slice(0, 10), []);

  function onSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const form = new FormData(event.currentTarget);
    if (!user) {
      router.push(
        loginHref({
          next: pathname || "/",
          pickup: String(form.get("pickup") ?? ""),
          drop: String(form.get("drop") ?? ""),
          tripType: trip,
        }),
      );
      return;
    }
    setSubmitted(true);
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
          const active = trip === option.value;
          return (
            <button
              key={option.value}
              type="button"
              role="tab"
              aria-selected={active}
              className={`px-2 py-2 text-xs font-semibold uppercase tracking-wide transition sm:text-[11px] ${
                active ? "bg-navy text-white" : "text-ink-muted hover:text-navy"
              }`}
              onClick={() => setTrip(option.value)}
            >
              {option.label}
            </button>
          );
        })}
      </div>
      <input type="hidden" name="tripType" value={trip} />

      <div className="mt-4 grid gap-3 md:grid-cols-2 lg:grid-cols-5">
        <TextField
          id={`${idPrefix}pickup`}
          name="pickup"
          label="Pickup location"
          placeholder="Enter pickup location"
          autoComplete="street-address"
          required
          defaultValue={defaultPickup}
        />
        <TextField
          id={`${idPrefix}drop`}
          name="drop"
          label="Drop location"
          placeholder="Enter destination"
          autoComplete="street-address"
          required
          defaultValue={defaultDrop}
        />
        <TextField id={`${idPrefix}date`} name="date" label="Travel date" type="date" min={minDate} required />
        <TextField id={`${idPrefix}time`} name="time" label="Pickup time" type="time" required />
        <SelectField id={`${idPrefix}vehicleType`} name="vehicleType" label="Vehicle type" required defaultValue="sedan">
          {vehicleTypes.map((option) => (
            <option key={option.value} value={option.value}>
              {option.label}
            </option>
          ))}
        </SelectField>
      </div>
      <div className="mt-4 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <Button type="submit" variant="taxi" className="uppercase sm:min-w-40">
          {submitLabel}
        </Button>
        <p className="text-xs text-ink-muted">
          We use these details only to process your trip request.
        </p>
      </div>
      {submitted ? (
        <div role="status" className="mt-3 text-sm text-navy">
          <p className="font-semibold">Booking request received</p>
          <p className="mt-1 text-ink-muted">Pending confirmation. Our team will review availability and confirm your trip.</p>
        </div>
      ) : null}
    </form>
  );
}
