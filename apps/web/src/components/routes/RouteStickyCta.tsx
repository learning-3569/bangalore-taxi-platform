import { Button } from "@/components/ui/Button";

export function RouteStickyCta({
  bookHref,
  bookLabel,
}: {
  bookHref: string;
  bookLabel: string;
}) {
  return (
    <div className="fixed inset-x-0 bottom-0 z-40 border-t border-line bg-paper/95 p-3 backdrop-blur-sm md:hidden">
      <div className="flex gap-2">
        <Button href={bookHref} variant="taxi" className="flex-1 uppercase">
          {bookLabel}
        </Button>
        <Button href="/#contact" variant="outline" className="flex-1 uppercase">
          Call now
        </Button>
      </div>
    </div>
  );
}
