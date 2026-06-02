"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import SiteHeader from "@/components/SiteHeader";
import { GameSummary, getGames } from "@/lib/api";

export default function HomePage() {
  const [games, setGames] = useState<GameSummary[]>([]);
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    let shouldIgnore = false;

    async function loadHomepageGames() {
      try {
        const gameData = await getGames();

        if (!shouldIgnore) {
          // For now, use the first 5 games as the "Top 5".
          // Later, this can come from likes/favorites/trending data.
          setGames(gameData.slice(0, 5));
        }
      } catch {
        if (!shouldIgnore) {
          setError("Unable to load homepage games.");
        }
      } finally {
        if (!shouldIgnore) {
          setIsLoading(false);
        }
      }
    }

    loadHomepageGames();

    return () => {
      shouldIgnore = true;
    };
  }, []);

  return (
    <>
      <SiteHeader />

      <main className="homepage">
        <section className="hero-section">
          <img
            src="/images/Zelda_the_hyrule_archives.png"
            alt="The Hyrule Archives"
            className="hero-logo"
          />

          <p className="hero-text">
            Explore the history, games, platforms, release regions, and community
            activity of The Legend of Zelda series.
          </p>
        </section>

        <section className="homepage-section">
          <div className="section-heading-row">
            <h2>Top 5 Zelda Games</h2>
            <Link href="/games">View all games</Link>
          </div>

          {isLoading && <p>Loading games...</p>}
          {error && <p className="error-text">{error}</p>}

          {!isLoading && games.length === 0 && (
            <p>No Zelda games are available yet.</p>
          )}

          <div className="game-carousel" aria-label="Top Zelda games">
            {games.map((game) => (
              <Link
                key={game.id}
                href={`/games/${game.id}`}
                className="game-card"
              >
                <div className="game-card-image">
                  {game.imageUrl ? (
                    <img src={game.imageUrl} alt={`Box art for ${game.title}`} />
                  ) : (
                    <span>Box Art</span>
                  )}
                </div>

                <h3>{game.title}</h3>

                <p>
                  {game.summary
                    ? game.summary.slice(0, 120) + "..."
                    : "No summary available yet."}
                </p>
              </Link>
            ))}
          </div>
        </section>

        <section className="homepage-grid">
          <aside className="trending-preview">
            <h2>Trending Preview</h2>

            <p>
              This section will show games that are receiving the most favorites,
              likes, dislikes, and comments.
            </p>

            <Link href="/trends">View trends</Link>
          </aside>

          <section className="recent-activity-preview">
            <h2>Recent Comments, Likes, and Dislikes</h2>

            <p>
              Community activity will appear here so the homepage feels lived in.
              Each comment preview will eventually link to the specific game page.
            </p>

            <ul>
              <li>A user saved a Zelda game to their collection.</li>
              <li>A user updated a game status to Completed.</li>
              <li>A user viewed MobyGames Zelda data.</li>
            </ul>
          </section>
        </section>
      </main>

      <footer className="site-footer">
        <p>
          The Legend of Zelda materials, names, and related intellectual property
          are owned by Nintendo. This is a non-commercial student project.
        </p>

        <p>Third-party game metadata pages include required source attribution.</p>
      </footer>
    </>
  );
}