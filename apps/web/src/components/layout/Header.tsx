"use client";

import Link from "next/link";
import { useEffect, useId, useRef, useState } from "react";
import { Logo } from "@/components/brand/Logo";
import { Button } from "@/components/ui/Button";
import { Container } from "@/components/ui/Container";
import { IconPhone } from "@/components/ui/Icons";
import { businessPlaceholders, navItems, siteConfig } from "@/config/site";

export function Header() {
  const [open, setOpen] = useState(false);
  const menuId = useId();
  const menuButtonRef = useRef<HTMLButtonElement>(null);
  const firstLinkRef = useRef<HTMLAnchorElement>(null);

  function closeMenu() {
    setOpen(false);
    menuButtonRef.current?.focus();
  }

  useEffect(() => {
    document.body.style.overflow = open ? "hidden" : "";
    return () => {
      document.body.style.overflow = "";
    };
  }, [open]);

  useEffect(() => {
    if (open) firstLinkRef.current?.focus();
  }, [open]);

  useEffect(() => {
    function onKey(event: KeyboardEvent) {
      if (event.key === "Escape") {
        setOpen(false);
        menuButtonRef.current?.focus();
      }
    }
    window.addEventListener("keydown", onKey);
    return () => window.removeEventListener("keydown", onKey);
  }, []);

  return (
    <header className="sticky top-0 z-40">
      <div className="hidden border-b border-white/10 bg-navy text-[11px] tracking-wide text-white/70 md:block">
        <Container className="flex h-8 items-center justify-between gap-4">
          <p>
            24/7 customer support · <span className="text-white/90">{businessPlaceholders.phone}</span>
          </p>
          <p className="hidden gap-4 lg:flex">
            <span>Live tracking — coming with driver assignment</span>
            <span className="text-taxi">Verified drivers</span>
            <span>Safe &amp; secure desk handling</span>
          </p>
        </Container>
      </div>
      <div className="border-b border-line bg-paper/95 backdrop-blur-sm">
        <Container className="flex h-[4.75rem] items-center justify-between gap-4">
          <Link href="/" aria-label={`${siteConfig.name} home`} className="shrink-0">
            <Logo variant="header" />
          </Link>

          <nav aria-label="Primary" className="hidden items-center gap-6 lg:flex">
            {navItems.map((item) => (
              <Link
                key={item.href}
                href={item.href}
                className="text-[13px] font-medium text-ink-muted transition hover:text-navy"
              >
                {item.label}
              </Link>
            ))}
          </nav>

          <div className="hidden items-center gap-3 lg:flex">
            <Button href="/#book" variant="taxi" className="uppercase">
              Book a cab
            </Button>
          </div>

          <div className="flex items-center gap-2 lg:hidden">
            <Link
              href="/#contact"
              className="grid h-10 w-10 place-items-center rounded-sm border border-line text-navy"
              aria-label="Contact the booking desk"
            >
              <IconPhone />
            </Link>
            <Button href="/#book" variant="taxi" className="px-3 py-2 text-xs uppercase">
              Book
            </Button>
            <button
              ref={menuButtonRef}
              type="button"
              className="inline-flex h-10 w-10 items-center justify-center rounded-sm border border-line text-navy"
              aria-expanded={open}
              aria-controls={menuId}
              aria-haspopup="true"
              onClick={() => setOpen((value) => !value)}
            >
              <span className="sr-only">{open ? "Close menu" : "Open menu"}</span>
              <span aria-hidden className="flex flex-col gap-1.5">
                <span className={`block h-0.5 w-5 bg-navy transition ${open ? "translate-y-2 rotate-45" : ""}`} />
                <span className={`block h-0.5 w-5 bg-navy transition ${open ? "opacity-0" : ""}`} />
                <span className={`block h-0.5 w-5 bg-navy transition ${open ? "-translate-y-2 -rotate-45" : ""}`} />
              </span>
            </button>
          </div>
        </Container>
      </div>

      {open ? (
        <div id={menuId} className="border-b border-line bg-paper lg:hidden">
          <Container className="flex flex-col gap-1 py-4" as="nav" aria-label="Mobile">
            {navItems.map((item, index) => (
              <Link
                key={item.href}
                ref={index === 0 ? firstLinkRef : undefined}
                href={item.href}
                className="rounded-sm px-2 py-3 text-base font-medium text-navy hover:bg-paper-soft"
                onClick={closeMenu}
              >
                {item.label}
              </Link>
            ))}
            <Button href="/#book" variant="taxi" className="mt-2 uppercase" onClick={closeMenu}>
              Book a cab
            </Button>
          </Container>
        </div>
      ) : null}
    </header>
  );
}
