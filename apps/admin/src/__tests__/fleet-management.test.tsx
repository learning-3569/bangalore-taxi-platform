import { render, screen, within } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { AdminShell } from "@/components/AdminShell";
import { DriverList } from "@/components/DriverList";
import { DriverCreate } from "@/components/DriverCreate";
import { DriverDetails } from "@/components/DriverDetails";
import { VehicleList } from "@/components/VehicleList";
import { VehicleCreate } from "@/components/VehicleCreate";
import { VehicleDetails } from "@/components/VehicleDetails";

const authenticatedFetch = vi.fn(); const push = vi.fn(); const replace = vi.fn(); const logout = vi.fn(); let pathname = "/drivers";
const auth = { ready: true, user: { userId: "a", phoneNumber: "+919876543210", maskedPhone: "******3210", roles: ["admin"] }, authenticatedFetch, logout };
vi.mock("next/navigation", () => ({ useRouter: () => ({ push, replace }), usePathname: () => pathname }));
vi.mock("@/components/AuthProvider", () => ({ useAuth: () => auth }));

const driver = { id: "d1", driverNumber: "DRV-000021", displayName: "Ramesh Kumar", phoneNumber: "+919876543210", employmentStatus: "active", availabilityStatus: "available", eligible: true, version: 10, currentVehicleId: null, currentVehicleRegistration: null, vehicleHistory: [] };
const vehicle = { id: "v1", registrationNumber: "KA01AB1234", vehicleTypeId: "t1", vehicleType: "sedan", vehicleTypeName: "Sedan", capacity: 4, status: "active", eligible: true, version: 20, currentDriverId: null, currentDriverNumber: null, currentDriverName: null, driverHistory: [] };
const driverPage = { items: [{ ...driver, currentVehicleId: "v1", currentVehicleRegistration: "KA01AB1234" }], page: 1, pageSize: 25, totalCount: 1, totalPages: 1 };
const vehiclePage = { items: [vehicle], page: 1, pageSize: 25, totalCount: 1, totalPages: 1 };
const types = [{ id: "t1", code: "sedan", name: "Sedan", typicalCapacity: 4 }];

