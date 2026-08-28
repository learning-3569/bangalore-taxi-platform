# Admin portal (`apps/admin`)

Internal Next.js App Router application. This origin remains non-indexable and provides the Phase 7 booking operations queue at `/bookings`. Admins use the shared phone OTP flow; the API's persisted `admin` role is the authorization boundary.

```bash
npm install
npm run dev    # http://127.0.0.1:43122
npm run build
```

Do not add public marketing pages here. Product documentation lives in the repository root `README.md` and `docs/`.
