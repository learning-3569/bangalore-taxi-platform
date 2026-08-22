import type { Metadata } from "next";
import { Geist, Geist_Mono } from "next/font/google";
import "./globals.css";

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

const siteUrl = process.env.NEXT_PUBLIC_SITE_URL ?? "http://127.0.0.1:43121";

export const metadata: Metadata = {
  metadataBase: new URL(siteUrl),
  title: {
    default: "Bangalore Taxi | Airport, Outstation and Local Cab Booking",
    template: "%s | Bangalore Taxi",
  },
  description:
    "Book taxis in Bangalore for airport transfers, outstation trips, and local travel. Advance booking for a 20-car Bangalore fleet.",
  alternates: {
    canonical: "/",
  },
  openGraph: {
    type: "website",
    locale: "en_IN",
    siteName: "Bangalore Taxi",
    title: "Bangalore Taxi | Airport, Outstation and Local Cab Booking",
    description:
      "Book taxis in Bangalore for airport transfers, outstation trips, and local travel.",
    url: "/",
  },
  robots: {
    index: true,
    follow: true,
  },
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en-IN">
      <body
        className={`${geistSans.variable} ${geistMono.variable} bg-stone-50 text-stone-900 antialiased`}
      >
        {children}
      </body>
    </html>
  );
}
