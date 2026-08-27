import { AuthAwareBookButton } from "@/components/auth/AuthAwareBookButton";
import { Button } from "@/components/ui/Button";
import type { BookingIntent } from "@/lib/booking-intent";

export function RouteStickyCta({
  authedHref,
  bookLabel,
  intent,
}: {
  authedHref: string;
  bookLabel: string;
  intent: BookingIntent;
}) {
  return (
    <div className="fixed inset-x-0 bottom-0 z-40 border-t border-line bg-paper/95 p-3 backdrop-blur-sm md:hidden">
      <div className="flex gap-2">
        <AuthAwareBookButton authedHref={authedHref} intent={intent} variant="taxi" className="flex-1 uppercase">
          {bookLabel}
        </AuthAwareBookButton>
        <Button href="/#contact" variant="outline" className="flex-1 uppercase">
          Call now
        </Button>
      </div>
    </div>
  );
}
