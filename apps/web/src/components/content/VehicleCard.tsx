import { Button } from "@/components/ui/Button";
import { Card } from "@/components/ui/Card";

function VehicleMark({ name }: { name: string }) {
  return (
    <div
      className="flex h-28 items-end justify-center rounded-lg bg-paper"
      role="img"
      aria-label={`${name} illustration placeholder`}
    >
      <svg viewBox="0 0 160 56" className="h-16 w-40 text-brand" aria-hidden="true">
        <rect x="18" y="22" width="124" height="22" rx="6" fill="currentColor" opacity="0.18" />
        <path d="M28 36h104l-10-16H44L28 36Z" fill="currentColor" opacity="0.85" />
        <circle cx="50" cy="40" r="8" fill="#1c1917" />
        <circle cx="112" cy="40" r="8" fill="#1c1917" />
      </svg>
    </div>
  );
}

export function VehicleCard({
  name,
  seats,
  luggage,
  description,
}: {
  name: string;
  seats: string;
  luggage: string;
  description: string;
}) {
  return (
    <Card>
      <VehicleMark name={name} />
      <h3 className="mt-4 font-serif text-xl font-semibold text-ink">{name}</h3>
      <p className="mt-1 text-sm text-ink-muted">
        {seats} · {luggage}
      </p>
      <p className="mt-2 text-sm leading-relaxed text-ink-muted">{description}</p>
      <Button href="/#book" variant="secondary" className="mt-4">
        Request this type
      </Button>
    </Card>
  );
}
