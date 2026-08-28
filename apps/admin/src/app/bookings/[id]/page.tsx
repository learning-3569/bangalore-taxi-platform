import { AdminGuard } from "@/components/AdminGuard";
import { AdminShell } from "@/components/AdminShell";
import { BookingDetails } from "@/components/BookingDetails";
export default async function Page({ params }: { params: Promise<{ id: string }> }) { const { id } = await params; return <AdminShell><AdminGuard><BookingDetails id={id} /></AdminGuard></AdminShell>; }
