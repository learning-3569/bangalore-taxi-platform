"use client";

import Link from "next/link";
import { usePathname, useRouter } from "next/navigation";
import { useState } from "react";
import { useAuth } from "@/components/AuthProvider";

const navigation = [
  { href: "/bookings", label: "Bookings" },
  { href: "/drivers", label: "Drivers" },
  { href: "/vehicles", label: "Vehicles" },
];

export function AdminShell({ children }: { children: React.ReactNode }) {
  const auth = useAuth();
  const pathname = usePathname();
  const router = useRouter();
  const [menuOpen, setMenuOpen] = useState(false);
  const [loggingOut, setLoggingOut] = useState(false);

  async function logout() {
    if (loggingOut) return;
    setLoggingOut(true);
    try {
      await auth.logout();
      router.replace("/login");
    } finally {
      setLoggingOut(false);
    }
  }

  function isActive(href: string) {
    return pathname === href || pathname.startsWith(`${href}/`);
  }

  const navLinks = navigation.map(item => (
    <Link
      key={item.href}
      href={item.href}
      aria-current={isActive(item.href) ? "page" : undefined}
      onClick={() => setMenuOpen(false)}
      className={`rounded px-3 py-2 text-sm font-medium transition-colors ${
        isActive(item.href)
          ? "bg-sky-500/15 text-sky-300"
          : "text-slate-300 hover:bg-slate-800 hover:text-white"
      }`}
    >
      {item.label}
    </Link>
  ));

  return (
    <div className="min-h-screen bg-slate-950 text-slate-100">
      <header className="border-b border-slate-800 bg-slate-950">
        <div className="mx-auto max-w-7xl px-4 sm:px-6">
          <div className="flex min-h-16 items-center justify-between gap-4">
            <div className="flex min-w-0 items-center gap-8">
              <Link href="/bookings" className="truncate font-semibold text-white">
                Bangalore Taxi Admin
              </Link>
              <nav aria-label="Admin navigation" className="hidden items-center gap-1 md:flex">
                {navLinks}
              </nav>
            </div>

            <div className="hidden shrink-0 items-center gap-4 md:flex">
              <span className="text-xs text-slate-400" aria-label="Admin account">
                {auth.user?.maskedPhone}
              </span>
              <button type="button" disabled={loggingOut} className="text-sm font-medium text-sky-400 hover:text-sky-300 disabled:opacity-50" onClick={() => void logout()}>
                {loggingOut ? "Signing out…" : "Sign out"}
              </button>
            </div>

            <button type="button" aria-label="Toggle admin menu" aria-expanded={menuOpen} aria-controls="admin-mobile-menu" onClick={() => setMenuOpen(value => !value)} className="shrink-0 rounded border border-slate-700 px-3 py-2 text-sm font-medium text-slate-200 md:hidden">
              Menu
            </button>
          </div>

          {menuOpen ? (
            <div id="admin-mobile-menu" className="border-t border-slate-800 py-3 md:hidden">
              <nav aria-label="Mobile admin navigation" className="grid gap-1">{navLinks}</nav>
              <div className="mt-3 flex items-center justify-between gap-3 border-t border-slate-800 pt-3">
                <span className="truncate text-xs text-slate-400" aria-label="Admin account">{auth.user?.maskedPhone}</span>
                <button type="button" disabled={loggingOut} className="shrink-0 text-sm font-medium text-sky-400 disabled:opacity-50" onClick={() => void logout()}>
                  {loggingOut ? "Signing out…" : "Sign out"}
                </button>
              </div>
            </div>
          ) : null}
        </div>
      </header>
      <main className="mx-auto max-w-7xl px-4 py-8 sm:px-6">{children}</main>
    </div>
  );
}
