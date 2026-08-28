import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { BookingWidget } from "@/components/booking/BookingWidget";
import { MyBookings } from "@/components/booking/MyBookings";
import { BookingDetails } from "@/components/booking/BookingDetails";
import { metadata as listMetadata } from "@/app/account/bookings/page";
import { metadata as detailMetadata } from "@/app/account/bookings/[id]/page";

const push = vi.fn(); const replace = vi.fn(); const authenticatedFetch = vi.fn();
const router = { push, replace };
let bookingSearch = "";
const auth = { user: { userId: "u", phoneNumber: "+919876543210", maskedPhone: "******3210", roles: ["customer"] }, ready: true, authenticatedFetch };
vi.mock("next/navigation", () => ({ useRouter: () => router, usePathname: () => "/airport-taxi-bangalore", useSearchParams: () => new URLSearchParams(bookingSearch) }));
vi.mock("@/components/auth/AuthProvider", () => ({ useAuth: () => auth }));

const booking = { id: "11111111-1111-4111-8111-111111111111", bookingNumber: "BLR-2026-000001", pickup: "MG Road", drop: "Kempegowda International Airport (BLR)", pickupAt: "2026-09-10T04:30:00Z", pickupTimezone: "Asia/Kolkata", pickupLocalDate: "2026-09-10", serviceType: "airport", airportJourneyType: "drop", vehicleType: "sedan", vehicleTypeName: "Sedan", status: "pending", statusLabel: "Pending confirmation", createdAt: "2026-08-27T10:00:00Z", canCancel: true, history: [{ status: "pending", statusLabel: "Pending confirmation", createdAt: "2026-08-27T10:00:00Z", reason: "Booking request received" }] };

async function completeForm(user: ReturnType<typeof userEvent.setup>) {
  await user.type(screen.getByLabelText(/pickup location/i), "MG Road");
  await user.type(screen.getByLabelText(/travel date/i), "2026-09-10"); await user.type(screen.getByLabelText(/pickup time/i), "10:00");
}

