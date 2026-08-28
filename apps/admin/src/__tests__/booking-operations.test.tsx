import { render, screen, within } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { AdminGuard } from "@/components/AdminGuard";
import { BookingQueue } from "@/components/BookingQueue";
import { BookingDetails } from "@/components/BookingDetails";
import { AdminLogin } from "@/components/AdminLogin";
import { AdminShell } from "@/components/AdminShell";
import LoginPage from "@/app/login/page";

const replace = vi.fn(); const authenticatedFetch = vi.fn(); const requestOtp = vi.fn(); const peekDevOtp = vi.fn(); const verifyOtp = vi.fn(); const logout = vi.fn();
let pathname = "/bookings";
const auth: { ready: boolean; user: null | { userId: string; phoneNumber: string; maskedPhone: string; roles: string[] }; authenticatedFetch: typeof authenticatedFetch; requestOtp: typeof requestOtp; peekDevOtp: typeof peekDevOtp; verifyOtp: typeof verifyOtp; logout: typeof logout } = { ready: true, user: { userId: "a", phoneNumber: "+919876543210", maskedPhone: "******3210", roles: ["admin"] }, authenticatedFetch, requestOtp, peekDevOtp, verifyOtp, logout };
vi.mock("next/navigation", () => ({ useRouter: () => ({ replace }), usePathname: () => pathname }));
vi.mock("@/components/AuthProvider", () => ({ useAuth: () => auth }));
const item = { id: "11111111-1111-4111-8111-111111111111", bookingNumber: "BLR-2026-000001", status: "pending", statusLabel: "Pending confirmation", pickup: "MG Road", drop: "Kempegowda International Airport (BLR)", pickupAt: "2026-09-10T04:30:00Z", pickupTimezone: "Asia/Kolkata", pickupLocalDate: "2026-09-10", vehicleType: "sedan", vehicleTypeName: "Sedan", createdAt: "2026-08-27T10:00:00Z" };
const details = { ...item, serviceType: "airport", airportJourneyType: "drop", contactName: "Customer", contactMobile: "+919876543210", canAccept: true, canReject: true, canAssign: false, history: [{ status: "pending", statusLabel: "Pending confirmation", createdAt: item.createdAt, reason: "Booking request received" }] };

