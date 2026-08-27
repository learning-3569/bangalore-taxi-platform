import { cookies } from "next/headers";
import { NextResponse } from "next/server";
import { newCsrfToken, proxyAuth, readRefreshToken, setAuthCookies } from "@/lib/server-auth";

export async function POST(request: Request) {
  const header = request.headers.get("x-csrf-token");
  const store = await cookies();
  const csrf = store.get("bt_csrf")?.value;
  if (!header || !csrf || header !== csrf) {
    return NextResponse.json({ title: "Unauthorized", detail: "Session is no longer valid." }, { status: 401 });
  }
  const refreshToken = await readRefreshToken();
  const upstream = await proxyAuth("/api/v1/auth/refresh", {
    method: "POST",
    body: JSON.stringify({ refreshToken }),
  });
  const json = (await upstream.json()) as {
    accessToken?: string;
    accessTokenExpiresAt?: string;
    refreshToken?: string;
    user?: unknown;
  };
  if (!upstream.ok) {
    return NextResponse.json(json, { status: upstream.status });
  }
  if (json.refreshToken) {
    await setAuthCookies(json.refreshToken, newCsrfToken());
  }
  return NextResponse.json({
    accessToken: json.accessToken,
    accessTokenExpiresAt: json.accessTokenExpiresAt,
    user: json.user,
  });
}
