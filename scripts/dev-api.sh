#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")/../apps/api"
dotnet run --launch-profile http
