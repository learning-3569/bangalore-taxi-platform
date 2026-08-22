# ADR-004: No online payment in V1

## Status

Accepted (Phase 0)

## Context

The current business flow is request → admin accept → assign driver → confirm → notify. Customers are not asked to pay on the website today. Adding Razorpay, Stripe, webhooks, refunds, invoices, or coupons would enlarge scope, compliance, and failure modes before the booking desk works.

## Decision

V1 includes **no** payment capability:

- No payment module, service, controller, UI, webhook, or table
- No Razorpay or Stripe integration
- No refund, coupon, or invoice features
- No placeholder payment code

Online payment may be introduced only as an explicit future phase, with a new ADR for provider and capture timing.

## Consequences

- Bookings are requests and operational records, not commerce orders.
- Fare is an estimate, not a captured charge.
- Future payment should be a new module with its own tables, not columns bolted onto `Booking` without review.

## Alternatives considered

- Build payment “stubs” now: rejected; they rot and invite accidental scope.
- Cash-on-delivery flags: not required for V1; add only if operations ask.
