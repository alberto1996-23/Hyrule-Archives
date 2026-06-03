# Product Brief: The Hyrule Archives

## App Name and Core Purpose

The Hyrule Archives is a full-stack community website dedicated to the recognition, history, and community discussion of The Legend of Zelda series.

The purpose of the app is to give Zelda fans one organized place to explore the series through game information, release history, platforms, screenshots, community reactions, comments, and user collections. Visitors can browse Zelda titles and view public community data, while signed-in users can build their own personal Zelda collection, favorite games, like or dislike titles, post comments, and track their activity history.

The app is not meant to be only a static Zelda encyclopedia. It is meant to feel like a community archive where many users are interacting with the same games, creating trends, discussions, and live activity across the site.

## Audience and Problem

The app is for Zelda fans, Nintendo fans, collectors, and players who want one organized place to explore The Legend of Zelda series. Instead of being at the mercy of corporate social media sites or Nintendo.com, users can view this information in one focused archive.

The app also solves the problem of passive browsing. Signed-in users can create their own “My Legend of Zelda Collection,” save favorite games, react to titles, and participate in comments. Their interactions will contribute to public community features such as trending games, most favorited titles, most discussed games, and recent activity.

## Domain

The chosen domain is:

**Video Games / Gaming History / Community Archive**

This project focuses specifically on The Legend of Zelda series, its release history, platforms, game information, and fan activity.

## Third-Party API

The primary third-party API will be the **MobyGames Video Games Database API**. I plan to use MobyGames to fetch Zelda game data such as game titles, descriptions, release dates, platforms, screenshots, ratings, and other video game metadata.

MobyGames will be used for the main game-level data. The Hyrule Compendium API would be used as an optional extra feature to make the site feel more like a complete Zelda archive. The MobyGames API is integrated through the ASP.NET Core backend so the API key is not exposed in the frontend. The React /mobygames page displays third-party Zelda metadata including titles, MobyGames IDs, Moby scores, descriptions, platforms, cover images, and required attribution.

The development seed endpoint creates 500 simulated domain records, 50 simulated users, and 1,000 interaction records. These records make the app feel populated while keeping simulated data separate from real OAuth users and real third-party API data.

## Advanced Integration

My advanced integration will be **SignalR**.

I plan to create a live community activity feed that updates when users favorite a game, like or dislike a title, or post a comment. This will help the site feel active and populated because users will be able to see activity happening across the community in real time.

Example live activity events could include:

- A user favorited *The Legend of Zelda: Ocarina of Time*
- A user commented on *Breath of the Wild*
- A user liked *Tears of the Kingdom*
- A game became one of the most discussed titles

This feature will help show that the site supports multiple users interacting with the same archive, not just one user managing private data.

## Public Users vs. Signed-In Users

Public users who are not logged in can:

- View Legend of Zelda titles
- View screenshots of gameplay
- View release dates, platforms, and summaries
- Search for Zelda titles
- Filter games by platform, region, or title
- View comments
- View likes and dislikes
- View trending Zelda titles
- View public community activity

If a public user tries to favorite a game, like or dislike a title, post a comment, or access a private collection, they will be redirected to the login screen.

Signed-in users can:

- Create and manage their “My Legend of Zelda Collection”
- Favorite games in the Zelda series
- Like or dislike Zelda titles
- Post comments
- Edit or delete their own comments
- View their own activity history
- Contribute to public trends and community statistics
- See live updates from other users through the SignalR activity feed

## Multi-User and Lived-In Community Features

The Hyrule Archives will include community-driven features so the site feels active and lived in. Signed-in users will create records such as favorites, reactions, comments, and activity events. These records will not only belong to the individual user, but will also contribute to public community features.

The app will show community-based data such as:

- Trending Zelda games
- Most favorited games
- Most liked games
- Most disliked games
- Most commented games
- Recent comments
- Recent activity from users
- Popular platforms
- Community activity by game

