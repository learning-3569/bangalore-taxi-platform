import { NextResponse } from "next/server";
import { apiBaseUrl } from "@/lib/server-auth";

export async function GET(request: Request) {
  const upstream = await fetch(`${apiBaseUrl()}/api/v1/admin/vehicles${new URL(request.url).search}`, {
    headers: { Authorization: request.headers.get("authorization") ?? "" }, cache: "no-store",
  });
  return new NextResponse(await upstream.text(), { status: upstream.status,
    headers: { "Content-Type": upstream.headers.get("content-type") ?? "application/json" } });
}

export async function POST(request: Request) {
  const upstream = await fetch(`${apiBaseUrl()}/api/v1/admin/vehicles`, {
    method: "POST", headers: { "Content-Type": "application/json", Authorization: request.headers.get("authorization") ?? "" },
    body: await request.text(), cache: "no-store",
  });
  return new NextResponse(await upstream.text(), { status: upstream.status,
    headers: { "Content-Type": upstream.headers.get("content-type") ?? "application/json" } });
}
