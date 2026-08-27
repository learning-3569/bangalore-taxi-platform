import { NextResponse } from "next/server";
import { newCsrfToken, proxyAuth, setAuthCookies } from "@/lib/server-auth";

export async function POST(request: Request) {
  const body = await request.json();
  const upstream = await proxyAuth("/api/v1/auth/otp/verify", {
    method: "POST",
    body: JSON.stringify(body),
  });
  const json = (await upstream.json()) as {
    accessToken?: string;
    accessTokenExpiresAt?: string;
    refreshToken?: string;
    user?: unknown;
    title?: string;
    detail?: string;
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
