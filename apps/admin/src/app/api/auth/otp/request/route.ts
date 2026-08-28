import { NextResponse } from "next/server";
import { proxyAuth } from "@/lib/server-auth";
export async function POST(request: Request) { const upstream = await proxyAuth("/api/v1/auth/otp/request", { method: "POST", body: await request.text() }); const body = await upstream.text(); const headers = new Headers({ "Content-Type": upstream.headers.get("content-type") ?? "application/json" }); const retry = upstream.headers.get("retry-after"); if (retry) headers.set("Retry-After", retry); return new NextResponse(body, { status: upstream.status, headers }); }
