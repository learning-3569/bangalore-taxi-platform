import { AdminGuard } from "@/components/AdminGuard";
import { AdminShell } from "@/components/AdminShell";
import { DriverList } from "@/components/DriverList";
export default function Page() { return <AdminShell><AdminGuard><DriverList /></AdminGuard></AdminShell>; }
