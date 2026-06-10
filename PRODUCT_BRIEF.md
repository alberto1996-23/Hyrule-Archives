# Product Brief: The Hyrule Archives

## App name and purpose

The Hyrule Archives is my Zelda-focused community archive. I built it to give fans one place to explore the series, track what they have played or collected, and participate in community discussion around each game.

I did not want this to feel like a static wiki. I wanted it to feel like a living archive where real users and simulated community activity make the product look active from the first visit.

## Audience and problem

My audience is Zelda fans, collectors, players, and people who enjoy video-game history.

The problem I am solving is fragmentation. Zelda information, release history, platform context, community discussion, and personal tracking usually live in different places. I wanted to combine them into one focused product with both public browsing and signed-in ownership features.

## Domain

### Domain summary

Video games / gaming history / fan community archive

The project is specifically centered on The Legend of Zelda series.

## Core product direction

The Hyrule Archives combines three layers of value:

1. **Curated archive value** through Zelda catalog and release presentation
2. **Community value** through comments, reactions, trends, and live activity
3. **Personal value** through a user-owned collection, favorites, and activity history

## Third-party API

The main third-party API is the **MobyGames API**.

I use it to enrich the project with Zelda game metadata that meaningfully affects the product experience. The backend proxies MobyGames requests so the API key stays private and the frontend receives a normalized response shape.

I also keep curated local Zelda catalog data because I wanted tighter control over the archive presentation, especially around release history, platform groupings, and collection appearances.

## Public users vs. signed-in users

### Public users can

- browse games
- open Zelda detail pages
- view trends and community highlights
- read public comments
- inspect reaction summaries
- explore release and platform information

### Signed-in users can

- open a personal dashboard
- add games to a personal collection
- update collection status
- favorite or unfavorite collection entries
- react to games
- create, edit, and delete comments
- review their own activity history
- watch the live community feed

## Advanced integration

My advanced integration is **SignalR**.

I use SignalR to power a live activity feed so the site feels active across multiple sessions. When users perform tracked actions, the backend creates an activity record and pushes the new event to connected clients.

## Lived-in community strategy

I wanted the app to feel populated even before a professor signs in with a real account, so I built a seed system that creates simulated users and interaction records.

### Final seed target

- 5,000 simulated domain records
- 500 simulated user profiles
- 14,500 interaction records total

That interaction layer includes collection items, comments, reactions, and activity events.

## Main entities

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

## Key relationships

- One user has many collection items
- One user has many comments
- One user has many reactions
- One user has many activity events
- One game has many collection items
- One game has many comments
- One game has many reactions
- One game has many releases
- Games and platforms connect through release records

## Most important user stories

1. As a visitor, I want to browse Zelda games and their release context so I can learn about the series history.
2. As a visitor, I want to see which games are trending so the archive feels active and community-driven.
3. As a signed-in user, I want to manage my own Zelda collection so I can track what I care about.
4. As a signed-in user, I want to react to and comment on games so I can participate in the community.
5. As a signed-in user, I want to watch a live activity feed so the site feels like a real shared space.

## Authentication choice

I use **Google OAuth / OpenID Connect** for real sign-in.

The backend creates a local application user from the provider subject identifier and then enforces ownership rules on private data. The frontend reflects auth state, but the actual security boundary is the API.

## Hosting and deployment plan

The repository now includes a GitHub Actions pipeline that builds, tests, runs Playwright, and can trigger deployment once production secrets are configured.

My intended hosting split is:

- frontend on Vercel
- backend on Render or another ASP.NET-friendly host
- PostgreSQL in production

I do not commit secrets. All sensitive configuration is expected through environment variables or platform secret storage.
