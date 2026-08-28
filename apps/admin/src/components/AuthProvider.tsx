"use client";
import { createContext, useCallback, useContext, useEffect, useMemo, useState } from "react";
export type AuthUser = { userId: string; customerId?: string | null; phoneNumber: string; maskedPhone: string; roles: string[] };
type Context = { user: AuthUser | null; ready: boolean; requestOtp(phone: string): Promise<void>; peekDevOtp(phone: string): Promise<string | null>; verifyOtp(phone: string, otp: string): Promise<void>; logout(): Promise<void>; authenticatedFetch(input: RequestInfo | URL, init?: RequestInit): Promise<Response> };
const Auth = createContext<Context | null>(null);
const csrf = () => document.cookie.match(/(?:^|; )bt_admin_csrf=([^;]*)/)?.[1] ?? "";
async function message(response: Response) { try { const body = await response.json() as { detail?: string; title?: string }; return body.detail ?? body.title ?? "Authentication failed."; } catch { return "Authentication failed."; } }
export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<AuthUser | null>(null); const [token, setToken] = useState<string | null>(null); const [ready, setReady] = useState(false);
  const refresh = useCallback(async () => { try { const response = await fetch("/api/auth/refresh", { method: "POST", headers: { "X-CSRF-Token": decodeURIComponent(csrf()) } }); if (!response.ok) { setUser(null); setToken(null); return null; } const body = await response.json() as { accessToken?: string; user?: AuthUser }; setUser(body.user ?? null); setToken(body.accessToken ?? null); return body.accessToken ?? null; } catch { setUser(null); setToken(null); return null; } }, []);
  useEffect(() => { void refresh().finally(() => setReady(true)); }, [refresh]);
  const requestOtp = useCallback(async (phoneNumber: string) => { const response = await fetch("/api/auth/otp/request", { method: "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify({ phoneNumber }) }); if (!response.ok) throw new Error(await message(response)); }, []);
  const peekDevOtp = useCallback(async (phoneNumber: string) => { if (process.env.NODE_ENV === "production") return null; const response = await fetch(`/api/auth/otp/dev-peek?phoneNumber=${encodeURIComponent(phoneNumber)}`); if (!response.ok) return null; const body = await response.json() as { otp?: string }; return body.otp ?? null; }, []);
  const verifyOtp = useCallback(async (phoneNumber: string, otp: string) => { const response = await fetch("/api/auth/otp/verify", { method: "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify({ phoneNumber, otp }) }); if (!response.ok) throw new Error(await message(response)); const body = await response.json() as { accessToken?: string; user?: AuthUser }; setUser(body.user ?? null); setToken(body.accessToken ?? null); }, []);
  const logout = useCallback(async () => { const response = await fetch("/api/auth/logout", { method: "POST", headers: { "X-CSRF-Token": decodeURIComponent(csrf()) } }); if (!response.ok) throw new Error(await message(response)); setUser(null); setToken(null); }, []);
  const authenticatedFetch = useCallback(async (input: RequestInfo | URL, init: RequestInit = {}) => { let access = token ?? await refresh(); const send = (value: string | null) => fetch(input, { ...init, headers: { ...(init.headers ?? {}), ...(value ? { Authorization: `Bearer ${value}` } : {}) } }); let response = await send(access); if (response.status === 401) { access = await refresh(); response = await send(access); } return response; }, [refresh, token]);
  const value = useMemo(() => ({ user, ready, requestOtp, peekDevOtp, verifyOtp, logout, authenticatedFetch }), [user, ready, requestOtp, peekDevOtp, verifyOtp, logout, authenticatedFetch]);
  return <Auth.Provider value={value}>{children}</Auth.Provider>;
}
export function useAuth() { const value = useContext(Auth); if (!value) throw new Error("useAuth requires AuthProvider"); return value; }
