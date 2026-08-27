import { NextResponse } from "next/server";
import { apiBaseUrl } from "@/lib/server-auth";

async function proxy(request: Request, context: { params: Promise<{ path?: string[] }> }) {
  const { path = [] } = await context.params;
  const upstream = await fetch(`${apiBaseUrl()}/api/v1/bookings${path.length ? `/${path.join("/")}` : ""}`, {
    method: request.method,
    headers: {
      "Content-Type": "application/json",
      Authorization: request.headers.get("authorization") ?? "",
      "Idempotency-Key": request.headers.get("idempotency-key") ?? "",
    },
    body: request.method === "GET" ? undefined : await request.text(),
    cache: "no-store",
  });
  const body = await upstream.text();
  return new NextResponse(body || null, { status: upstream.status, headers: { "Content-Type": upstream.headers.get("content-type") ?? "application/json" } });
}

export const GET = proxy;
export const POST = proxy;
