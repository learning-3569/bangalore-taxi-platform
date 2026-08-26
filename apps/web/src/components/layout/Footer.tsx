import Link from "next/link";
import { Logo } from "@/components/brand/Logo";
import { Container } from "@/components/ui/Container";
import { businessPlaceholders, legalPages, navItems, popularRoutes, siteConfig } from "@/config/site";

export function Footer() {
  return (
    <footer className="bg-navy text-white">
      <Container className="grid gap-10 py-14 sm:grid-cols-2 lg:grid-cols-4">
        <div>
          <Logo variant="footer" />
          <p className="mt-3 max-w-xs text-sm leading-relaxed text-white/70">
            Airport transfers, city rides, and outstation cars from a Bangalore desk. Fares, phone,
            and fleet models appear here once the business confirms them.
          </p>
        </div>
        <div>
          <p className="text-xs font-semibold uppercase tracking-[0.16em] text-taxi">Taxi services</p>
          <ul className="mt-4 space-y-2 text-sm text-white/75">
            {navItems.slice(1).map((item) => (
              <li key={item.href}>
                <Link href={item.href} className="hover:text-white">
                  {item.label}
                </Link>
              </li>
            ))}
          </ul>
        </div>
        <div>
          <p className="text-xs font-semibold uppercase tracking-[0.16em] text-taxi">Popular routes</p>
          <ul className="mt-4 space-y-2 text-sm text-white/75">
            {popularRoutes.slice(0, 5).map((route) => (
              <li key={`${route.from}-${route.to}`}>
                {"href" in route && route.href ? (
                  <Link href={route.href} className="hover:text-white">
                    {route.from} → {route.to}
                  </Link>
                ) : (
                  <>
                    {route.from} → {route.to}
                  </>
                )}
              </li>
            ))}
          </ul>
          <p className="mt-3 text-xs text-white/45">Live links only where a unique route page exists.</p>
        </div>
        <div>
          <p className="text-xs font-semibold uppercase tracking-[0.16em] text-taxi">Contact</p>
          <ul className="mt-4 space-y-2 text-sm text-white/75">
            <li>{businessPlaceholders.phone}</li>
            <li>{businessPlaceholders.email}</li>
            <li>{businessPlaceholders.address}</li>
          </ul>
          <ul className="mt-6 space-y-2 text-sm text-white/75">
            {legalPages.map((item) => (
              <li key={item.href}>
                <Link href={item.href} className="hover:text-white">
                  {item.label}
                </Link>
              </li>
            ))}
          </ul>
        </div>
      </Container>
      <div className="border-t border-white/10">
        <Container className="flex flex-col gap-2 py-4 text-xs text-white/50 sm:flex-row sm:justify-between">
          <p>
            © {new Date().getFullYear()} {siteConfig.name}. Bengaluru, Karnataka.
          </p>
          <p>Login / My bookings will appear when accounts launch. Payment is not on this site.</p>
        </Container>
      </div>
    </footer>
  );
}
