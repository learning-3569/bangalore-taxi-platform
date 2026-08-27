import { NextResponse } from "next/server";
import { apiBaseUrl } from "@/lib/server-auth";

export async function GET(request: Request) {
  const auth = request.headers.get("authorization");
  if (!auth) {
    return NextResponse.json({ title: "Unauthorized", detail: "Session is no longer valid." }, { status: 401 });
  }
  const upstream = await fetch(`${apiBaseUrl()}/api/v1/auth/me`, {
    headers: { Authorization: auth },
    cache: "no-store",
  });
  const data = await upstream.text();
  return new NextResponse(data, {
    status: upstream.status,
    headers: { "Content-Type": upstream.headers.get("Content-Type") ?? "application/json" },
  });
}
