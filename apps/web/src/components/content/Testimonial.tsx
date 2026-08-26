import { Card } from "@/components/ui/Card";

export function Testimonial({ quote, attribution }: { quote: string; attribution: string }) {
  return (
    <figure>
      <Card>
        <blockquote className="text-sm leading-relaxed text-ink-muted">{quote}</blockquote>
        <figcaption className="mt-3 text-xs font-medium uppercase tracking-wide text-accent">
          {attribution}
        </figcaption>
      </Card>
    </figure>
  );
}
