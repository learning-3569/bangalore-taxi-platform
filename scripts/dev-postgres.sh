#!/usr/bin/env bash
set -euo pipefail
root="$(cd "$(dirname "$0")/.." && pwd)"
cd "$root"

docker compose up -d postgres

echo "Waiting for PostgreSQL to become healthy..."
for _ in $(seq 1 30); do
  health_status="$(docker inspect --format='{{if .State.Health}}{{.State.Health.Status}}{{else}}{{.State.Status}}{{end}}' bangalore-taxi-pg 2>/dev/null || true)"
  if [[ "$health_status" == "healthy" ]]; then
    echo "PostgreSQL is healthy at 127.0.0.1:5432 (user bangalore_taxi, database bangalore_taxi)."
    echo "Apply schema: dotnet ef database update --project apps/api/BangaloreTaxi.Api.csproj --startup-project apps/api/BangaloreTaxi.Api.csproj"
    exit 0
  fi
  sleep 1
done

echo "PostgreSQL did not become healthy in time. Logs:"
docker logs bangalore-taxi-pg
exit 1
