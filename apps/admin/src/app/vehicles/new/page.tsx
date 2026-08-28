import { AdminGuard } from "@/components/AdminGuard";
import { AdminShell } from "@/components/AdminShell";
import { VehicleCreate } from "@/components/VehicleCreate";
export default function Page() { return <AdminShell><AdminGuard><VehicleCreate /></AdminGuard></AdminShell>; }
