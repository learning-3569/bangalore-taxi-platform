import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { OtpAuthForm } from "@/components/auth/OtpAuthForm";
import { continueHref, isValidIndianMobile, loginHref } from "@/lib/booking-intent";
import { OtpCooldownError } from "@/lib/otp-cooldown";

const requestOtp = vi.fn();
const verifyOtp = vi.fn();
const logout = vi.fn();
const peekDevOtp = vi.fn();
const userState: { user: { maskedPhone: string; phoneNumber: string; userId: string; roles: string[] } | null } = {
  user: null,
};

vi.mock("@/components/auth/AuthProvider", () => ({
  useAuth: () => ({
    user: userState.user,
    accessToken: null,
    ready: true,
    requestOtp,
    verifyOtp,
    logout,
    peekDevOtp,
  }),
}));

describe("booking intent helpers", () => {
  it("preserves route context in the login URL", () => {
    const href = loginHref({
      next: "/whitefield-to-bangalore-airport-taxi",
      pickup: "Whitefield",
      drop: "Bangalore Airport",
      tripType: "airport",
    });
    expect(href).toContain("/login?");
    expect(href).toContain("next=%2Fwhitefield-to-bangalore-airport-taxi");
    expect(href).toContain("pickup=Whitefield");
    expect(continueHref({ next: "/whitefield-to-bangalore-airport-taxi" })).toBe(
      "/whitefield-to-bangalore-airport-taxi#book",
    );
  });

  it("validates Indian mobiles", () => {
    expect(isValidIndianMobile("9876543210")).toBe(true);
    expect(isValidIndianMobile("12345")).toBe(false);
  });
});

