import { Suspense } from "react";
import { OtpAuthForm } from "@/components/auth/OtpAuthForm";
import { Container } from "@/components/ui/Container";
import { createPageMetadata } from "@/lib/seo";

export const metadata = createPageMetadata({
  title: "Verify mobile number",
  description: "Verify your phone with OTP to continue a Bangalore taxi booking request.",
  path: "/login",
  indexable: false,
});

export default function LoginPage() {
  return (
    <main id="main" className="bg-paper-soft py-10 sm:py-16">
      <Container className="max-w-lg">
        <Suspense fallback={<p className="text-sm text-ink-muted">Loading sign-in…</p>}>
          <OtpAuthForm />
        </Suspense>
      </Container>
    </main>
  );
}