describe("authenticated booking", () => {
  beforeEach(() => { bookingSearch = ""; authenticatedFetch.mockReset(); push.mockReset(); replace.mockReset(); auth.user = { userId: "u", phoneNumber: "+919876543210", maskedPhone: "******3210", roles: ["customer"] }; });
  it("exposes Airport Transfer journey choices and defaults Drop to the canonical airport", () => {
    render(<BookingWidget />);
    expect(screen.getByRole("tab", { name: /airport transfer/i })).toHaveAttribute("aria-selected", "true");
    expect(screen.getByRole("group", { name: /airport journey/i })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /^pickup$/i })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /^drop$/i })).toHaveAttribute("aria-pressed", "true");
    expect(screen.getByRole("button", { name: /round trip/i })).toBeInTheDocument();
    expect(screen.getByLabelText(/drop location/i)).toHaveValue("Kempegowda International Airport (BLR)");
    expect(screen.getByLabelText(/drop location/i)).toHaveAttribute("readonly");
  });
  it("locks Airport Pickup and switches cleanly to Airport Drop", async () => {
    const user = userEvent.setup(); render(<BookingWidget />);
    await user.type(screen.getByLabelText(/pickup location/i), "Whitefield");
    await user.click(screen.getByRole("button", { name: /^pickup$/i }));
    expect(screen.getByLabelText(/pickup location/i)).toHaveValue("Kempegowda International Airport (BLR)");
    expect(screen.getByLabelText(/pickup location/i)).toHaveAttribute("readonly");
    expect(screen.getByLabelText(/drop location/i)).not.toHaveAttribute("readonly");
    await user.type(screen.getByLabelText(/drop location/i), "Whitefield");
    await user.click(screen.getByRole("button", { name: /^drop$/i }));
    expect(screen.getByLabelText(/pickup location/i)).toHaveValue("Whitefield");
    expect(screen.getByLabelText(/pickup location/i)).not.toHaveAttribute("readonly");
    expect(screen.getByLabelText(/drop location/i)).toHaveValue("Kempegowda International Airport (BLR)");
  });
  it("shows return controls only for Airport Round Trip", async () => {
    const user = userEvent.setup(); render(<BookingWidget />);
    expect(screen.queryByLabelText(/return date/i)).not.toBeInTheDocument();
    await user.click(screen.getByRole("button", { name: /round trip/i }));
    expect(screen.getByLabelText(/return date/i)).toBeRequired();
    expect(screen.getByLabelText(/return time/i)).toBeRequired();
  });
  it("restores editable Local fields after Airport Drop", async () => {
    const user = userEvent.setup(); render(<BookingWidget />);
    await user.type(screen.getByLabelText(/pickup location/i), "Whitefield");
    await user.click(screen.getByRole("tab", { name: /^local$/i }));
    expect(screen.getByLabelText(/pickup location/i)).not.toHaveAttribute("readonly");
    expect(screen.getByLabelText(/drop location/i)).not.toHaveAttribute("readonly");
    await user.type(screen.getByLabelText(/drop location/i), "Koramangala");
    expect(screen.getByLabelText(/drop location/i)).toHaveValue("Koramangala");
  });
  it("normalizes restored Airport Pickup intent and preserves date, time, and vehicle", () => {
    bookingSearch = "serviceType=airport&airportJourneyType=pickup&pickup=Whitefield&drop=Indiranagar&travelDate=2026-09-10&pickupTime=10%3A30&vehicleType=suv";
    render(<BookingWidget />);
    expect(screen.getByRole("tab", { name: /airport transfer/i })).toHaveAttribute("aria-selected", "true");
    expect(screen.getByRole("button", { name: /^pickup$/i })).toHaveAttribute("aria-pressed", "true");
    expect(screen.getByLabelText(/pickup location/i)).toHaveValue("Kempegowda International Airport (BLR)");
    expect(screen.getByLabelText(/drop location/i)).toHaveValue("Indiranagar");
    expect(screen.getByLabelText(/travel date/i)).toHaveValue("2026-09-10");
    expect(screen.getByLabelText(/pickup time/i)).toHaveValue("10:30");
    expect(screen.getByLabelText(/vehicle type/i)).toHaveValue("suv");
  });
  it("redirects unauthenticated customers with every intent field", async () => {
    auth.user = null as never; const user = userEvent.setup(); render(<BookingWidget />); await completeForm(user); await user.click(screen.getByRole("button", { name: /book now/i }));
    expect(push).toHaveBeenCalledWith(expect.stringMatching(/travelDate=2026-09-10.*pickupTime=10%3A00.*vehicleType=sedan/)); expect(authenticatedFetch).not.toHaveBeenCalled();
  });
  it("shows success and booking number only after persistence", async () => {
    let resolve!: (value: Response) => void; authenticatedFetch.mockReturnValue(new Promise<Response>(r => { resolve = r; }));
    const user = userEvent.setup(); render(<BookingWidget />); await completeForm(user); await user.click(screen.getByRole("button", { name: /book now/i }));
    expect(screen.queryByText(/booking request received/i)).not.toBeInTheDocument(); expect(screen.getByRole("button", { name: /submitting/i })).toBeDisabled();
    resolve(new Response(JSON.stringify(booking), { status: 201 }));
    expect(await screen.findByText("BLR-2026-000001")).toBeInTheDocument(); expect(screen.getByText(/pending confirmation/i)).toBeInTheDocument();
    expect(screen.getByRole("link", { name: /view my bookings/i })).toHaveAttribute("href", "/account/bookings");
  });
  it("does not show false success or double submit on API failure", async () => {
    let resolve!: (value: Response) => void; authenticatedFetch.mockReturnValue(new Promise<Response>(r => { resolve = r; })); const user = userEvent.setup(); render(<BookingWidget />); await completeForm(user);
    const button = screen.getByRole("button", { name: /book now/i }); await user.dblClick(button); expect(authenticatedFetch).toHaveBeenCalledTimes(1); resolve(new Response(JSON.stringify({ detail: "Unavailable" }), { status: 503 })); expect(await screen.findByRole("alert")).toHaveTextContent("Unavailable"); expect(screen.queryByText(/booking request received/i)).not.toBeInTheDocument();
  });
});

