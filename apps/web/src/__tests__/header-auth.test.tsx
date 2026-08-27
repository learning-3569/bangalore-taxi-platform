import { render, screen, waitFor, within } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { AuthProvider } from "@/components/auth/AuthProvider";
import { Header } from "@/components/layout/Header";

const sessionUser = {
  userId: "u1",
  customerId: "c1",
  phoneNumber: "+919876543210",
  maskedPhone: "******3210",
  roles: ["customer"],
};

function jsonResponse(body: unknown, status = 200) {
  return new Response(JSON.stringify(body), {
    status,
    headers: { "Content-Type": "application/json" },
  });
}

describe("header account logout", () => {
  beforeEach(() => {
    vi.stubGlobal(
      "fetch",
      vi.fn(async (input: RequestInfo | URL) => {
        const url = String(input);
        if (url.includes("/api/auth/refresh")) {
          return jsonResponse({ accessToken: "access-1", user: sessionUser });
        }
        if (url.includes("/api/auth/logout")) {
          return new Response(null, { status: 204 });
        }
        return new Response(null, { status: 401 });
      }),
    );
  });

  it("shows Account and Logout when authenticated, then signed-out header after logout", async () => {
    const user = userEvent.setup();
    render(
      <AuthProvider>
        <Header />
      </AuthProvider>,
    );

    const account = await screen.findByRole("button", { name: /account/i });
    expect(screen.queryByRole("link", { name: /my bookings/i })).not.toBeInTheDocument();
    expect(screen.getByRole("link", { name: /^book a cab$/i })).toBeInTheDocument();

    await user.click(account);
    const myBookings = await screen.findByRole("menuitem", { name: /my bookings/i });
    expect(myBookings).toHaveAttribute("href", "/account/bookings");
    expect(screen.getByRole("menuitem", { name: /logout/i })).toBeInTheDocument();
    const logout = await screen.findByRole("menuitem", { name: /logout/i });
    await user.click(logout);

    await waitFor(() => {
      expect(screen.queryByRole("button", { name: /account/i })).not.toBeInTheDocument();
    });
    const logoutCall = vi.mocked(fetch).mock.calls.find((call) => String(call[0]).includes("/api/auth/logout"));
    expect(logoutCall?.[1]).toMatchObject({ method: "POST", credentials: "same-origin" });
    expect((logoutCall?.[1]?.headers as Record<string, string>)["X-CSRF-Token"]).toBeDefined();
    expect(screen.getByRole("link", { name: /^book a cab$/i })).toHaveAttribute("href", expect.stringContaining("/login"));
  });

  it("keeps the session when logout fails", async () => {
    vi.mocked(fetch).mockImplementation(async (input: RequestInfo | URL, init?: RequestInit) => {
      const url = String(input);
      const method = init?.method ?? "GET";
      if (url.includes("/api/auth/refresh") && method === "POST") {
        return jsonResponse({ accessToken: "access-1", user: sessionUser });
      }
      if (url.includes("/api/auth/logout")) {
        return jsonResponse({ title: "Unable to log out", detail: "Please try again. You are still signed in." }, 502);
      }
      return new Response(null, { status: 401 });
    });
    const user = userEvent.setup();
    render(
      <AuthProvider>
        <Header />
      </AuthProvider>,
    );
    await user.click(await screen.findByRole("button", { name: /account/i }));
    await user.click(await screen.findByRole("menuitem", { name: /logout/i }));
    expect(await screen.findByRole("alert")).toHaveTextContent(/still signed in|unable to log out|try again/i);
    expect(screen.getByRole("button", { name: /account/i })).toBeInTheDocument();
  });
});

describe("unauthenticated header", () => {
  it("does not show authenticated account actions", () => {
    render(
      <AuthProvider>
        <Header />
      </AuthProvider>,
    );
    expect(screen.queryByRole("button", { name: /account/i })).not.toBeInTheDocument();
    expect(screen.queryByRole("menuitem", { name: /logout/i })).not.toBeInTheDocument();
    expect(screen.queryByRole("link", { name: /my bookings/i })).not.toBeInTheDocument();
    expect(screen.getByRole("navigation", { name: "Primary" })).toBeInTheDocument();
    expect(screen.getByRole("link", { name: /^book a cab$/i })).toBeInTheDocument();
  });
});

describe("header mobile logout", () => {
  beforeEach(() => {
    vi.stubGlobal(
      "fetch",
      vi.fn(async (input: RequestInfo | URL, init?: RequestInit) => {
        const url = String(input);
        if (String(url).includes("/api/auth/refresh") && (init?.method ?? "GET") === "POST") {
          return jsonResponse({ accessToken: "access-1", user: sessionUser });
        }
        return new Response(null, { status: 401 });
      }),
    );
  });

  it("exposes My Bookings and Logout inside the mobile account menu", async () => {
    const user = userEvent.setup();
    render(
      <AuthProvider>
        <Header />
      </AuthProvider>,
    );
    await screen.findByRole("button", { name: /account/i });
    await user.click(screen.getByRole("button", { name: /open menu/i }));
    const mobile = screen.getByRole("navigation", { name: "Mobile" });
    expect(within(mobile).getByRole("button", { name: /account/i })).toBeInTheDocument();
    await user.click(within(mobile).getByRole("button", { name: /account/i }));
    expect(within(mobile).getByRole("menuitem", { name: /my bookings/i })).toHaveAttribute("href", "/account/bookings");
    expect(within(mobile).getByRole("menuitem", { name: /logout/i })).toBeInTheDocument();
  });

  it("closes the mobile navigation after choosing My Bookings", async () => {
    const user = userEvent.setup();
    render(<AuthProvider><Header /></AuthProvider>);
    await screen.findByRole("button", { name: /account/i });
    await user.click(screen.getByRole("button", { name: /open menu/i }));
    const mobile = screen.getByRole("navigation", { name: "Mobile" });
    await user.click(within(mobile).getByRole("button", { name: /account/i }));
    await user.click(within(mobile).getByRole("menuitem", { name: /my bookings/i }));
    expect(screen.queryByRole("navigation", { name: "Mobile" })).not.toBeInTheDocument();
    expect(screen.getByRole("button", { name: /open menu/i })).toHaveFocus();
  });
});
