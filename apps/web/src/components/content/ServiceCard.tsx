import Link from "next/link";
import { Card } from "@/components/ui/Card";

export function ServiceCard({
  title,
  description,
  href,
}: {
  title: string;
  description: string;
  href: string;
}) {
  return (
    <Card>
      <h3 className="font-serif text-lg font-semibold text-ink">{title}</h3>
      <p className="mt-2 text-sm leading-relaxed text-ink-muted">{description}</p>
      <Link href={href} className="mt-4 inline-block text-sm font-semibold text-brand hover:underline">
        View section
      </Link>
    </Card>
  );
}
