"use client";

import { FormEvent, useEffect, useId, useRef, useState } from "react";
import { useSearchParams } from "next/navigation";
import { useAuth } from "@/components/auth/AuthProvider";
import { Button } from "@/components/ui/Button";
import { TextField } from "@/components/ui/Fields";
import { continueHref, isValidIndianMobile, parseBookingIntent } from "@/lib/booking-intent";
import { cooldownMessage, OtpCooldownError } from "@/lib/otp-cooldown";

type Step = "phone" | "otp" | "done";

export function OtpAuthForm() {
  const auth = useAuth();
  const search = useSearchParams();
  const intent = parseBookingIntent(search);
  const phoneRef = useRef<HTMLInputElement>(null);
  const errorId = useId();

  const [step, setStep] = useState<Step>("phone");
  const [phone, setPhone] = useState("");
  const [otp, setOtp] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const [seconds, setSeconds] = useState(0);
  const [cooldownBlocked, setCooldownBlocked] = useState(false);
  const [devOtp, setDevOtp] = useState<string | null>(null);

  useEffect(() => {
    if (auth.user) setStep("done");
  }, [auth.user]);

  useEffect(() => {
    if (seconds <= 0) return;
    const id = window.setTimeout(() => setSeconds((value) => value - 1), 1000);
    return () => window.clearTimeout(id);
  }, [seconds]);

  useEffect(() => {
    if (!cooldownBlocked) return;
    if (seconds > 0) {
      setError(cooldownMessage(seconds));
      return;
    }
    setError("");
    setCooldownBlocked(false);
  }, [seconds, cooldownBlocked]);

  useEffect(() => {
    if (step === "phone") phoneRef.current?.focus();
  }, [step]);

  async function sendCode() {
    setError("");
    if (!isValidIndianMobile(phone)) {
      setError("Enter a 10-digit Indian mobile number.");
      return;
    }
    setLoading(true);
    try {
      const result = await auth.requestOtp(phone);
      setSeconds(result.resendAvailableInSeconds ?? 60);
      setCooldownBlocked(false);
      setError("");
      setStep("otp");
      if (process.env.NODE_ENV !== "production") {
        setDevOtp(await auth.peekDevOtp(phone));
      }
    } catch (err) {
      if (err instanceof OtpCooldownError) {
        setCooldownBlocked(true);
        setSeconds(err.retryAfterSeconds);
        setError(cooldownMessage(err.retryAfterSeconds));
        return;
      }
      setError(err instanceof Error ? err.message : "Could not send the code.");
    } finally {
      setLoading(false);
    }
  }

  async function onPhoneSubmit(event: FormEvent) {
    event.preventDefault();
    await sendCode();
  }

  async function onOtpSubmit(event: FormEvent) {
    event.preventDefault();
    setError("");
    if (!/^\d{4,8}$/.test(otp.trim())) {
      setError("Enter the 6-digit code.");
      return;
    }
    setLoading(true);
    try {
      await auth.verifyOtp(phone, otp.trim());
      setStep("done");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Unable to verify the code.");
    } finally {
      setLoading(false);
    }
  }

  if (step === "done" && auth.user) {
    return (
      <div className="border border-line bg-paper p-5">
        <p className="text-sm font-semibold uppercase tracking-[0.16em] text-taxi">Verified</p>
        <h1 className="mt-2 font-display text-2xl font-semibold text-navy">You are signed in</h1>
        <p className="mt-2 text-sm text-ink-muted">
          Phone {auth.user.maskedPhone}. Continue to complete your booking request. Our team reviews each request
          before a trip is confirmed.
        </p>
        {error ? (
          <p role="alert" className="mt-2 text-sm text-red-700">
            {error}
          </p>
        ) : null}
        <div className="mt-6 flex flex-wrap gap-3">
          <Button href={continueHref(intent)} variant="taxi" className="uppercase">
            Continue to Booking
          </Button>
          <Button
            type="button"
            variant="outline"
            onClick={() => {
              void auth
                .logout()
                .then(() => {
                  setStep("phone");
                  setOtp("");
                  setPhone("");
                  setError("");
                })
                .catch((err: unknown) => {
                  setError(err instanceof Error ? err.message : "Unable to log out. Please try again.");
                });
            }}
          >
            Log out
          </Button>
        </div>
      </div>
    );
  }

  if (step === "otp") {
    return (
      <form onSubmit={onOtpSubmit} className="border border-line bg-paper p-5" noValidate>
        <p className="text-sm font-semibold uppercase tracking-[0.16em] text-taxi">Step 2</p>
        <h1 className="mt-2 font-display text-2xl font-semibold text-navy">Enter the OTP</h1>
        <p className="mt-2 text-sm text-ink-muted">
          We sent a code to +91 {phone.replace(/\D/g, "").slice(-10)}.
        </p>
        <div className="mt-4">
          <TextField
            id="otp"
            name="otp"
            label="One-time code"
            inputMode="numeric"
            autoComplete="one-time-code"
            pattern="[0-9]*"
            maxLength={8}
            value={otp}
            autoFocus
            onChange={(event) => setOtp(event.target.value)}
            error={error}
            required
          />
        </div>
        {devOtp ? (
          <p className="mt-2 text-xs text-ink-muted">Development code: {devOtp}</p>
        ) : null}
        <div className="mt-4 flex flex-wrap items-center gap-3">
          <Button type="submit" variant="taxi" className="uppercase" disabled={loading} aria-busy={loading}>
            {loading ? "Verifying…" : "Verify OTP"}
          </Button>
          <button
            type="button"
            className="text-sm font-medium text-navy underline disabled:text-ink-muted"
            disabled={seconds > 0 || loading}
            aria-disabled={seconds > 0 || loading}
            aria-live="polite"
            onClick={() => void sendCode()}
          >
            {seconds > 0 ? `Resend in ${seconds}s` : "Resend code"}
          </button>
          <button
            type="button"
            className="text-sm font-medium text-navy underline"
            onClick={() => {
              setStep("phone");
              setOtp("");
              setError("");
            }}
          >
            Change number
          </button>
        </div>
      </form>
    );
  }

  return (
    <form onSubmit={onPhoneSubmit} className="border border-line bg-paper p-5" noValidate>
      <p className="text-sm font-semibold uppercase tracking-[0.16em] text-taxi">Step 1</p>
      <h1 className="mt-2 font-display text-2xl font-semibold text-navy">Verify your mobile number</h1>
      <p className="mt-2 text-sm text-ink-muted">
        You&apos;ll verify your mobile number before completing your booking request. Guest checkout is not
        available.
      </p>
      <div className="mt-4">
        <label htmlFor="phone" className="text-sm font-medium text-ink">
          Mobile number
        </label>
        <div className="mt-1 flex">
          <span className="inline-flex items-center border border-r-0 border-line bg-paper-soft px-3 text-sm text-ink-muted">
            +91
          </span>
          <input
            ref={phoneRef}
            id="phone"
            name="phone"
            inputMode="numeric"
            autoComplete="tel-national"
            className="w-full rounded-none border border-line bg-paper-raised px-3 py-2.5 text-base text-ink focus:border-brand focus:ring-1 focus:ring-brand"
            placeholder="9876543210"
            value={phone}
            aria-describedby={error ? errorId : undefined}
            aria-invalid={Boolean(error) && !cooldownBlocked ? true : undefined}
            onChange={(event) => setPhone(event.target.value)}
          />
        </div>
        {error ? (
          <p
            id={errorId}
            role={cooldownBlocked ? "status" : "alert"}
            aria-live="polite"
            aria-atomic="true"
            className="mt-1 text-sm text-red-700"
          >
            {error}
          </p>
        ) : (
          <p className="mt-1 text-xs text-ink-muted">10-digit Indian mobile number.</p>
        )}
      </div>
      <div className="mt-4">
        <Button
          type="submit"
          variant="taxi"
          className="uppercase"
          disabled={loading || seconds > 0}
          aria-busy={loading}
          aria-live="polite"
        >
          {loading ? "Sending…" : seconds > 0 ? `Send OTP in ${seconds}s` : "Send OTP"}
        </Button>
      </div>
    </form>
  );
}
