import { NextResponse } from "next/server";
import { proxyAuth } from "@/lib/server-auth";

export async function POST(request: Request) {
  const body = await request.json();
  const upstream = await proxyAuth("/api/v1/auth/otp/request", {
    method: "POST",
    body: JSON.stringify(body),
  });
  const data = await upstream.text();
  const headers = new Headers({
    "Content-Type": upstream.headers.get("Content-Type") ?? "application/json",
  });
  const retryAfter = upstream.headers.get("Retry-After");
  if (retryAfter) {
    headers.set("Retry-After", retryAfter);
  }
  return new NextResponse(data, {
    status: upstream.status,
    headers,
  });
}
