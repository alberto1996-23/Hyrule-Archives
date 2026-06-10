# Security Review

I treated security as a backend-enforced concern, not just a frontend convenience feature. This review summarizes the protections currently in the project, the checks I ran, and the risks I still want to monitor.

## Authentication model

The app uses Google OAuth/OIDC through the ASP.NET Core backend.

### Authentication protections

- The browser session uses a server-issued auth cookie instead of exposing tokens to the React client.
- The backend creates or resolves the local `AppUser` from provider claims.
- Protected endpoints rely on ASP.NET Core authentication and authorization middleware.
- API requests receive proper `401` and `403` status codes instead of HTML redirect pages.
- Secrets are expected through environment variables or user secrets, not committed config.

## Authorization and user isolation

The most important rule in this project is that one user cannot modify another user's private data.

### Authorization protections

- Collection endpoints are protected and scoped to the current user.
- Comment update/delete flows check ownership before allowing changes.
- Reaction writes require authentication.
- Protected frontend routes exist for member-only views, but the real protection still lives on the API.
- Backend tests verify unauthorized requests are rejected.

## Data and configuration safety

### Data protections

- SQLite is used for local development; the code is PostgreSQL-ready for production.
- Migrations are used instead of `EnsureCreated()`.
- The MobyGames API key stays on the server.
- CORS is explicitly configured around allowed frontend origins.
- Development-only seed and auth endpoints are gated away from production.

## Error handling

The backend uses centralized exception handling and structured API error helpers so failures do not leak raw stack traces into normal client responses.

## Dependency review

I ran vulnerability checks against both the .NET and frontend dependency graphs.

### Backend packages

I checked both backend projects with NuGet vulnerability scanning.

**Result:** no known vulnerable packages were reported for:

- `backend/backend.csproj`
- `backend.tests/backend.tests.csproj`

### Frontend packages

I ran `npm audit` in the frontend.

**Result:** 2 moderate vulnerabilities were reported in the dev toolchain.

1. `esbuild` — advisory about dev-server request exposure
2. `vite` — advisory about path traversal in optimized dependency sourcemap handling

### Risk assessment

- These findings are in the frontend tooling layer rather than my production runtime code.
- They still matter during development and CI, especially if the dev server is exposed on an untrusted network.
- The recommended automated fix is a major-version Vite upgrade, so I left that as a deliberate follow-up instead of forcing a risky last-minute toolchain jump.

## Security testing evidence

I validated security behavior through automated tests and manual flow review.

### Automated evidence

- Backend integration tests currently pass with protected endpoint coverage
- Unauthorized mutation attempts return `401`
- Authenticated user flows can read their own data
- Comment ownership protections are covered in the API test suite

## Remaining risks and follow-up work

If I keep iterating after submission, these are my next security tasks.

1. Upgrade Vite and re-run the frontend test/build pipeline.
2. Move production cookie settings to strict HTTPS-only production values after deployment.
3. Add rate limiting around comment/reaction endpoints.
4. Add anti-abuse monitoring for the dev seed endpoints even though they are development-only.
5. Add structured audit logging for moderation or future admin actions.

## Bottom line

The core application security model is solid for the scope of this project: auth is server-driven, authorization is backend-enforced, third-party secrets stay off the client, and the test suite now covers the most important protected behaviors. The main unresolved issue is the moderate frontend tooling advisory, which I documented rather than hiding.