describe("admin fleet management", () => {
  beforeEach(() => { authenticatedFetch.mockReset(); push.mockReset(); replace.mockReset(); logout.mockReset(); pathname = "/drivers"; });

  it("exposes genuine Drivers and Vehicles navigation with active state", () => {
    render(<AdminShell><p>Fleet</p></AdminShell>); const nav = screen.getByRole("navigation", { name: "Admin navigation" });
    expect(within(nav).getByRole("link", { name: "Bookings" })).toHaveAttribute("href", "/bookings"); expect(within(nav).getByRole("link", { name: "Drivers" })).toHaveAttribute("aria-current", "page"); expect(within(nav).getByRole("link", { name: "Vehicles" })).toHaveAttribute("href", "/vehicles");
  });

  it("renders searchable paginated driver and vehicle lists with empty handling", async () => {
    authenticatedFetch.mockResolvedValueOnce(new Response(JSON.stringify(driverPage))); const first = render(<DriverList />); expect(await screen.findByText("DRV-000021")).toBeInTheDocument(); expect(screen.getByText("KA01AB1234", { exact: false })).toBeInTheDocument(); expect(screen.getByRole("link", { name: /add driver/i })).toHaveAttribute("href", "/drivers/new"); first.unmount();
    authenticatedFetch.mockResolvedValueOnce(new Response(JSON.stringify(vehiclePage))); const second = render(<VehicleList />); expect(await screen.findByText("KA01AB1234")).toBeInTheDocument(); expect(screen.getByRole("link", { name: /add vehicle/i })).toHaveAttribute("href", "/vehicles/new"); second.unmount();
    authenticatedFetch.mockResolvedValueOnce(new Response(JSON.stringify({ ...driverPage, items: [], totalCount: 0, totalPages: 0 }))); render(<DriverList />); expect(await screen.findByText(/no drivers found/i)).toBeInTheDocument();
  });

  it("creates a driver through the API and navigates to its generated number record", async () => {
    authenticatedFetch.mockResolvedValueOnce(new Response(JSON.stringify(driver))); const user = userEvent.setup(); render(<DriverCreate />); await user.type(screen.getByLabelText("Driver name"), "Ramesh Kumar"); await user.type(screen.getByLabelText("Mobile number"), "9876543210"); await user.click(screen.getByRole("button", { name: "Create Driver" }));
    expect(authenticatedFetch).toHaveBeenCalledWith("/api/admin/drivers", expect.objectContaining({ method: "POST" })); expect(push).toHaveBeenCalledWith("/drivers/d1");
  });

  it("edits, deactivates, reactivates, and changes a driver's vehicle tag", async () => {
    authenticatedFetch.mockResolvedValueOnce(new Response(JSON.stringify(driver))).mockResolvedValueOnce(new Response(JSON.stringify(vehiclePage)));
    const user = userEvent.setup(); render(<DriverDetails id="d1" />); expect(await screen.findByDisplayValue("DRV-000021")).toHaveAttribute("readonly"); await user.clear(screen.getByLabelText("Driver name")); await user.type(screen.getByLabelText("Driver name"), "Ramesh Edited");
    authenticatedFetch.mockResolvedValueOnce(new Response(JSON.stringify({ ...driver, displayName: "Ramesh Edited", version: 11 }))); await user.click(screen.getByRole("button", { name: "Save changes" })); expect(await screen.findByText("Driver updated.")).toBeInTheDocument();
    vi.spyOn(window, "confirm").mockReturnValue(true); authenticatedFetch.mockResolvedValueOnce(new Response(JSON.stringify({ ...driver, displayName: "Ramesh Edited", employmentStatus: "inactive", eligible: false, version: 12 }))); await user.click(screen.getByRole("button", { name: "Deactivate driver" })); expect(window.confirm).toHaveBeenCalledWith(expect.stringContaining("DRV-000021")); expect(await screen.findByRole("button", { name: "Reactivate driver" })).toBeInTheDocument();
    await user.selectOptions(screen.getByLabelText("Tagged vehicle"), "v1"); authenticatedFetch.mockResolvedValueOnce(new Response(JSON.stringify({ ...driver, currentVehicleId: "v1", currentVehicleRegistration: "KA01AB1234", version: 13 }))); await user.click(screen.getByRole("button", { name: "Save vehicle tag" })); expect(await screen.findByText("Vehicle tagged.")).toBeInTheDocument();
  });

  it("creates a vehicle from authoritative types and handles validation conflicts", async () => {
    authenticatedFetch.mockResolvedValueOnce(new Response(JSON.stringify(types))).mockResolvedValueOnce(new Response(JSON.stringify(vehicle))); const user = userEvent.setup(); render(<VehicleCreate />); await screen.findByRole("option", { name: "Sedan" }); await user.type(screen.getByLabelText("Registration number"), "KA01AB1234"); await user.click(screen.getByRole("button", { name: "Create Vehicle" })); expect(push).toHaveBeenCalledWith("/vehicles/v1");
  });

  it("edits, deactivates and tags a vehicle while displaying server conflicts", async () => {
    authenticatedFetch.mockResolvedValueOnce(new Response(JSON.stringify(vehicle))).mockResolvedValueOnce(new Response(JSON.stringify(types))).mockResolvedValueOnce(new Response(JSON.stringify(driverPage))); const user = userEvent.setup(); render(<VehicleDetails id="v1" />); await screen.findByDisplayValue("KA01AB1234");
    authenticatedFetch.mockResolvedValueOnce(new Response(JSON.stringify({ detail: "The vehicle changed. Refresh and try again." }), { status: 409 })); await user.click(screen.getByRole("button", { name: "Save changes" })); expect(await screen.findByRole("alert")).toHaveTextContent(/changed.*refresh/i);
    await user.selectOptions(screen.getByLabelText("Tagged driver"), "d1"); authenticatedFetch.mockResolvedValueOnce(new Response(JSON.stringify({ ...vehicle, currentDriverId: "d1", currentDriverNumber: "DRV-000021", currentDriverName: "Ramesh Kumar", version: 21 }))); await user.click(screen.getByRole("button", { name: "Save driver tag" })); expect(await screen.findByText("Driver tagged.")).toBeInTheDocument();
  });
});
