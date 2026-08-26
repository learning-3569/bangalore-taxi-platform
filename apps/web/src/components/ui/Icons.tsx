export function IconPhone({ className = "h-4 w-4" }: { className?: string }) {
  return (
    <svg className={className} viewBox="0 0 24 24" fill="none" aria-hidden="true">
      <path
        d="M6.5 3.75h3.2l1.1 3.2-1.8 1.1a12.5 12.5 0 0 0 5.95 5.95l1.1-1.8 3.2 1.1v3.2c0 .7-.55 1.3-1.25 1.3C10.7 18.8 5.2 13.3 4.2 5.3c0-.7.6-1.55 1.3-1.55Z"
        stroke="currentColor"
        strokeWidth="1.6"
        strokeLinejoin="round"
      />
    </svg>
  );
}

export function IconPin({ className = "h-4 w-4" }: { className?: string }) {
  return (
    <svg className={className} viewBox="0 0 24 24" fill="none" aria-hidden="true">
      <path d="M12 21s6.5-6.1 6.5-11A6.5 6.5 0 0 0 5.5 10c0 4.9 6.5 11 6.5 11Z" stroke="currentColor" strokeWidth="1.6" />
      <circle cx="12" cy="10" r="2.2" stroke="currentColor" strokeWidth="1.6" />
    </svg>
  );
}

export function IconWhatsApp({ className = "h-6 w-6" }: { className?: string }) {
  return (
    <svg className={className} viewBox="0 0 24 24" aria-hidden="true">
      <path
        fill="currentColor"
        d="M12.04 2.2A9.7 9.7 0 0 0 2.3 11.9a9.6 9.6 0 0 0 1.4 5L2 22l5.25-1.64a9.8 9.8 0 0 0 4.79 1.22h.04A9.7 9.7 0 0 0 12.04 2.2Zm5.72 13.7c-.24.68-1.4 1.3-1.94 1.34-.5.04-1.12.06-1.81-.11-.42-.11-.95-.31-1.63-.6-2.87-1.24-4.74-4.14-4.88-4.33-.14-.19-1.14-1.52-1.14-2.9 0-1.38.72-2.06.98-2.34.24-.27.64-.4.98-.4h.7c.22 0 .53-.1.83.64.3.75 1.03 2.58 1.12 2.77.09.19.15.4.03.64-.12.24-.18.4-.36.61-.18.22-.38.48-.54.65-.18.18-.36.38-.15.74.2.36.9 1.48 1.93 2.4 1.33 1.18 2.45 1.55 2.81 1.73.36.18.57.15.78-.09.21-.24.9-1.05 1.14-1.41.24-.36.48-.3.8-.18.33.12 2.07.98 2.42 1.15.36.18.6.27.68.42.09.15.09.86-.15 1.54Z"
      />
    </svg>
  );
}
