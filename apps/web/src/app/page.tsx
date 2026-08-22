export default function HomePage() {
  return (
    <div className="min-h-screen bg-stone-50">
      <header className="border-b border-stone-200 bg-white">
        <div className="mx-auto flex max-w-5xl items-center justify-between px-4 py-4 sm:px-6">
          <p className="text-lg font-semibold tracking-tight text-stone-900">
            Bangalore Taxi
          </p>
          <p className="text-sm text-stone-500">Bengaluru, India</p>
        </div>
      </header>

      <main className="mx-auto max-w-5xl px-4 py-16 sm:px-6 sm:py-24">
        <p className="text-sm font-medium uppercase tracking-wide text-amber-800">
          Phase 0 foundation
        </p>
        <h1 className="mt-3 max-w-3xl text-4xl font-semibold tracking-tight text-stone-900 sm:text-5xl">
          Reliable taxi booking for Bangalore airport, outstation, and city trips.
        </h1>
        <p className="mt-6 max-w-2xl text-lg leading-relaxed text-stone-600">
          This public website is the SEO-first customer surface for a Bangalore
          taxi fleet of about 20 cars. Online booking, customer accounts, and
          landing pages will be added in later phases. Online payment is not
          part of the first release.
        </p>

        <section className="mt-12 grid gap-4 sm:grid-cols-3">
          <article className="rounded-xl border border-stone-200 bg-white p-5">
            <h2 className="font-semibold text-stone-900">Airport transfers</h2>
            <p className="mt-2 text-sm leading-relaxed text-stone-600">
              Pickup and drop pages for Kempegowda International Airport will
              be published as dedicated, crawlable routes.
            </p>
          </article>
          <article className="rounded-xl border border-stone-200 bg-white p-5">
            <h2 className="font-semibold text-stone-900">Outstation taxis</h2>
            <p className="mt-2 text-sm leading-relaxed text-stone-600">
              One-way and round-trip routes such as Bangalore to Mysore and
              Coorg will use stable, search-friendly URLs.
            </p>
          </article>
          <article className="rounded-xl border border-stone-200 bg-white p-5">
            <h2 className="font-semibold text-stone-900">Advance booking</h2>
            <p className="mt-2 text-sm leading-relaxed text-stone-600">
              Customers will request trips in advance. Administrators will
              accept requests and assign drivers from a separate admin portal.
            </p>
          </article>
        </section>
      </main>

      <footer className="border-t border-stone-200 bg-white">
        <div className="mx-auto max-w-5xl px-4 py-6 text-sm text-stone-500 sm:px-6">
          Bangalore Taxi Platform. Current phase: Architecture &amp; project
          setup.
        </div>
      </footer>
    </div>
  );
}
