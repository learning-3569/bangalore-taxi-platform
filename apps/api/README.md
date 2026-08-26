# API project

Modular monolith host (`BangaloreTaxi.Api`). Phase 2 is the HTTP kernel. Do not create empty business module folders ahead of their phases.

```text
Application/     Cross-cutting exceptions
Configuration/   Options
Hosting/         Pipeline, DI, errors, health checks
Health/          GET /api/health
Persistence/     EF Core (Phase 1)
```

Planned modules (later phases): Authentication, Customers, Bookings, Pricing, Drivers, Vehicles, Notifications, SEO, Administration.

There is no Payment module in V1.

Run (Development): `dotnet run --launch-profile http` → http://127.0.0.1:43130/swagger
