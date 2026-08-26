const NAVY = "#0A1F3D";
const GOLD = "#FFB703";
const SUN = "#F59E0B";

type LogoProps = {
  variant?: "header" | "footer" | "mark";
  className?: string;
};

function Emblem({ invert = false }: { invert?: boolean }) {
  const ink = invert ? "#FFFFFF" : NAVY;

  return (
    <svg viewBox="0 0 220 148" className="h-full w-full" aria-hidden="true">
      <circle cx="118" cy="66" r="40" fill={SUN} />
      <rect x="78" y="60" width="80" height="3.4" fill="#fff" />
      <rect x="78" y="70.5" width="80" height="3.4" fill="#fff" />

      <path
        d="M24 108c28-22 62-28 96-16 28 10 52 6 78-16"
        fill="none"
        stroke={ink}
        strokeWidth="5.2"
        strokeLinecap="round"
      />
      <path
        d="M18 118c34-18 72-22 108-8 28 11 50 6 78-18"
        fill="none"
        stroke={GOLD}
        strokeWidth="6.4"
        strokeLinecap="round"
      />

      <g fill={ink}>
        <path d="M54 128c-1.6-18-1.2-34 1.4-48-12-7-24-6-30-18 10 4 18 2 26 10-8-14-16-24-10-38 8 12 14 20 18 32 4-16 14-28 28-26-12 8-16 18-18 30 12-4 24-2 32-10-10 10-18 14-28 18 2 16 1 32-2 50h-17.4Z" />
        <path d="M78 128c-1.2-14-.8-26 1.2-38-9-5-18-5-23-14 8 3 14 1.5 20 8-6-11-12-19-7-30 6 9 11 16 14 25 3-12 11-22 22-20-9 6-13 14-14 24 9-3 18-1.5 24-8-8 8-14 11-22 14 1.4 12 .8 25-1.2 39H78Z" />
      </g>

      <g fill={ink} transform="translate(148 18) rotate(-22)">
        <path d="M4 18 52 6l6 4-18 8 14 16-8 2-16-14-12 3 2-6Z" />
        <path d="M24 10 32-2l6 2-4 12Z" />
        <path d="M46 8h10l-2 4H46Z" />
      </g>

      <g fill="none" stroke={ink} strokeWidth="2.1" strokeLinecap="round">
        <path d="M168 16c2.2-2.4 4.4-2.4 6.6 0" />
        <path d="M174.6 16c2.2-2.4 4.4-2.4 6.6 0" />
        <path d="M178 28c1.8-2 3.6-2 5.4 0" />
        <path d="M183.4 28c1.8-2 3.6-2 5.4 0" />
        <path d="M164 30c1.6-1.8 3.2-1.8 4.8 0" />
        <path d="M168.8 30c1.6-1.8 3.2-1.8 4.8 0" />
      </g>
    </svg>
  );
}

function Taxi({ className }: { className?: string }) {
  return (
    <svg viewBox="0 0 64 26" className={className} aria-hidden="true">
      <path
        fill={GOLD}
        d="M8 16 12.2 8.2h8.6L24.6 4h16.8l4 4.2H52l4.6 7.8H8Zm8.4 2.2a3.1 3.1 0 1 0 0 6.2 3.1 3.1 0 0 0 0-6.2Zm24.2 0a3.1 3.1 0 1 0 0 6.2 3.1 3.1 0 0 0 0-6.2Z"
      />
      <path fill={NAVY} d="M22.4 8.4h16.8l2.2 4.6H20.6Z" opacity="0.18" />
    </svg>
  );
}

function Wordmark({ invert = false, showTagline = false }: { invert?: boolean; showTagline?: boolean }) {
  const ink = invert ? "text-white" : "text-[#0A1F3D]";
  const tag = invert ? "text-white/70" : "text-[#0A1F3D]/70";

  return (
    <span className={`min-w-0 leading-none ${ink}`}>
      <span className="relative block pr-1 font-display text-[1.18rem] font-bold tracking-tight sm:text-[1.38rem]">
        Bengaluru
        <Taxi className="absolute -right-0.5 -top-[0.85rem] h-[0.9rem] w-[2.15rem] sm:-top-[0.95rem] sm:h-[1.05rem] sm:w-[2.45rem]" />
      </span>
      <span className="mt-1 block font-display text-[0.7rem] font-bold tracking-[0.42em] text-[#FFB703] sm:text-[0.78rem] sm:tracking-[0.48em]">
        CABS
      </span>
      {showTagline ? (
        <span className={`mt-2 flex items-center gap-2 ${tag}`}>
          <span className="h-px flex-1 bg-current opacity-40" />
          <span className="whitespace-nowrap text-[8px] font-medium uppercase tracking-[0.18em]">
            Your comfort, our priority
          </span>
          <span className="h-px flex-1 bg-current opacity-40" />
        </span>
      ) : null}
    </span>
  );
}

export function Logo({ variant = "header", className = "" }: LogoProps) {
  const invert = variant === "footer";

  if (variant === "mark") {
    return (
      <span className={`inline-block h-11 w-[4.05rem] ${className}`}>
        <Emblem />
      </span>
    );
  }

  return (
    <span className={`flex items-end gap-2.5 sm:gap-3 ${className}`}>
      <span className={variant === "footer" ? "mb-0.5 h-[3.4rem] w-[5rem]" : "mb-0.5 h-[2.85rem] w-[4.2rem] sm:h-[3.15rem] sm:w-[4.65rem]"}>
        <Emblem invert={invert} />
      </span>
      <Wordmark invert={invert} showTagline={variant === "footer"} />
    </span>
  );
}
