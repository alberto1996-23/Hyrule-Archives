## Final Project Proposal

Name: The Hyrule Archives

- I want to create a website where we host the recognition and history of The Legend of Zelda games.
I want to use an API to be able to fetch the name, release date (both for Japan and international release),
and summary of each game in the UI. Alongside, I want there to be, for each title, screenshots of gameplay, and the platform its on.

what it solves:

- The app is for Zelda fans, Nintendo fans, collectors, and players who want one organized place to explore the Legend of Zelda series. Instead of searching across multiple websites for game summaries, release dates, screenshots, platforms, and community opinions, users can view this information in one focused archive. The app also solves the problem of passive browsing by letting signed-in users create a personal collection, save favorite games, react to titles, and participate in comments.

Domain: Video Games/ Gaming History/ Community Archive

- I chose the video game history and fan community domain, focused specifically on The Legend of Zelda series.

## OAUTH2.0 for login Vs. Public user

I plan to use Google OAuth / OpenID Connect for authentication. When a user signs in for the first time, the backend will create a user profile in the database linked to the provider’s subject identifier. Signed-in users will be able to create private user-owned data such as favorites, reactions, comments, and activity history.

- For users who login, they will be created a "My Legend of Zelda Collection" id. This will allow users to interact with the Hyrule Archives via liking or disliking specific Legend of Zelda titles, make comments (along with editing/deleting them), and have their likes or dislikes contribute to Legend of Zelda trends. Their My Legend of Zelda accounts also will hold the data for their:
        -Favorite games in the Legend of Zelda series
        -their liked and disliked titles
        -comments which they posted
        -activity history

- if they arent logged in, they can only
    -view Legend of Zelda titles, screenshots of gameplay, release date, platform, etc.
    -view comments and the likes to dislikes
    -search for Legend of Zelda titles (as well as use filters)
    -view trending Legend of Zelda titles

  if they try and do any of these things logged-in users do, they will be redirected to the login screen.

## API

- The primary third-party API will be the RAWG Video Games Database API. I plan to use it to fetch game titles, descriptions, release dates, platforms, screenshots, ratings, and other video game metadata. RAWG supports a large video game database, screenshots, release dates, platforms, ratings, and game descriptions. It requires an API key for requests.

I may also use the Hyrule Compendium API as a secondary API for Zelda-specific encyclopedia content such as creatures, monsters, materials, equipment, treasure, descriptions, categories, and images from Breath of the Wild / Tears of the Kingdom-style compendium data.

## Advance Integeration

- My advanced integration will be SignalR. I plan to create a live community activity feed that updates when users favorite a game, like or dislike a title, or post a comment. This will make the site feel active and populated, and it will demonstrate real-time updates across multiple browser sessions.

## Main Entities and Relationships

Entity                  Purpose
_____________________________________
User           Stores authenticated users linked to OAuth/OIDC identity
Game           Stores Zelda title data imported or cached from RAWG
Platform       Stores consoles/platforms like NES, N64, GameCube, Switch
GamePlatform   Join table connecting games to platforms
Favorite       User-owned record showing which games a user saved
Reaction       User-owned like/dislike record for a game
Comment           User-owned comment on a game or the series
ActivityEvent   Tracks user actions for activity history and SignalR feed
CompendiumEntry   Optional Zelda encyclopedia entries from Hyrule Compendium

One User has many Favorites
One User has many Comments
One User has many Reactions
One Game has many Comments
One Game has many Reactions
One Game has many Favorites
Many Games can appear on many Platforms
One User has many ActivityEvents

## 5 Most Important User Stories

1. As a visitor, I want to browse Zelda games so that I can learn about the series without creating an account.
2. As a visitor, I want to search and filter Zelda titles by platform or title so that I can quickly find a specific game.
3. As a signed-in user, I want to favorite Zelda games so that I can build my own personal Zelda collection.
4. As a signed-in user, I want to like, dislike, and comment on games so that I can contribute to community opinions and discussion.
5. As a signed-in user, I want to view my activity history so that I can see the games I interacted with over time.

## Hosting

- I plan to host the React frontend on Vercel and the ASP.NET Core backend on Render or Railway. The production database will be PostgreSQL through a hosted database provider such as Neon, Supabase, Render, or Railway. Environment variables will store API keys, OAuth credentials, and database connection strings.