describe("customer booking pages", () => {
  beforeEach(() => { authenticatedFetch.mockReset(); auth.user = { userId: "u", phoneNumber: "+919876543210", maskedPhone: "******3210", roles: ["customer"] }; });
  it("renders loading, empty, and populated My Bookings states", async () => {
    let resolve!: (value: Response) => void; authenticatedFetch.mockReturnValue(new Promise<Response>(r => { resolve = r; })); const view = render(<MyBookings />); expect(screen.getByRole("status")).toHaveTextContent(/loading/i);
    resolve(new Response("[]")); expect(await screen.findByText(/no booking requests/i)).toBeInTheDocument(); view.unmount(); authenticatedFetch.mockResolvedValue(new Response(JSON.stringify([booking]))); render(<MyBookings />); expect(await screen.findByText("BLR-2026-000001")).toBeInTheDocument();
  });
  it("shows details and confirms cancellation", async () => {
    authenticatedFetch.mockResolvedValueOnce(new Response(JSON.stringify(booking))).mockResolvedValueOnce(new Response(JSON.stringify({ ...booking, status: "cancelled", statusLabel: "Cancelled", canCancel: false })));
    vi.spyOn(window, "confirm").mockReturnValue(true); const user = userEvent.setup(); render(<BookingDetails id={booking.id} />); expect(await screen.findByText("BLR-2026-000001")).toBeInTheDocument();
    await user.click(screen.getByRole("button", { name: /cancel request/i })); await waitFor(() => expect(authenticatedFetch).toHaveBeenLastCalledWith(`/api/bookings/${booking.id}/cancel`, { method: "POST" })); expect(await screen.findByText("Cancelled")).toBeInTheDocument();
  });
  it("keeps the booking cancellable and shows an API cancellation error", async () => {
    authenticatedFetch.mockResolvedValueOnce(new Response(JSON.stringify(booking))).mockResolvedValueOnce(new Response(JSON.stringify({ detail: "Cancellation unavailable" }), { status: 409 }));
    vi.spyOn(window, "confirm").mockReturnValue(true); const user = userEvent.setup(); render(<BookingDetails id={booking.id} />); await screen.findByText("BLR-2026-000001"); await user.click(screen.getByRole("button", { name: /cancel request/i }));
    expect(await screen.findByRole("alert")).toHaveTextContent("Cancellation unavailable"); expect(screen.getByRole("button", { name: /cancel request/i })).toBeEnabled();
  });
  it("shows only customer-safe assignment details after assignment", async () => {
    authenticatedFetch.mockResolvedValueOnce(new Response(JSON.stringify({ ...booking, status: "driver_assigned", statusLabel: "Driver assigned", canCancel: false, assignedDriverName: "Asha", assignedVehicleRegistration: "KA01AA1001", assignedVehicleTypeName: "Sedan" })));
    render(<BookingDetails id={booking.id} />); expect(await screen.findByText("Asha")).toBeInTheDocument(); expect(screen.getByText("Sedan · KA01AA1001")).toBeInTheDocument();
    expect(screen.queryByText("+919800000001")).not.toBeInTheDocument(); expect(screen.queryByRole("button", { name: /cancel request/i })).not.toBeInTheDocument(); expect(screen.getByText(/does not mean the driver is en route/i)).toBeInTheDocument();
  });
  it("marks account booking pages noindex", () => {
    expect(listMetadata.robots).toEqual({ index: false, follow: false }); expect(detailMetadata.robots).toEqual({ index: false, follow: false });
  });
});
