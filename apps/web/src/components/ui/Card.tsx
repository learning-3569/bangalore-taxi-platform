import type { ReactNode } from "react";

export function Card({ children, className = "" }: { children: ReactNode; className?: string }) {
  return (
    <div className={`rounded-xl border border-line bg-paper-raised p-5 shadow-[0_1px_2px_rgba(28,25,23,0.04)] ${className}`}>
      {children}
    </div>
  );
}
