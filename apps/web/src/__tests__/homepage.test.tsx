import { describe, expect, it } from "vitest";
import { render, screen, within } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import HomePage from "@/app/page";
import { Header } from "@/components/layout/Header";

describe("homepage", () => {
  it("renders the primary heading, navigation, and booking widget", () => {
    render(
      <>
        <Header />
        <HomePage />
      </>,
    );
    expect(
      screen.getByRole("heading", {
        level: 1,
        name: /airport taxis, without the scramble/i,
      }),
    ).toBeInTheDocument();
    expect(screen.getByRole("navigation", { name: "Primary" })).toBeInTheDocument();
    expect(screen.getByLabelText(/pickup location/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/drop location/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/travel date/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/pickup time/i)).toBeInTheDocument();
    expect(screen.getByRole("tablist", { name: /trip type/i })).toBeInTheDocument();
    expect(screen.getByLabelText(/vehicle type/i)).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /book now/i })).toBeInTheDocument();
    expect(screen.getByText(/phone verification will be required/i)).toBeInTheDocument();
  });
});

describe("header mobile navigation", () => {
  it("opens, exposes links, and closes on Escape", async () => {
    const user = userEvent.setup();
    render(<Header />);
    await user.click(screen.getByRole("button", { name: /open menu/i }));
    const mobile = screen.getByRole("navigation", { name: "Mobile" });
    expect(within(mobile).getByRole("link", { name: "Taxi Services" })).toBeInTheDocument();
    await user.keyboard("{Escape}");
    expect(screen.queryByRole("navigation", { name: "Mobile" })).not.toBeInTheDocument();
  });
});
