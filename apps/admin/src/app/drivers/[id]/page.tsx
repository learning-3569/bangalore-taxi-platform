import { AdminGuard } from "@/components/AdminGuard";
import { AdminShell } from "@/components/AdminShell";
import { DriverDetails } from "@/components/DriverDetails";
export default async function Page({ params }: { params: Promise<{ id: string }> }) { const { id } = await params; return <AdminShell><AdminGuard><DriverDetails id={id} /></AdminGuard></AdminShell>; }
