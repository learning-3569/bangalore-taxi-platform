"use client";

import { useEffect, useState } from "react";
import { useAuth } from "@/components/AuthProvider";
import { problem, type AdminBookingDetails, type AdminDriverPage, type AdminVehiclePage } from "@/lib/admin-bookings";

export function AssignmentPanel({ booking, onAssigned }: { booking: AdminBookingDetails; onAssigned(value: AdminBookingDetails): void }) {
  const auth = useAuth();
  const [drivers, setDrivers] = useState<AdminDriverPage | null>(null);
  const [vehicles, setVehicles] = useState<AdminVehiclePage | null>(null);
  const [driverId, setDriverId] = useState(""); const [vehicleId, setVehicleId] = useState("");
  const [error, setError] = useState(""); const [busy, setBusy] = useState(false);

  useEffect(() => {
    let active = true; setError("");
    Promise.all([
      auth.authenticatedFetch("/api/admin/drivers?eligibleOnly=true&page=1&pageSize=100"),
      auth.authenticatedFetch(`/api/admin/vehicles?eligibleOnly=true&vehicleType=${encodeURIComponent(booking.vehicleType)}&page=1&pageSize=100`),
    ]).then(async ([driverResponse, vehicleResponse]) => {
      if (!driverResponse.ok) throw new Error(await problem(driverResponse));
      if (!vehicleResponse.ok) throw new Error(await problem(vehicleResponse));
      const [driverPage, vehiclePage] = await Promise.all([driverResponse.json() as Promise<AdminDriverPage>, vehicleResponse.json() as Promise<AdminVehiclePage>]);
      if (active) { setDrivers(driverPage); setVehicles(vehiclePage); }
    }).catch(reason => { if (active) setError(reason instanceof Error ? reason.message : "Could not load assignment options."); });
    return () => { active = false; };
  }, [auth, booking.vehicleType]);

  async function assign() {
    if (!driverId || !vehicleId) { setError("Choose both a driver and a compatible vehicle."); return; }
    const driver = drivers?.items.find(x => x.id === driverId); const vehicle = vehicles?.items.find(x => x.id === vehicleId);
    if (!driver || !vehicle) { setError("The selected driver or vehicle is no longer available."); return; }
    if (!window.confirm(`Assign ${driver.displayName} with ${vehicle.registrationNumber}?`)) return;
    setBusy(true); setError("");
    try {
      const response = await auth.authenticatedFetch(`/api/admin/bookings/${booking.id}/assignment`, {
        method: "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify({ driverId, vehicleId }),
      });
      if (!response.ok) throw new Error(await problem(response));
      onAssigned(await response.json() as AdminBookingDetails);
    } catch (reason) { setError(reason instanceof Error ? reason.message : "Could not assign the booking."); }
    finally { setBusy(false); }
  }

  if (!drivers || !vehicles) return error ? <p role="alert" className="mt-8 text-red-300">{error}</p> : <p role="status" className="mt-8 text-slate-400">Loading eligible drivers and vehicles…</p>;
  return <section className="mt-8 rounded border border-slate-800 bg-slate-900 p-6"><h2 className="text-xl font-semibold">Assign driver and vehicle</h2><p className="mt-2 text-sm text-slate-400">Only active, available drivers and active vehicles matching the requested {booking.vehicleTypeName} category are listed.</p>
      {drivers.items.length === 0 ? <p className="mt-5 text-amber-300">No eligible drivers are available.</p> : <label className="mt-5 block text-sm font-medium">Driver<select aria-label="Driver" value={driverId} onChange={event => setDriverId(event.target.value)} className="mt-2 w-full rounded border border-slate-700 bg-slate-950 p-3"><option value="">Choose a driver</option>{drivers.items.map(driver => <option key={driver.id} value={driver.id}>{driver.driverNumber} — {driver.displayName} · {driver.phoneNumber}</option>)}</select></label>}
    {vehicles.items.length === 0 ? <p className="mt-5 text-amber-300">No eligible compatible vehicles are available.</p> : <label className="mt-5 block text-sm font-medium">Vehicle<select aria-label="Vehicle" value={vehicleId} onChange={event => setVehicleId(event.target.value)} className="mt-2 w-full rounded border border-slate-700 bg-slate-950 p-3"><option value="">Choose a vehicle</option>{vehicles.items.map(vehicle => <option key={vehicle.id} value={vehicle.id}>{vehicle.registrationNumber} · {vehicle.vehicleTypeName}</option>)}</select></label>}
    {error ? <p role="alert" className="mt-4 text-red-300">{error}</p> : null}<button type="button" disabled={busy || !drivers.items.length || !vehicles.items.length} onClick={() => void assign()} className="mt-5 rounded bg-sky-500 px-4 py-2 font-semibold text-slate-950 disabled:opacity-50">{busy ? "Assigning…" : "Review and assign"}</button>
  </section>;
}
