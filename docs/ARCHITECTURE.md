# Architecture

I built The Hyrule Archives as a React + ASP.NET Core system with a clear split between presentation, API logic, persistence, external integrations, and live updates. I wanted the app to feel like a real community product, so I designed it around two kinds of data at the same time: curated Zelda catalog data and user-owned community activity.

## High-level shape

- **Frontend:** React + Vite single-page app in `frontend/`
- **Backend:** ASP.NET Core minimal API on .NET 10 in `backend/`
- **Database:** SQLite for local development, PostgreSQL-ready connection handling for production
- **ORM:** Entity Framework Core with migrations
- **Auth:** Google OAuth/OIDC through backend cookie authentication
- **Real-time:** SignalR activity hub at `/hubs/activity`
- **External data:** MobyGames API, proxied through the backend

## Request flow

1. A user loads the React app.
2. React calls the backend for public catalog data, trends, highlights, or authenticated user data.
3. The backend resolves the current user from the auth cookie when a protected endpoint is hit.
4. Entity Framework Core reads or writes relational data.
5. For external Zelda metadata, the backend calls MobyGames and normalizes the response before sending it to the frontend.
6. When a signed-in user performs a tracked action, the backend records an `ActivityEvent` and broadcasts it through SignalR so the live feed updates without a refresh.

## Frontend structure

I organized the frontend around route-focused pages and reusable components.

### Main routes

#### Public routes

- `/`
- `/login`
- `/auth/callback`
- `/games`
- `/games/:gameSlug`
- `/games/moby/:mobyGameId`
- `/trends`

#### Protected routes

- `/dashboard`
- `/my-collection`
- `/my-favorites`
- `/my-activity`
- `/community/live`

### Frontend responsibilities

- Route protection with `RequireAuth`
- Fetching and rendering public catalog data
- Managing authenticated collection, favorite, reaction, and comment flows
- Showing loading, empty, and error states
- Running inline validation for reusable forms
- Listening for SignalR activity updates through `useLiveActivityFeed`

## Backend structure

I kept the backend modular even though it uses minimal APIs.

### Main backend areas

- `Endpoints/`: HTTP surface area grouped by feature
- `Services/`: business logic, third-party integration, seeding, and activity creation
- `Data/`: `AppDbContext`
- `Models/`: EF Core entities
- `DTOs/`: API response and request contracts
- `Infrastructure/`: shared helpers and API error formatting
- `Hubs/`: SignalR hub definitions
- `Validation/`: request validation wiring

### Public API examples

- `GET /api/games`
- `GET /api/catalog/zelda`
- `GET /api/community/highlights`
- `GET /api/activity/recent`
- `GET /api/games/{id}/comments`
- `GET /api/games/{id}/reactions/summary`

### Protected API examples

- `GET /api/me`
- `GET /api/me/collection`
- `GET /api/me/collection/favorites`
- `POST /api/me/collection`
- `PUT /api/me/collection/{id}`
- `DELETE /api/me/collection/{id}`
- `POST /api/games/{id}/comments`
- `PUT /api/comments/{commentId}`
- `DELETE /api/comments/{commentId}`
- `PUT /api/games/{id}/reaction`
- `DELETE /api/games/{id}/reaction`

## Data model

The core domain mixes curated Zelda catalog records with user interaction data.

### Core entities

- `AppUser`
- `Game`
- `Platform`
- `Region`
- `GameRelease`
- `CollectionItem`
- `Comment`
- `Reaction`
- `ActivityEvent`
- `ZeldaCatalogGame`
- `ZeldaCatalogAvailability`
- `ZeldaCatalogCollectionAppearance`

### Important relationships

- One `AppUser` to many `CollectionItem`
- One `AppUser` to many `Comment`
- One `AppUser` to many `Reaction`
- One `AppUser` to many `ActivityEvent`
- One `Game` to many `Comment`
- One `Game` to many `Reaction`
- One `Game` to many `CollectionItem`
- One `Game` to many `GameRelease`
- Many-to-many between games and platforms through `GameRelease`
- Curated catalog records linked to multiple availability and collection appearance rows

## Authentication and authorization design

I chose backend cookie authentication because it fit the React + ASP.NET setup well and let me enforce security on the server instead of trusting the client.

### Auth flow

1. The user starts login from the frontend.
2. The backend redirects to Google.
3. Google returns to the backend callback.
4. The backend validates identity claims.
5. The backend creates or updates the local `AppUser` record.
6. The backend issues the application auth cookie.
7. Protected endpoints use that cookie for authorization.

### Authorization rules

- Anonymous requests to protected endpoints receive `401`
- Cross-user mutations are blocked with ownership checks and `403`/not-found protections where appropriate
- Only the signed-in owner can edit or delete their comment or collection data
- Development-only login helpers are excluded from production

## External API strategy

I did not call MobyGames directly from React. Instead, I proxied those requests through the backend so I could:

- keep the API key off the client
- normalize inconsistent upstream data
- cache responses
- return a stable DTO shape to the frontend
- separate upstream failures from my own application errors

## Real-time design

The live feature is a SignalR activity feed.

- The backend writes an `ActivityEvent` when a user performs a tracked action
- `ActivityEventService` broadcasts the new event to `/hubs/activity`
- The frontend hook reconnects automatically and prepends new events into the live list
- I cap and deduplicate client items so the feed stays usable

## Seed and simulation strategy

I wanted the product to feel active before real users sign in.

### Development seed targets

- Milestone seed: 500 domain records, 50 simulated users, 1,000 interactions
- Final seed: 5,000 domain records, 500 simulated users, 14,500 interactions total
  - 5,000 collection items
  - 3,000 comments
  - 2,500 reactions
  - 4,000 activity events

The seed flow only resets simulated records, so real OAuth users and curated Zelda catalog data remain intact.

## Testing architecture

I used layered testing because different risks live in different places.

- **Frontend unit/component tests:** route guards, reusable forms, trend rendering
- **Backend integration tests:** protected endpoint behavior and auth enforcement
- **End-to-end tests:** signed-in login, collection management, and comment lifecycle
- **Accessibility scan:** Playwright + axe on key public and signed-in surfaces

## Tradeoffs

- I used minimal APIs for speed and clarity, but I still separated logic into feature modules and services so the codebase would not turn into one giant startup file.
- I kept deployment provider choice flexible rather than hard-coding one platform into the application code.
- I added a development-only auth shortcut for test automation because real OAuth popups are brittle in CI.
- I used curated catalog tables alongside imported game/community tables so I could control the Zelda presentation while still proving meaningful third-party API integration.
