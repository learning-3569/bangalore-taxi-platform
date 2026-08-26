import { LegalPlaceholder } from "@/components/legal/LegalPlaceholder";
import { createPageMetadata } from "@/lib/seo";

export const metadata = createPageMetadata({
  title: "Terms and Conditions",
  description:
    "Terms and conditions placeholder for Bangalore Taxi. Final text requires business and legal review.",
  path: "/terms-and-conditions",
});

export default function TermsPage() {
  return (
    <LegalPlaceholder
      title="Terms and Conditions"
      crumbs={[
        { name: "Home", path: "/", label: "Home", href: "/" },
        {
          name: "Terms and Conditions",
          path: "/terms-and-conditions",
          label: "Terms and Conditions",
        },
      ]}
    />
  );
}
