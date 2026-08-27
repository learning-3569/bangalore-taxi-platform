export class OtpCooldownError extends Error {
  readonly retryAfterSeconds: number;

  constructor(retryAfterSeconds: number) {
    const seconds = Math.max(1, Math.floor(retryAfterSeconds));
    super(`You can request another code in ${seconds} seconds`);
    this.name = "OtpCooldownError";
    this.retryAfterSeconds = seconds;
  }
}

export function parseRetryAfterSeconds(response: Response, body: { retryAfterSeconds?: unknown }): number | null {
  const header = response.headers.get("Retry-After");
  if (header && /^\d+$/.test(header.trim())) {
    const fromHeader = Number(header.trim());
    if (fromHeader > 0) return fromHeader;
  }
  if (typeof body.retryAfterSeconds === "number" && body.retryAfterSeconds > 0) {
    return Math.floor(body.retryAfterSeconds);
  }
  return null;
}

export function cooldownMessage(seconds: number): string {
  return `You can request another code in ${seconds} seconds`;
}
