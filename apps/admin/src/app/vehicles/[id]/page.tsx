import { AdminGuard } from "@/components/AdminGuard";
import { AdminShell } from "@/components/AdminShell";
import { VehicleDetails } from "@/components/VehicleDetails";
export default async function Page({ params }: { params: Promise<{ id: string }> }) { const { id } = await params; return <AdminShell><AdminGuard><VehicleDetails id={id} /></AdminGuard></AdminShell>; }
