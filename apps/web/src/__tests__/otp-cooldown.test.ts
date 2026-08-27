import { describe, expect, it } from "vitest";
import { cooldownMessage, OtpCooldownError, parseRetryAfterSeconds } from "@/lib/otp-cooldown";

describe("OTP request cooldown helpers", () => {
  it("prefers a numeric Retry-After header", () => {
    const response = new Response(null, { headers: { "Retry-After": "42" } });
    expect(parseRetryAfterSeconds(response, { retryAfterSeconds: 7 })).toBe(42);
  });

  it("uses retryAfterSeconds when the header is missing or not a delay", () => {
    const missing = new Response(null);
    expect(parseRetryAfterSeconds(missing, { retryAfterSeconds: 9 })).toBe(9);
    const httpDate = new Response(null, { headers: { "Retry-After": "Wed, 21 Oct 2015 07:28:00 GMT" } });
    expect(parseRetryAfterSeconds(httpDate, { retryAfterSeconds: 5 })).toBe(5);
  });

  it("ignores non-positive values", () => {
    const response = new Response(null, { headers: { "Retry-After": "0" } });
    expect(parseRetryAfterSeconds(response, { retryAfterSeconds: -1 })).toBeNull();
  });

  it("builds a customer-facing wait message without internal details", () => {
    const error = new OtpCooldownError(12);
    expect(error.message).toBe("You can request another code in 12 seconds");
    expect(cooldownMessage(3)).toBe("You can request another code in 3 seconds");
    expect(error.message.toLowerCase()).not.toMatch(/rate.?limit|429|challenge/);
  });
});
