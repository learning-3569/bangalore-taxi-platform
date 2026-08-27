import { cookies } from "next/headers";

const REFRESH = "bt_refresh";
const CSRF = "bt_csrf";

export function apiBaseUrl(): string {
  return process.env.API_BASE_URL ?? process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://127.0.0.1:43130";
}

export function cookieOptions() {
  const secure = process.env.NODE_ENV === "production";
  return {
    httpOnly: true,
    secure,
    sameSite: "lax" as const,
    path: "/",
  };
}

export async function setAuthCookies(refreshToken: string, csrfToken: string) {
  const store = await cookies();
  store.set(REFRESH, refreshToken, { ...cookieOptions(), httpOnly: true });
  store.set(CSRF, csrfToken, { ...cookieOptions(), httpOnly: false });
}

export async function clearAuthCookies() {
  const store = await cookies();
  store.delete({ name: REFRESH, path: "/" });
  store.delete({ name: CSRF, path: "/" });
}

export async function readRefreshToken(): Promise<string | undefined> {
  const store = await cookies();
  return store.get(REFRESH)?.value;
}

export function newCsrfToken(): string {
  const bytes = new Uint8Array(16);
  crypto.getRandomValues(bytes);
  return Array.from(bytes, (b) => b.toString(16).padStart(2, "0")).join("");
}

export async function proxyAuth(path: string, init: RequestInit = {}): Promise<Response> {
  return fetch(`${apiBaseUrl()}${path}`, {
    ...init,
    headers: {
      "Content-Type": "application/json",
      "X-Auth-Client": "bearer",
      ...(init.headers ?? {}),
    },
    cache: "no-store",
  });
}
