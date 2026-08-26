import Link from "next/link";
import type { ButtonHTMLAttributes, MouseEventHandler, ReactNode } from "react";

const variants = {
  primary:
    "inline-flex items-center justify-center rounded-sm bg-navy px-5 py-2.5 text-sm font-semibold tracking-wide text-white transition hover:-translate-y-px hover:bg-navy-mid",
  secondary:
    "inline-flex items-center justify-center rounded-sm border border-white/40 bg-transparent px-5 py-2.5 text-sm font-semibold tracking-wide text-white transition hover:border-taxi hover:text-taxi",
  taxi:
    "inline-flex items-center justify-center rounded-sm bg-taxi px-5 py-2.5 text-sm font-bold tracking-wide text-navy transition hover:-translate-y-px hover:bg-taxi-deep hover:shadow-[0_8px_18px_rgba(247,169,0,0.28)]",
  outline:
    "inline-flex items-center justify-center rounded-sm border border-line bg-paper px-5 py-2.5 text-sm font-semibold text-navy transition hover:border-navy",
} as const;

type ButtonProps = ButtonHTMLAttributes<HTMLButtonElement> & {
  variant?: keyof typeof variants;
  href?: string;
  children: ReactNode;
};

export function Button({ variant = "taxi", href, className = "", children, ...props }: ButtonProps) {
  const classes = `${variants[variant]} ${className}`;
  if (href) {
    const { disabled, onClick } = props;
    return (
      <Link
        href={href}
        className={`${classes} ${disabled ? "pointer-events-none opacity-60" : ""}`}
        onClick={onClick as MouseEventHandler<HTMLAnchorElement> | undefined}
      >
        {children}
      </Link>
    );
  }
  return (
    <button className={classes} {...props}>
      {children}
    </button>
  );
}
