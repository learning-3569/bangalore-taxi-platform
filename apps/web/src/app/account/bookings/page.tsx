import type { Metadata } from "next";
import { MyBookings } from "@/components/booking/MyBookings";
import { Container } from "@/components/ui/Container";

export const metadata: Metadata = { title: "My bookings", robots: { index: false, follow: false } };
export default function Page() { return <main className="bg-paper-soft py-10"><Container className="max-w-3xl"><h1 className="mb-6 font-display text-3xl font-semibold text-navy">My bookings</h1><MyBookings /></Container></main>; }
