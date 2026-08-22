# Tests

- `unit/` — xUnit tests for API models and, later, domain/application services.
- `integration/` — HTTP tests using `WebApplicationFactory`.

```bash
dotnet test BangaloreTaxi.sln
```

PostgreSQL integration tests belong to later phases when the database exists.
