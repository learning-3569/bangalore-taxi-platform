"use client";

import { useEffect, useId, useRef, useState } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/components/auth/AuthProvider";

export function AccountMenu({ compact = false }: { compact?: boolean }) {
  const { logout } = useAuth();
  const router = useRouter();
  const [open, setOpen] = useState(false);
  const [loggingOut, setLoggingOut] = useState(false);
  const [error, setError] = useState("");
  const rootRef = useRef<HTMLDivElement>(null);
  const buttonRef = useRef<HTMLButtonElement>(null);
  const menuId = useId();

  useEffect(() => {
    function onPointer(event: MouseEvent) {
      if (!rootRef.current?.contains(event.target as Node)) {
        setOpen(false);
      }
    }
    function onKey(event: KeyboardEvent) {
      if (event.key === "Escape") {
        setOpen(false);
        buttonRef.current?.focus();
      }
    }
    document.addEventListener("mousedown", onPointer);
    document.addEventListener("keydown", onKey);
    return () => {
      document.removeEventListener("mousedown", onPointer);
      document.removeEventListener("keydown", onKey);
    };
  }, []);

  async function onLogout() {
    setError("");
    setLoggingOut(true);
    try {
      await logout();
      setOpen(false);
      router.push("/");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Unable to log out. Please try again.");
      setOpen(true);
    } finally {
      setLoggingOut(false);
    }
  }

  return (
    <div ref={rootRef} className="relative">
      <button
        ref={buttonRef}
        type="button"
        className={`inline-flex items-center gap-1 rounded-sm border border-line bg-paper px-3 py-2 text-sm font-semibold text-navy transition hover:border-navy focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-brand ${compact ? "w-full justify-between" : ""}`}
        aria-expanded={open}
        aria-haspopup="menu"
        aria-controls={menuId}
        onClick={() => setOpen((value) => !value)}
      >
        Account
        <span aria-hidden className="text-ink-muted">
          ▾
        </span>
      </button>
      {open ? (
        <ul
          id={menuId}
          role="menu"
          aria-label="Account"
          className={`z-50 min-w-44 border border-line bg-paper py-1 shadow-[0_12px_28px_rgba(8,24,39,0.12)] ${compact ? "relative mt-2 w-full" : "absolute right-0 mt-2"}`}
        >
          <li role="none">
            <button
              type="button"
              role="menuitem"
              className="flex w-full px-3 py-2 text-left text-sm font-medium text-navy hover:bg-paper-soft focus-visible:bg-paper-soft focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-inset focus-visible:ring-brand disabled:text-ink-muted"
              disabled={loggingOut}
              aria-busy={loggingOut}
              onClick={() => void onLogout()}
            >
              {loggingOut ? "Logging out…" : "Logout"}
            </button>
          </li>
        </ul>
      ) : null}
      {error ? (
        <p role="alert" className="mt-2 text-xs text-red-700">
          {error}
        </p>
      ) : null}
    </div>
  );
}
