import { NextResponse } from "next/server";
import { apiBaseUrl } from "@/lib/server-auth";

export async function GET(request: Request) {
  if (process.env.NODE_ENV === "production") {
    return NextResponse.json({ title: "Not Found" }, { status: 404 });
  }
  const phone = new URL(request.url).searchParams.get("phoneNumber") ?? "";
  const upstream = await fetch(
    `${apiBaseUrl()}/api/v1/auth/otp/dev-peek?phoneNumber=${encodeURIComponent(phone)}`,
    { cache: "no-store" },
  );
  const data = await upstream.text();
  return new NextResponse(data, {
    status: upstream.status,
    headers: { "Content-Type": upstream.headers.get("Content-Type") ?? "application/json" },
  });
}
