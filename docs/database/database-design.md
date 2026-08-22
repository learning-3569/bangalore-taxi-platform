# Database design

**Status:** Design only. No schema is created in Phase 0. Phase 1 introduces PostgreSQL, EF Core, and initial migrations.

Engine: PostgreSQL. See [ADR-003](../decisions/ADR-003-postgresql.md).

## Principles

- One database for the modular monolith.
- Referential integrity via foreign keys.
- Unique constraints for natural keys (email, phone, vehicle number, SEO slug).
- Indexes for lookup patterns (booking date, status, customer, vehicle assignment windows).
- UTC timestamps (`timestamptz`).
- Soft operational states (active/inactive) where history matters; do not silently delete bookings.
- No payment tables in V1.

## Future entities

Logical model (not implemented):

```text
User
Customer
AdminUser
Driver
Vehicle
Booking
BookingStatusHistory
VehicleAssignment          # overlap protection
PricingRule
VehiclePricing
Notification
SeoPage
SeoPageFaq
Review
AuditLog
```

Payment, refund, invoice, and coupon tables are excluded from V1.

## Conceptual booking

| Field | Notes |
| --- | --- |
| Booking ID | Stable public identifier (UUID) |
| Customer | FK |
| Pickup address, lat, lng | Required at submit |
| Drop address, lat, lng | Required for the V1 trip types that need a drop |
| Booking date / pickup time | Stored in timezone-aware form; display in IST |
| Vehicle type requested | Distinct from assigned vehicle |
| Estimated distance / fare | Snapshot at request time |
| Driver / Vehicle | Nullable until assignment |
| Status | See status list |
| Customer notes | Bounded length |
| Created/Updated | Audit timestamps |

Statuses: `Pending`, `Accepted`, `Rejected`, `DriverAssigned`, `Confirmed`, `DriverEnRoute`, `PickedUp`, `Completed`, `Cancelled`.

Status changes append `BookingStatusHistory` (who, when, from, to, reason).

## Driver and vehicle

Driver: name, mobile, license information, status, optional assigned default vehicle.

Vehicle: registration number (unique), type, capacity, status, availability flag.

A vehicle may be the default for a driver and still be assigned per booking. Assignment overlap is enforced on booking assignments, not on the default link.

## Pricing

`PricingRule` and `VehiclePricing` hold base fare, minimum fare, per-km, airport, toll, waiting, night, one-way, round-trip, and outstation components. Rules are versioned or dated so historical bookings keep the fare they were quoted.

## SEO pages

`SeoPage`: slug (unique), title, meta description, h1, content, canonical URL, featured image, publish status, created/updated. `SeoPageFaq` for FAQ items used in FAQ structured data.

## Users

Prefer a single `User` identity table with role, plus `Customer` / `AdminUser` / `Driver` profile tables keyed to `User` if those profiles diverge. Exact split is decided in Phase 1; do not invent unused tables.

## Indexes (planned)

- `booking (status, pickup_at)`
- `booking (customer_id, pickup_at desc)`
- `booking (assigned_vehicle_id, pickup_at)` where vehicle is not null
- `seo_page (slug)` unique
- `vehicle (registration_number)` unique

## Double-booking protection

Problem: two bookings must not receive the same vehicle for overlapping times.

Approach (implement in the assignment phase, not Phase 0):

1. **Availability check** inside the assignment service: load existing assignments for the vehicle whose time window overlaps `[pickup, estimated_end]` (end may be derived from duration + buffer).
2. **Database transaction** with a serializable or repeatable-read transaction, or a row lock on the vehicle (`SELECT … FOR UPDATE`) before insert.
3. **Exclusion constraint** (preferred on PostgreSQL) using `tstzrange` and `btree_gist`:

   ```text
   no two rows for the same vehicle_id with overlapping assignment_window
   unless status is cancelled/rejected
   ```

   Cancelled rows must be excluded from the constraint (partial unique/exclusion index).

4. **Buffers**: operational gap between trips (to be defined with the business before Phase 5/8).

The UI must not be the only guard. Two admins assigning the same car concurrently must produce one success and one conflict error.

## Concurrency on booking updates

Use a `xmin`/rowversion or `UpdatedAt` concurrency token on `Booking` so accept/reject/assign does not silently overwrite another admin's change.

## Auditability

`AuditLog`: actor, action, entity type, entity id, timestamp, metadata JSON. Do not store secrets in metadata.

## Transaction safety

Booking request creation, status transitions, and vehicle assignment each run in a single DB transaction. Notification send happens after commit (outbox in Phase 9 if needed). Do not send WhatsApp inside an open booking transaction.

## Phase 1 expected delivery

Initial empty-or-core schema, EF configuration, migrations, local PostgreSQL instructions. Entity set in Phase 1 should be the minimum needed to connect and migrate, not the entire model above unless that phase's spec says otherwise.
