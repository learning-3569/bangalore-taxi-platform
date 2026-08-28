import { AdminGuard } from "@/components/AdminGuard";
import { AdminShell } from "@/components/AdminShell";
import { DriverCreate } from "@/components/DriverCreate";
export default function Page() { return <AdminShell><AdminGuard><DriverCreate /></AdminGuard></AdminShell>; }
