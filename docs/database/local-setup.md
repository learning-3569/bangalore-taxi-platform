# Local PostgreSQL (Phase 1)

The API uses one PostgreSQL 16 database. Schema is applied with EF Core migrations. Credentials below are **local development only**. Do not use them in production. Do not commit production passwords.

## Start PostgreSQL

From the repository root:

```bash
docker compose up -d
```

Wait until the container is healthy (`docker ps` should show `healthy` for `bangalore-taxi-pg`). Helper:

```bash
./scripts/dev-postgres.sh
```

Stop:

```bash
docker compose down
```

`docker compose down` keeps the named volume `bangalore_taxi_pgdata`. To wipe local data as well:

```bash
docker compose down -v
```

## Connection

```text
Host=127.0.0.1
Port=5432
Database=bangalore_taxi
Username=bangalore_taxi
Password=dev
```

Same values are in `apps/api/appsettings.Development.json`. Override with `ConnectionStrings__DefaultConnection` in `apps/api/.env` if needed.

## Apply migrations

```bash
dotnet tool restore
dotnet ef database update --project apps/api/BangaloreTaxi.Api.csproj --startup-project apps/api/BangaloreTaxi.Api.csproj
```

## Configuration (not hardcoded in C#)

| Setting | Development default | Where |
| --- | --- | --- |
| Assignment buffer | 15 minutes | `operational_setting` + `Operations:AssignmentBufferMinutes` |
| Default trip duration | 120 minutes | `operational_setting` + `Operations:DefaultTripDurationMinutes` |
| Pickup time zone | `Asia/Kolkata` | `Operations:DefaultTimeZone` |
| Currency | `INR` | `Operations:DefaultCurrencyCode` |

Production values will be set later. Application services in later phases should read settings, not compile them in.

## Tests

With Docker PostgreSQL running:

```bash
dotnet test BangaloreTaxi.sln
```

Schema tests connect to the Compose instance (they create/reset `bangalore_taxi_test` on the same server so they do not overwrite `bangalore_taxi`). In CI, `SCHEMA_TEST_CONNECTION` points at the GitHub Actions Postgres service.

If PostgreSQL is not running and Docker is unavailable, schema and `/health/ready` tests skip. Pipeline and unit tests still run.

API (after Compose is up):

```bash
cd apps/api && dotnet run --launch-profile http
```

- Live: http://127.0.0.1:43130/health/live
- Ready: http://127.0.0.1:43130/health/ready
- Swagger (Development): http://127.0.0.1:43130/swagger

