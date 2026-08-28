import { AdminGuard } from "@/components/AdminGuard";
import { AdminShell } from "@/components/AdminShell";
import { VehicleList } from "@/components/VehicleList";
export default function Page() { return <AdminShell><AdminGuard><VehicleList /></AdminGuard></AdminShell>; }
