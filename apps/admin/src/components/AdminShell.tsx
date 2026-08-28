"use client";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { useAuth } from "@/components/AuthProvider";
export function AdminShell({ children }: { children: React.ReactNode }) {
  const auth = useAuth(); const router = useRouter();
  async function logout() { await auth.logout(); router.replace("/login"); }
  return <div className="min-h-screen bg-slate-950 text-slate-100"><header className="border-b border-slate-800 bg-slate-950"><div className="mx-auto flex max-w-7xl items-center justify-between px-4 py-4 sm:px-6"><Link href="/bookings" className="font-semibold">Bangalore Taxi Operations</Link><div className="flex items-center gap-4"><span className="text-xs text-slate-400">{auth.user?.maskedPhone}</span><button className="text-sm text-sky-400 hover:text-sky-300" onClick={() => void logout()}>Sign out</button></div></div></header><main className="mx-auto max-w-7xl px-4 py-8 sm:px-6">{children}</main></div>;
}