The site will also include simulated users and seeded interaction records so that the archive feels populated before real users begin using it. These simulated users will help create realistic activity such as comments, favorites, likes, dislikes, and activity history.

This will make the app feel like an active Zelda community instead of an empty database.

## Grouping and Organization of Artifacts

The app will organize Zelda-related artifacts in multiple ways so users can explore the archive from different angles.

Artifacts may include:

- Games
- Platforms
- Release dates
- Regions
- Screenshots
- Comments
- Favorites
- Reactions
- Activity events
- Compendium entries
- Items, creatures, monsters, materials, and equipment

The app will group these artifacts by:

### Game

Each Zelda game will have its own detail page showing its summary, platforms, screenshots, release information, comments, likes, dislikes, and recent activity.

### Platform

Users will be able to explore games by console or platform, such as NES, SNES, Nintendo 64, GameCube, Wii, Wii U, Nintendo DS, Nintendo 3DS, and Nintendo Switch.

### Region

Since Zelda games can have different release dates depending on location, the app will support release information by region. Regions may include Japan, North America, Europe, and international/global releases.

### Community Activity

The app will also group activity by game, showing which games are being favorited, commented on, liked, or disliked the most.

## Main Entities and Relationships

The main entities will include:

| Entity | Purpose |
|---|---|---|---|---|---|
| User | Stores authenticated users linked to OAuth/OIDC identity |
| Game | Stores Zelda title data imported or cached from RAWG |
| Platform | Stores consoles/platforms like NES, N64, GameCube, Wii, Switch, etc. |
| Region | Stores release regions such as Japan, North America, Europe, or Global |
| GameRelease | Connects a game to a platform, region, and release date |
| Collection | Represents a user’s “My Legend of Zelda Collection” |
| CollectionItem | Stores games saved inside a user’s collection |
| Favorite | User-owned record showing which games a user saved |
| Reaction | User-owned like/dislike record for a game |
| Comment | User-owned comment on a game or the series |
| ActivityEvent | Tracks user actions for activity history and the SignalR live feed |

The main relationships will include:

- One User can have one or more Collections.
- One Collection can have many CollectionItems.
- One Game can appear in many CollectionItems.
- One User can have many Favorites.
- One User can have many Comments.
- One User can have many Reactions.
- One User can have many ActivityEvents.
- One Game can have many Comments.
- One Game can have many Reactions.
- One Game can have many Favorites.
- One Game can have many GameReleases.
- One Platform can have many GameReleases.
- One Region can have many GameReleases.
- Many Games can appear on many Platforms through GameRelease or GamePlatform.
- ActivityEvents will support both the user’s private activity history and the public live community feed.

## Five Most Important User Stories

1. As a visitor, I want to browse Zelda games grouped by platform, region, and release history so that I can understand how the series developed over time.

2. As a visitor, I want to view trending Zelda games based on community favorites, likes, dislikes, and comments so that I can see what the community is currently interested in.

3. As a signed-in user, I want to create and manage my own Zelda collection so that I can save the games that matter most to me.

4. As a signed-in user, I want to like, dislike, and comment on Zelda games so that I can participate in the community discussion.

5. As a signed-in user, I want to see live activity from other users so that the site feels active, connected, and lived in.

## OAuth Provider

I plan to use **Google OAuth / OpenID Connect** for authentication.

When a user signs in for the first time, the backend will create a user profile in the database linked to the provider’s subject identifier. Signed-in users will be able to create private user-owned data such as collections, favorites, reactions, comments, and activity history.

The backend will enforce user isolation so that one user cannot view, edit, or delete another user’s private collection, comments, favorites, or activity history.

## Hosting Plan

I plan to host the React frontend on **Vercel** and the ASP.NET Core backend on **Render** or **Railway**.

The production database will be **PostgreSQL** through a hosted database provider such as Neon, Supabase, Render, or Railway.

Environment variables will store API keys, OAuth credentials, and database connection strings so that no secrets are committed to the repository.
