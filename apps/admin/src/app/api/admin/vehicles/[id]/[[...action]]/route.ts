import { NextResponse } from "next/server";
import { apiBaseUrl } from "@/lib/server-auth";

async function proxy(request: Request, context: { params: Promise<{ id: string; action?: string[] }> }) {
  const { id, action = [] } = await context.params;
  const upstream = await fetch(`${apiBaseUrl()}/api/v1/admin/vehicles/${id}${action.length ? `/${action.join("/")}` : ""}`, {
    method: request.method, headers: { "Content-Type": "application/json", Authorization: request.headers.get("authorization") ?? "" },
    body: request.method === "GET" ? undefined : await request.text(), cache: "no-store",
  });
  return new NextResponse(await upstream.text(), { status: upstream.status, headers: { "Content-Type": upstream.headers.get("content-type") ?? "application/json" } });
}
export const GET = proxy; export const PUT = proxy; export const POST = proxy;
