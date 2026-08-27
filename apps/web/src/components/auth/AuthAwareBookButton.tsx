"use client";

import type { ReactNode } from "react";
import { useAuth } from "@/components/auth/AuthProvider";
import { Button } from "@/components/ui/Button";
import { loginHref, type BookingIntent } from "@/lib/booking-intent";

type Variant = "primary" | "secondary" | "taxi" | "outline";

export function AuthAwareBookButton({
  authedHref,
  intent,
  children,
  variant = "taxi",
  className,
}: {
  authedHref: string;
  intent: BookingIntent;
  children: ReactNode;
  variant?: Variant;
  className?: string;
}) {
  const { user } = useAuth();
  return (
    <Button href={user ? authedHref : loginHref(intent)} variant={variant} className={className}>
      {children}
    </Button>
  );
}
