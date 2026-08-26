#!/usr/bin/env bash
set -euo pipefail
root="$(cd "$(dirname "$0")/.." && pwd)"

echo "==> Building public website"
(cd "$root/apps/web" && npm run build)

echo "==> Building admin portal"
(cd "$root/apps/admin" && npm run build)

echo "==> Building and testing API"
(cd "$root" && dotnet test BangaloreTaxi.sln --nologo)

echo "==> Phase 1 schema tests need Docker (Testcontainers)."
