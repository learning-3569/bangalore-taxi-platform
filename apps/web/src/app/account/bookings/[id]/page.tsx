import type { Metadata } from "next";
import { BookingDetails } from "@/components/booking/BookingDetails";
import { Container } from "@/components/ui/Container";

export const metadata: Metadata = { title: "Booking details", robots: { index: false, follow: false } };
export default async function Page({ params }: { params: Promise<{ id: string }> }) { const { id } = await params; return <main className="bg-paper-soft py-10"><Container className="max-w-3xl"><BookingDetails id={id} /></Container></main>; }
