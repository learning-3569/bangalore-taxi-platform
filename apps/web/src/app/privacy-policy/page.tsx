import { LegalPlaceholder } from "@/components/legal/LegalPlaceholder";
import { legalPagesArePlaceholders } from "@/config/site";
import { createPageMetadata } from "@/lib/seo";

export const metadata = createPageMetadata({
  title: "Privacy Policy",
  description:
    "Privacy policy placeholder for Bangalore Taxi. Final text requires business and legal review.",
  path: "/privacy-policy",
  indexable: !legalPagesArePlaceholders,
});

export default function PrivacyPolicyPage() {
  return (
    <LegalPlaceholder
      title="Privacy Policy"
      crumbs={[
        { name: "Home", path: "/", label: "Home", href: "/" },
        { name: "Privacy Policy", path: "/privacy-policy", label: "Privacy Policy" },
      ]}
    />
  );
}
