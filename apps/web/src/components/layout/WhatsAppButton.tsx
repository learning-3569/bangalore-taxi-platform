"use client";

import { getWhatsAppNumber, businessPlaceholders } from "@/config/site";
import { IconWhatsApp } from "@/components/ui/Icons";

export function WhatsAppButton() {
  const number = getWhatsAppNumber();
  const href = number
    ? `https://wa.me/${number}`
    : "/#contact";
  const label = number
    ? "Chat on WhatsApp"
    : `WhatsApp: ${businessPlaceholders.whatsapp}`;

  return (
    <a
      href={href}
      className="fixed right-4 z-50 grid h-14 w-14 place-items-center rounded-full bg-[#25D366] text-white shadow-[0_8px_20px_rgba(37,211,102,0.35)] transition hover:scale-105 max-md:bottom-[max(5.5rem,env(safe-area-inset-bottom))] md:bottom-6"
      aria-label={label}
      rel={number ? "noopener noreferrer" : undefined}
      target={number ? "_blank" : undefined}
    >
      <IconWhatsApp className="h-7 w-7" />
    </a>
  );
}
