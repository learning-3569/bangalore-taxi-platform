# ADR-003: PostgreSQL as the primary database

## Status

Accepted (Phase 0)

## Context

The system needs relational data with foreign keys, unique constraints, and strong transactional guarantees—especially to prevent two bookings from using the same vehicle at overlapping times.

## Decision

Use PostgreSQL as the only primary database, accessed via EF Core from the modular monolith.

## Consequences

- Exclusion constraints and `tstzrange` are available for assignment overlap.
- One backup/restore story.
- JSON columns can hold sparse metadata without a second store.
- No additional database product in V1.

## Alternatives considered

- SQL Server: viable with EF Core, but PostgreSQL is specified and is a strong fit for constraints and hosting flexibility.
- MongoDB: weaker integrity for assignments and financial-adjacent fare snapshots.
- Multiple databases per module: rejected with the monolith decision.
