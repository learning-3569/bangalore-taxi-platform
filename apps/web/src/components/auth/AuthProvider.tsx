"use client";

import { createContext, useCallback, useContext, useEffect, useMemo, useState } from "react";
import { OtpCooldownError, parseRetryAfterSeconds } from "@/lib/otp-cooldown";

export type AuthUser = {
  userId: string;
  customerId?: string | null;
  phoneNumber: string;
  maskedPhone: string;
  roles: string[];
};

type AuthContextValue = {
  user: AuthUser | null;
  accessToken: string | null;
  ready: boolean;
  requestOtp: (phoneNumber: string) => Promise<{ resendAvailableInSeconds: number }>;
  verifyOtp: (phoneNumber: string, otp: string) => Promise<void>;
  logout: () => Promise<void>;
  peekDevOtp: (phoneNumber: string) => Promise<string | null>;
  authenticatedFetch: (input: RequestInfo | URL, init?: RequestInit) => Promise<Response>;
};

const AuthContext = createContext<AuthContextValue | null>(null);

function csrfToken(): string {
  const match = document.cookie.match(/(?:^|; )bt_csrf=([^;]*)/);
  return match ? decodeURIComponent(match[1]) : "";
}

async function readProblem(response: Response): Promise<{
  message: string;
  retryAfterSeconds: number | null;
  phoneCooldown: boolean;
  status: number;
}> {
  try {
    const data = (await response.json()) as { detail?: string; title?: string; retryAfterSeconds?: unknown };
    return {
      message: data.detail ?? data.title ?? "Something went wrong.",
      retryAfterSeconds: parseRetryAfterSeconds(response, data),
      phoneCooldown: typeof data.retryAfterSeconds === "number" && data.retryAfterSeconds > 0,
      status: response.status,
    };
  } catch {
    return {
      message: "Something went wrong.",
      retryAfterSeconds: parseRetryAfterSeconds(response, {}),
      phoneCooldown: false,
      status: response.status,
    };
  }
}

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<AuthUser | null>(null);
  const [accessToken, setAccessToken] = useState<string | null>(null);
  const [ready, setReady] = useState(false);

  const applySession = useCallback((payload: { accessToken?: string; user?: AuthUser }) => {
    setAccessToken(payload.accessToken ?? null);
    setUser(payload.user ?? null);
  }, []);

  const refresh = useCallback(async (): Promise<string | null> => {
    try {
      const response = await fetch("/api/auth/refresh", {
        method: "POST",
        headers: { "X-CSRF-Token": csrfToken() },
      });
      if (!response.ok) {
        setUser(null);
        setAccessToken(null);
        return null;
      }
      const payload = (await response.json()) as { accessToken?: string; user?: AuthUser };
      applySession(payload);
      return payload.accessToken ?? null;
    } catch {
      setUser(null);
      setAccessToken(null);
      return null;
    }
  }, [applySession]);

  useEffect(() => {
    void refresh().finally(() => setReady(true));
  }, [refresh]);

  const requestOtp = useCallback(async (phoneNumber: string) => {
    const response = await fetch("/api/auth/otp/request", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ phoneNumber }),
    });
    if (!response.ok) {
      const problem = await readProblem(response);
      if (response.status === 429 && problem.phoneCooldown && problem.retryAfterSeconds) {
        throw new OtpCooldownError(problem.retryAfterSeconds);
      }
      if (response.status === 429) {
        throw new Error("Too many verification requests. Please try again later.");
      }
      throw new Error(problem.message);
    }
    return (await response.json()) as { resendAvailableInSeconds: number };
  }, []);

  const verifyOtp = useCallback(
    async (phoneNumber: string, otp: string) => {
      const response = await fetch("/api/auth/otp/verify", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ phoneNumber, otp }),
      });
      if (!response.ok) {
        throw new Error((await readProblem(response)).message);
      }
      applySession((await response.json()) as { accessToken?: string; user?: AuthUser });
    },
    [applySession],
  );

  const logout = useCallback(async () => {
    const response = await fetch("/api/auth/logout", {
      method: "POST",
      credentials: "same-origin",
      headers: { "X-CSRF-Token": csrfToken() },
    });
    if (!response.ok) {
      throw new Error((await readProblem(response)).message);
    }
    setUser(null);
    setAccessToken(null);
  }, []);

  const peekDevOtp = useCallback(async (phoneNumber: string) => {
    if (process.env.NODE_ENV === "production") return null;
    const response = await fetch(`/api/auth/otp/dev-peek?phoneNumber=${encodeURIComponent(phoneNumber)}`);
    if (!response.ok) return null;
    const data = (await response.json()) as { otp?: string };
    return data.otp ?? null;
  }, []);

  const authenticatedFetch = useCallback(async (input: RequestInfo | URL, init: RequestInit = {}) => {
    let token = accessToken;
    if (!token) token = await refresh();
    const send = (value: string | null) => fetch(input, {
      ...init,
      headers: { ...(init.headers ?? {}), ...(value ? { Authorization: `Bearer ${value}` } : {}) },
    });
    let response = await send(token);
    if (response.status === 401) response = await send(await refresh());
    return response;
  }, [accessToken, refresh]);

  const value = useMemo(
    () => ({ user, accessToken, ready, requestOtp, verifyOtp, logout, peekDevOtp, authenticatedFetch }),
    [user, accessToken, ready, requestOtp, verifyOtp, logout, peekDevOtp, authenticatedFetch],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth(): AuthContextValue {
  const value = useContext(AuthContext);
  if (!value) {
    throw new Error("useAuth must be used within AuthProvider");
  }
  return value;
}
