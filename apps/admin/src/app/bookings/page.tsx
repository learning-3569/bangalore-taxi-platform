import { AdminGuard } from "@/components/AdminGuard";
import { AdminShell } from "@/components/AdminShell";
import { BookingQueue } from "@/components/BookingQueue";
export default function Page() { return <AdminShell><AdminGuard><BookingQueue /></AdminGuard></AdminShell>; }
