export default function AdminHomePage() {
  return (
    <div className="min-h-screen bg-slate-950">
      <header className="border-b border-slate-800">
        <div className="mx-auto flex max-w-5xl items-center justify-between px-4 py-4 sm:px-6">
          <p className="text-sm font-semibold tracking-tight text-white">
            Bangalore Taxi Admin
          </p>
          <p className="text-xs uppercase tracking-wide text-slate-500">
            Internal only
          </p>
        </div>
      </header>

      <main className="mx-auto max-w-5xl px-4 py-16 sm:px-6 sm:py-24">
        <p className="text-sm font-medium uppercase tracking-wide text-sky-400">
          Phase 0 foundation
        </p>
        <h1 className="mt-3 max-w-3xl text-4xl font-semibold tracking-tight text-white sm:text-5xl">
          Operations portal for bookings, fleet, and SEO content.
        </h1>
        <p className="mt-6 max-w-2xl text-lg leading-relaxed text-slate-400">
          This application is separate from the public website. Authentication,
          dashboards, assignment workflows, and CMS tools will be implemented in
          later phases. This app must remain non-indexable.
        </p>

        <ul className="mt-10 grid gap-3 text-sm text-slate-300 sm:grid-cols-2">
          <li className="rounded-lg border border-slate-800 bg-slate-900 px-4 py-3">
            Booking request review and driver assignment
          </li>
          <li className="rounded-lg border border-slate-800 bg-slate-900 px-4 py-3">
            Customer, driver, and vehicle records
          </li>
          <li className="rounded-lg border border-slate-800 bg-slate-900 px-4 py-3">
            Configurable pricing rules
          </li>
          <li className="rounded-lg border border-slate-800 bg-slate-900 px-4 py-3">
            SEO page publishing without code changes
          </li>
        </ul>
      </main>
    </div>
  );
}