describe("admin operations frontend", () => {
  beforeEach(() => { authenticatedFetch.mockReset(); requestOtp.mockReset(); peekDevOtp.mockReset(); verifyOtp.mockReset(); logout.mockReset(); replace.mockReset(); vi.restoreAllMocks(); pathname = "/bookings"; auth.ready = true; auth.user = { userId: "a", phoneNumber: "+919876543210", maskedPhone: "******3210", roles: ["admin"] }; });
  it("shows persistent active Bookings navigation and the existing secure logout", async () => {
    logout.mockResolvedValue(undefined); const user = userEvent.setup(); render(<AdminShell><p>Booking Operations</p></AdminShell>);
    expect(screen.getByText("Bangalore Taxi Admin")).toBeInTheDocument(); const desktopNav = screen.getByRole("navigation", { name: "Admin navigation" });
    const bookings = within(desktopNav).getByRole("link", { name: "Bookings" }); expect(bookings).toHaveAttribute("href", "/bookings"); expect(bookings).toHaveAttribute("aria-current", "page");
    expect(screen.getByLabelText("Admin account")).toHaveTextContent("******3210"); await user.click(screen.getByRole("button", { name: "Sign out" })); expect(logout).toHaveBeenCalledOnce(); expect(replace).toHaveBeenCalledWith("/login");
  });
  it("keeps navigation active on booking details and exposes it through the mobile menu", async () => {
    pathname = `/bookings/${item.id}`; const user = userEvent.setup(); render(<AdminShell><p>Booking details</p></AdminShell>);
    expect(within(screen.getByRole("navigation", { name: "Admin navigation" })).getByRole("link", { name: "Bookings" })).toHaveAttribute("aria-current", "page");
    const toggle = screen.getByRole("button", { name: /toggle admin menu/i }); expect(toggle).toHaveAttribute("aria-expanded", "false"); await user.click(toggle); expect(toggle).toHaveAttribute("aria-expanded", "true");
    expect(within(screen.getByRole("navigation", { name: "Mobile admin navigation" })).getByRole("link", { name: "Bookings" })).toHaveAttribute("href", "/bookings");
  });
  it("keeps the login page focused without authenticated navigation", () => {
    auth.user = null; pathname = "/login"; render(<LoginPage />); expect(screen.getByRole("heading", { name: /booking operations sign in/i })).toBeInTheDocument(); expect(screen.queryByRole("navigation", { name: /admin navigation/i })).not.toBeInTheDocument(); expect(screen.queryByText("Bangalore Taxi Admin")).not.toBeInTheDocument();
  });
  it("redirects signed-out users and denies non-admin accounts", () => { auth.user = null; const first = render(<AdminGuard><p>Queue</p></AdminGuard>); expect(replace).toHaveBeenCalledWith("/login"); first.unmount(); auth.user = { userId: "c", phoneNumber: "+919876543210", maskedPhone: "******3210", roles: ["customer"] }; render(<AdminGuard><p>Queue</p></AdminGuard>); expect(screen.getByRole("alert")).toHaveTextContent("Access denied"); });
  it("shows pending queue, filters, empty and error states", async () => { authenticatedFetch.mockResolvedValueOnce(new Response(JSON.stringify({ items: [item], page: 1, pageSize: 25, totalCount: 1, totalPages: 1 }))); const user = userEvent.setup(); render(<BookingQueue />); expect(await screen.findByText(item.bookingNumber)).toBeInTheDocument(); authenticatedFetch.mockResolvedValueOnce(new Response(JSON.stringify({ items: [], page: 1, pageSize: 25, totalCount: 0, totalPages: 0 }))); await user.click(screen.getByRole("button", { name: "rejected" })); expect(await screen.findByText(/no rejected bookings/i)).toBeInTheDocument(); authenticatedFetch.mockResolvedValueOnce(new Response(JSON.stringify({ detail: "Unavailable" }), { status: 503 })); await user.click(screen.getByRole("button", { name: "accepted" })); expect(await screen.findByRole("alert")).toHaveTextContent("Unavailable"); });
  it("confirms accept, disables both actions in flight, and refreshes state", async () => { let resolve!: (response: Response) => void; authenticatedFetch.mockResolvedValueOnce(new Response(JSON.stringify(details))).mockReturnValueOnce(new Promise<Response>(done => { resolve = done; })); vi.spyOn(window, "confirm").mockReturnValue(true); const user = userEvent.setup(); render(<BookingDetails id={item.id} />); await screen.findByText(item.bookingNumber); await user.click(screen.getByRole("button", { name: /accept request/i })); expect(screen.getByRole("button", { name: /accepting/i })).toBeDisabled(); expect(screen.getByRole("button", { name: /reject request/i })).toBeDisabled(); resolve(new Response(JSON.stringify({ ...details, status: "accepted", statusLabel: "Accepted — awaiting assignment", canAccept: false, canReject: false }))); expect(await screen.findByText("Booking request accepted.")).toBeInTheDocument(); });
  it("validates rejection and explains a competing transition", async () => { authenticatedFetch.mockResolvedValueOnce(new Response(JSON.stringify(details))); vi.spyOn(window, "confirm").mockReturnValue(true); const user = userEvent.setup(); render(<BookingDetails id={item.id} />); await screen.findByText(item.bookingNumber); await user.click(screen.getByRole("button", { name: /reject request/i })); expect(screen.getByRole("alert")).toHaveTextContent(/at least 3/i); await user.type(screen.getByLabelText(/rejection reason/i), "No capacity"); authenticatedFetch.mockResolvedValueOnce(new Response(null, { status: 409 })); await user.click(screen.getByRole("button", { name: /reject request/i })); expect(await screen.findByRole("alert")).toHaveTextContent(/changed.*refresh/i); });
  it("loads compatible candidates and confirms assignment", async () => {
    const accepted = { ...details, status: "accepted", statusLabel: "Accepted — awaiting assignment", canAccept: false, canReject: false, canAssign: true };
    const assigned = { ...accepted, status: "driver_assigned", statusLabel: "Driver assigned", canAssign: false, assignedDriverName: "Asha", assignedVehicleRegistration: "KA01AA1001", assignedVehicleTypeName: "Sedan" };
    authenticatedFetch.mockResolvedValueOnce(new Response(JSON.stringify(accepted)))
      .mockResolvedValueOnce(new Response(JSON.stringify({ items: [{ id: "d1", displayName: "Asha", phoneNumber: "+919800000001", employmentStatus: "active", availabilityStatus: "available", eligible: true }], page: 1, pageSize: 100, totalCount: 1, totalPages: 1 })))
      .mockResolvedValueOnce(new Response(JSON.stringify({ items: [{ id: "v1", registrationNumber: "KA01AA1001", vehicleType: "sedan", vehicleTypeName: "Sedan", capacity: 4, status: "active", eligible: true }], page: 1, pageSize: 100, totalCount: 1, totalPages: 1 })))
      .mockResolvedValueOnce(new Response(JSON.stringify(assigned)));
    vi.spyOn(window, "confirm").mockReturnValue(true); const user = userEvent.setup(); render(<BookingDetails id={item.id} />);
    expect(await screen.findByRole("heading", { name: /assign driver and vehicle/i })).toBeInTheDocument();
    await user.selectOptions(screen.getByLabelText("Driver"), "d1"); await user.selectOptions(screen.getByLabelText("Vehicle"), "v1"); await user.click(screen.getByRole("button", { name: /review and assign/i }));
    expect(window.confirm).toHaveBeenCalledWith("Assign Asha with KA01AA1001?"); expect(await screen.findByText("Driver and vehicle assigned.")).toBeInTheDocument(); expect(screen.getByText(/Asha · Sedan · KA01AA1001/)).toBeInTheDocument();
  });
  it("shows empty assignment states and hides assignment on pending bookings", async () => {
    authenticatedFetch.mockResolvedValueOnce(new Response(JSON.stringify({ ...details, status: "accepted", canAccept: false, canReject: false, canAssign: true })))
      .mockResolvedValueOnce(new Response(JSON.stringify({ items: [], page: 1, pageSize: 100, totalCount: 0, totalPages: 0 })))
      .mockResolvedValueOnce(new Response(JSON.stringify({ items: [], page: 1, pageSize: 100, totalCount: 0, totalPages: 0 })));
    const view = render(<BookingDetails id={item.id} />); expect(await screen.findByText(/no eligible drivers/i)).toBeInTheDocument(); expect(screen.getByText(/no eligible compatible vehicles/i)).toBeInTheDocument(); view.unmount();
    authenticatedFetch.mockResolvedValueOnce(new Response(JSON.stringify(details))); render(<BookingDetails id={item.id} />); await screen.findByText(item.bookingNumber); expect(screen.queryByRole("heading", { name: /assign driver/i })).not.toBeInTheDocument();
  });
  it("disables duplicate assignment and displays a server mismatch conflict", async () => {
    const accepted = { ...details, status: "accepted", canAccept: false, canReject: false, canAssign: true };
    const drivers = { items: [{ id: "d1", displayName: "Asha", phoneNumber: "+919800000001", employmentStatus: "active", availabilityStatus: "available", eligible: true }], page: 1, pageSize: 100, totalCount: 1, totalPages: 1 };
    const vehicles = { items: [{ id: "v1", registrationNumber: "KA01AA1001", vehicleType: "sedan", vehicleTypeName: "Sedan", capacity: 4, status: "active", eligible: true }], page: 1, pageSize: 100, totalCount: 1, totalPages: 1 };
    let resolve!: (response: Response) => void; authenticatedFetch.mockResolvedValueOnce(new Response(JSON.stringify(accepted))).mockResolvedValueOnce(new Response(JSON.stringify(drivers))).mockResolvedValueOnce(new Response(JSON.stringify(vehicles))).mockReturnValueOnce(new Promise(done => { resolve = done; }));
    vi.spyOn(window, "confirm").mockReturnValue(true); const user = userEvent.setup(); render(<BookingDetails id={item.id} />); await screen.findByLabelText("Driver"); await user.selectOptions(screen.getByLabelText("Driver"), "d1"); await user.selectOptions(screen.getByLabelText("Vehicle"), "v1"); await user.dblClick(screen.getByRole("button", { name: /review and assign/i }));
    expect(screen.getByRole("button", { name: /assigning/i })).toBeDisabled(); expect(authenticatedFetch).toHaveBeenCalledTimes(4);
    resolve(new Response(JSON.stringify({ detail: "The selected vehicle type does not match the requested category." }), { status: 409 })); expect(await screen.findByRole("alert")).toHaveTextContent(/does not match/i);
  });
  it("shows the development OTP and verifies the real entered code", async () => { auth.user = null; requestOtp.mockResolvedValue(undefined); peekDevOtp.mockResolvedValue("123456"); verifyOtp.mockResolvedValue(undefined); const user = userEvent.setup(); render(<AdminLogin />); await user.type(screen.getByLabelText(/mobile number/i), "9876543210"); await user.click(screen.getByRole("button", { name: /send otp/i })); expect(await screen.findByText(/development code:/i)).toHaveTextContent("123456"); await user.type(screen.getByLabelText(/one-time code/i), "123456"); await user.click(screen.getByRole("button", { name: /verify otp/i })); expect(verifyOtp).toHaveBeenCalledWith("9876543210", "123456"); });
});
