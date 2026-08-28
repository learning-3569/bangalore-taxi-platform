import { cookies } from "next/headers";

const REFRESH = "bt_admin_refresh";
const CSRF = "bt_admin_csrf";

export function apiBaseUrl(): string {
  return process.env.API_BASE_URL ?? process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://127.0.0.1:43130";
}
const options = () => ({ secure: process.env.NODE_ENV === "production", sameSite: "lax" as const, path: "/" });
export async function setAuthCookies(refreshToken: string, csrfToken: string) {
  const store = await cookies(); store.set(REFRESH, refreshToken, { ...options(), httpOnly: true }); store.set(CSRF, csrfToken, { ...options(), httpOnly: false });
}
export async function clearAuthCookies() { const store = await cookies(); store.delete({ name: REFRESH, path: "/" }); store.delete({ name: CSRF, path: "/" }); }
export async function readRefreshToken() { return (await cookies()).get(REFRESH)?.value; }
export function newCsrfToken() { const bytes = new Uint8Array(16); crypto.getRandomValues(bytes); return Array.from(bytes, b => b.toString(16).padStart(2, "0")).join(""); }
export function proxyAuth(path: string, init: RequestInit = {}) {
  return fetch(`${apiBaseUrl()}${path}`, { ...init, headers: { "Content-Type": "application/json", "X-Auth-Client": "bearer", ...(init.headers ?? {}) }, cache: "no-store" });
}
