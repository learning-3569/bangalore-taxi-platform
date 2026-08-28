"use client";
import { useEffect } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/components/AuthProvider";
export function AdminGuard({ children }: { children: React.ReactNode }) { const auth = useAuth(); const router = useRouter(); useEffect(() => { if (auth.ready && !auth.user) router.replace("/login"); }, [auth.ready, auth.user, router]); if (!auth.ready || !auth.user) return <p role="status" className="text-slate-400">Checking admin access…</p>; if (!auth.user.roles.includes("admin")) return <div role="alert" className="rounded-lg border border-red-900 bg-red-950/40 p-6"><h1 className="text-2xl font-semibold">Access denied</h1><p className="mt-2 text-red-200">Your authenticated account is not authorized for booking operations.</p></div>; return children; }
