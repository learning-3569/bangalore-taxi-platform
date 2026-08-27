import { cookies } from "next/headers";
import { NextResponse } from "next/server";
import { clearAuthCookies, proxyAuth, readRefreshToken } from "@/lib/server-auth";

export async function POST(request: Request) {
  const header = request.headers.get("x-csrf-token");
  const store = await cookies();
  const csrf = store.get("bt_csrf")?.value;
  if (csrf && header !== csrf) {
    return NextResponse.json({ title: "Unauthorized", detail: "Session is no longer valid." }, { status: 401 });
  }
  const refreshToken = await readRefreshToken();
  try {
    const upstream = await proxyAuth("/api/v1/auth/logout", {
      method: "POST",
      body: JSON.stringify({ refreshToken }),
    });
    if (!upstream.ok && upstream.status >= 500) {
      return NextResponse.json(
        { title: "Unable to log out", detail: "Please try again. You are still signed in." },
        { status: 502 },
      );
    }
  } catch {
    return NextResponse.json(
      { title: "Unable to log out", detail: "Please try again. You are still signed in." },
      { status: 502 },
    );
  }
  await clearAuthCookies();
  return new NextResponse(null, { status: 204 });
}