describe("OTP form", () => {
  beforeEach(() => {
    userState.user = null;
    requestOtp.mockReset().mockResolvedValue({ resendAvailableInSeconds: 60 });
    verifyOtp.mockReset().mockResolvedValue(undefined);
    peekDevOtp.mockReset().mockResolvedValue("123456");
    logout.mockReset();
  });

  it("renders the phone step and validates input", async () => {
    const user = userEvent.setup();
    render(<OtpAuthForm />);
    expect(screen.getByRole("heading", { name: /verify your mobile number/i })).toBeInTheDocument();
    expect(screen.getByLabelText(/mobile number/i)).toBeInTheDocument();
    await user.click(screen.getByRole("button", { name: /send otp/i }));
    expect(screen.getByRole("alert")).toHaveTextContent(/10-digit/i);
    expect(requestOtp).not.toHaveBeenCalled();
  });

  it("moves to OTP entry after send and verifies", async () => {
    const user = userEvent.setup();
    render(<OtpAuthForm />);
    await user.type(screen.getByLabelText(/mobile number/i), "9876543210");
    await user.click(screen.getByRole("button", { name: /send otp/i }));
    expect(await screen.findByLabelText(/one-time code/i)).toBeInTheDocument();
    expect(requestOtp).toHaveBeenCalled();
    await user.type(screen.getByLabelText(/one-time code/i), "000000");
    await user.click(screen.getByRole("button", { name: /verify otp/i }));
    expect(verifyOtp).toHaveBeenCalledWith("9876543210", "000000");
  });

  it("disables resend until the cooldown elapses", async () => {
    const user = userEvent.setup();
    render(<OtpAuthForm />);
    await user.type(screen.getByLabelText(/mobile number/i), "9876543210");
    await user.click(screen.getByRole("button", { name: /send otp/i }));
    expect(await screen.findByRole("button", { name: /resend in 60s/i })).toBeDisabled();
    expect(screen.getByRole("button", { name: /change number/i })).toBeEnabled();
  });

  it("does not treat a successful send as a cooldown error on the OTP field", async () => {
    const user = userEvent.setup();
    render(<OtpAuthForm />);
    await user.type(screen.getByLabelText(/mobile number/i), "9876543210");
    await user.click(screen.getByRole("button", { name: /send otp/i }));
    expect(await screen.findByLabelText(/one-time code/i)).toBeInTheDocument();
    expect(screen.queryByText(/you can request another code/i)).not.toBeInTheDocument();
    expect(screen.getByRole("button", { name: /resend in 60s/i })).toBeDisabled();
  });

  it("shows a cooldown wait on the phone step and re-enables Send when it expires", async () => {
    requestOtp.mockRejectedValueOnce(new OtpCooldownError(1));
    const user = userEvent.setup();
    render(<OtpAuthForm />);
    await user.type(screen.getByLabelText(/mobile number/i), "9876543210");
    await user.click(screen.getByRole("button", { name: /send otp/i }));

    expect(screen.getByRole("heading", { name: /verify your mobile number/i })).toBeInTheDocument();
    expect(await screen.findByRole("status")).toHaveTextContent("You can request another code in 1 seconds");
    expect(screen.getByRole("button", { name: /send otp in 1s/i })).toBeDisabled();
    expect(screen.queryByText(/rate limit|too many requests|429/i)).not.toBeInTheDocument();

    await waitFor(
      () => {
        expect(screen.queryByRole("status")).not.toBeInTheDocument();
        expect(screen.getByRole("button", { name: /^send otp$/i })).toBeEnabled();
      },
      { timeout: 2500 },
    );
  });

  it("shows expired OTP errors", async () => {
    verifyOtp.mockRejectedValueOnce(new Error("Unable to verify the code."));
    const user = userEvent.setup();
    render(<OtpAuthForm />);
    await user.type(screen.getByLabelText(/mobile number/i), "9876543210");
    await user.click(screen.getByRole("button", { name: /send otp/i }));
    await screen.findByLabelText(/one-time code/i);
    await user.type(screen.getByLabelText(/one-time code/i), "123456");
    await user.click(screen.getByRole("button", { name: /verify otp/i }));
    expect(await screen.findByRole("alert")).toHaveTextContent(/unable to verify/i);
  });

  it("shows wrong OTP errors", async () => {
    verifyOtp.mockRejectedValueOnce(new Error("Unable to verify the code."));
    const user = userEvent.setup();
    render(<OtpAuthForm />);
    await user.type(screen.getByLabelText(/mobile number/i), "9876543210");
    await user.click(screen.getByRole("button", { name: /send otp/i }));
    await screen.findByLabelText(/one-time code/i);
    await user.type(screen.getByLabelText(/one-time code/i), "111111");
    await user.click(screen.getByRole("button", { name: /verify otp/i }));
    expect(await screen.findByRole("alert")).toHaveTextContent(/unable to verify/i);
  });

  it("shows a continue-to-booking CTA after success", () => {
    userState.user = {
      userId: "u1",
      phoneNumber: "+919876543210",
      maskedPhone: "******3210",
      roles: ["customer"],
    };
    render(<OtpAuthForm />);
    expect(screen.getByRole("heading", { name: /you are signed in/i })).toBeInTheDocument();
    expect(screen.getByRole("link", { name: /continue to booking/i })).toBeInTheDocument();
  });

  it("returns to the phone step after logout so the same number can request a new OTP", async () => {
    userState.user = {
      userId: "u1",
      phoneNumber: "+919876543210",
      maskedPhone: "******3210",
      roles: ["customer"],
    };
    logout.mockImplementation(async () => {
      userState.user = null;
    });
    const user = userEvent.setup();
    const { rerender } = render(<OtpAuthForm />);
    await user.click(screen.getByRole("button", { name: /log out/i }));
    await waitFor(() => expect(logout).toHaveBeenCalled());
    rerender(<OtpAuthForm />);
    await user.type(screen.getByLabelText(/mobile number/i), "9876543210");
    await user.click(screen.getByRole("button", { name: /send otp/i }));
    expect(requestOtp).toHaveBeenCalledWith("9876543210");
  });

  it("keeps the phone step and shows a wait after logout when resend cooldown is still active", async () => {
    userState.user = {
      userId: "u1",
      phoneNumber: "+919876543210",
      maskedPhone: "******3210",
      roles: ["customer"],
    };
    logout.mockImplementation(async () => {
      userState.user = null;
    });
    requestOtp.mockRejectedValueOnce(new OtpCooldownError(1));
    const user = userEvent.setup();
    const { rerender } = render(<OtpAuthForm />);
    await user.click(screen.getByRole("button", { name: /log out/i }));
    await waitFor(() => expect(logout).toHaveBeenCalled());
    rerender(<OtpAuthForm />);
    await user.type(screen.getByLabelText(/mobile number/i), "9876543210");
    await user.click(screen.getByRole("button", { name: /send otp/i }));
    expect(await screen.findByRole("status")).toHaveTextContent(/you can request another code in 1 seconds/i);
    expect(screen.getByRole("heading", { name: /verify your mobile number/i })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /send otp in 1s/i })).toBeDisabled();
  });
});
