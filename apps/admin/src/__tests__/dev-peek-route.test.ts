import { afterEach, describe, expect, it, vi } from "vitest";

describe("admin development OTP BFF", () => {
  afterEach(() => { vi.unstubAllEnvs(); vi.resetModules(); });

  it("returns 404 without contacting the API in production", async () => {
    vi.stubEnv("NODE_ENV", "production");
    const fetchSpy = vi.spyOn(globalThis, "fetch");
    const { GET } = await import("@/app/api/auth/otp/dev-peek/route");
    const response = await GET(new Request("http://admin.test/api/auth/otp/dev-peek?phoneNumber=9876543210"));
    expect(response.status).toBe(404);
    expect(fetchSpy).not.toHaveBeenCalled();
    expect(await response.text()).not.toMatch(/otp|123456/i);
  });
});
