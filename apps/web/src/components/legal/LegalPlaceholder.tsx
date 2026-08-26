import { Breadcrumbs } from "@/components/ui/Breadcrumbs";
import { Container } from "@/components/ui/Container";
import { breadcrumbJsonLd, JsonLd } from "@/components/seo/JsonLd";

export function LegalPlaceholder({
  title,
  crumbs,
}: {
  title: string;
  crumbs: { name: string; path: string; label: string; href?: string }[];
}) {
  return (
    <main id="main" className="py-10 sm:py-14">
      <JsonLd
        data={breadcrumbJsonLd(
          crumbs.map((item) => ({ name: item.name, path: item.path })),
        )}
      />
      <Container className="max-w-3xl">
        <Breadcrumbs
          items={crumbs.map((item) => ({
            href: item.href,
            label: item.label,
          }))}
        />
        <h1 className="mt-6 font-serif text-3xl font-semibold tracking-tight text-ink">{title}</h1>
        <p className="mt-4 rounded-lg border border-line bg-paper-raised px-4 py-3 text-sm font-medium text-accent">
          Placeholder — pending business and legal review. This is not legal advice and is not a
          published policy.
        </p>
        <div className="mt-6 space-y-4 text-base leading-relaxed text-ink-muted">
          <p>
            This page exists so the public site has a stable URL for {title.toLowerCase()}. Final
            wording must be written or approved by the business and, where needed, a qualified
            adviser.
          </p>
          <p>Until that review is complete, do not treat anything on this page as an operational rule.</p>
        </div>
      </Container>
    </main>
  );
}
