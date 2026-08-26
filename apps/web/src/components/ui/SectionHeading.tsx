type SectionHeadingProps = {
  eyebrow?: string;
  title: string;
  description?: string;
  align?: "left" | "center";
  invert?: boolean;
};

export function SectionHeading({
  eyebrow,
  title,
  description,
  align = "left",
  invert = false,
}: SectionHeadingProps) {
  return (
    <div className={align === "center" ? "mx-auto max-w-2xl text-center" : "max-w-2xl"}>
      {eyebrow ? (
        <p className={`text-xs font-semibold uppercase tracking-[0.18em] ${invert ? "text-taxi" : "text-taxi-deep"}`}>
          {eyebrow}
        </p>
      ) : null}
      <h2
        className={`mt-2 font-display text-2xl font-semibold tracking-tight sm:text-3xl lg:text-[2.1rem] ${
          invert ? "text-white" : "text-navy"
        }`}
      >
        {title}
      </h2>
      {description ? (
        <p className={`mt-3 text-base leading-relaxed ${invert ? "text-white/70" : "text-ink-muted"}`}>{description}</p>
      ) : null}
    </div>
  );
}
