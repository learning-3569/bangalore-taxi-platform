# ADR-001: Modular monolith

## Status

Accepted (Phase 0)

## Context

The Bangalore Taxi Booking Platform serves a fleet of about 20 cars. The product needs a public SEO website, an admin portal, and a backend for bookings, pricing, drivers, vehicles, and later notifications.

A microservices design (many deployable services, Kafka, Kubernetes, service mesh) is a common default in large-scale systems. It is not a default that fits this business.

## Decision

Build a modular monolith: one ASP.NET Core API process, one PostgreSQL database, modules separated by business capability. Two frontend apps talk to that API.

## Consequences

- Simpler operations, transactions, and local development.
- Double-booking protection can use a single database transaction and PostgreSQL constraints.
- Modules can still be extracted later if a real scale or team boundary appears.
- Contributors must respect module boundaries inside the repo; a monolith is not an excuse for a single unstructured folder.

## Alternatives considered

- Microservices: rejected as operational overhead without a matching scale or team size.
- Serverless function-per-endpoint: rejected; booking assignment needs transactional consistency.
