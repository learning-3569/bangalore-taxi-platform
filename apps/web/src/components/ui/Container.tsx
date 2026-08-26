import type { ReactNode } from "react";

type ContainerProps = {
  children: ReactNode;
  className?: string;
  as?: "div" | "section" | "header" | "footer" | "nav";
  "aria-label"?: string;
};

export function Container({ children, className = "", as: Tag = "div", ...rest }: ContainerProps) {
  return (
    <Tag className={`mx-auto w-full max-w-6xl px-4 sm:px-6 lg:px-8 ${className}`} {...rest}>
      {children}
    </Tag>
  );
}
