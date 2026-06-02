"use client";

import { useEffect, useState } from "react";
import { useParams } from "next/navigation";
import SiteHeader from "@/components/SiteHeader";
import { GameDetail, getCurrentUser, getGameById } from "@/lib/api";

export default function SpecificGamePage() {
  const params = useParams();

  const gameId = String(params.id);

  const [game, setGame] = useState<GameDetail | null>(null);
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [commentText, setCommentText] = useState("");
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    let shouldIgnore = false;

    async function loadGamePage() {
      try {
        const [gameData, currentUser] = await Promise.all([
          getGameById(gameId),
          getCurrentUser(),
        ]);

        if (shouldIgnore) {
          return;
        }

        setGame(gameData);
        setIsLoggedIn(currentUser !== null);
      } catch {
        if (!shouldIgnore) {
          setError("Unable to load this game page.");
        }
      } finally {
        if (!shouldIgnore) {
          setIsLoading(false);
        }
      }
    }

    loadGamePage();

    return () => {
      shouldIgnore = true;
    };
  }, [gameId]);

  function handleSubmitComment(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();

    // This is only a frontend placeholder for now.
    // Later, we will connect this to a real comments backend endpoint.
    setCommentText("");
  }

  return (
    <>
      <SiteHeader />

      <main className="specific-game-page">
        {isLoading && <p>Loading game...</p>}
        {error && <p className="error-text">{error}</p>}

        {!isLoading && game && (
          <>
            <section className="specific-game-layout">
              <aside className="specific-game-left">
                <div className="specific-game-art">
                  {game.imageUrl ? (
                    <img
                      src={game.imageUrl}
                      alt={`Box art for ${game.title}`}
                    />
                  ) : (
                    <span>Zelda game box art</span>
                  )}
                </div>

                <div className="reaction-box">
                  <h2>Community Reactions</h2>

                  <p>Likes: Coming soon</p>
                  <p>Dislikes: Coming soon</p>
                </div>
              </aside>

              <section className="specific-game-info">
                <h1>{game.title}</h1>

                <div className="release-panel">
                  <h2>Release Date and Platform(s)</h2>

                  {game.releases.length === 0 ? (
                    <p>No release information available yet.</p>
                  ) : (
                    <ul>
                      {game.releases.map((release) => (
                        <li key={release.id}>
                          <strong>{formatDate(release.releaseDate)}</strong> —{" "}
                          {release.platformName} ({release.regionName})
                        </li>
                      ))}
                    </ul>
                  )}
                </div>

                <div className="summary-panel">
                  <h2>Summary</h2>

                  <p>{game.summary ?? "No summary available yet."}</p>
                </div>
              </section>
            </section>

            <section className="screenshots-section">
              <h2>Screenshots of Game</h2>

              <div className="screenshot-carousel">
                <div className="screenshot-placeholder">Screenshot 1</div>
                <div className="screenshot-placeholder">Screenshot 2</div>
                <div className="screenshot-placeholder">Screenshot 3</div>
              </div>

              <p className="helper-text">
                Screenshot carousel will be connected to external or imported
                image data later.
              </p>
            </section>

            <section className="comments-section">
              <h2>Comments Section for Game</h2>

              {isLoggedIn ? (
                <form onSubmit={handleSubmitComment} className="comment-form">
                  <label>
                    Write a comment:
                    <textarea
                      value={commentText}
                      onChange={(event) => setCommentText(event.target.value)}
                      placeholder="Share your thoughts about this Zelda game..."
                    />
                  </label>

                  <button type="submit">Post Comment</button>
                </form>
              ) : (
                <p>
                  Please log in to write and post a comment. If you are not
                  logged in, this page will let you read comments but not post.
                </p>
              )}

              <div className="comment-list-placeholder">
                <p>No comments loaded yet.</p>
                <p>
                  Later, this area will show comments for this specific game and
                  will be scrollable.
                </p>
              </div>
            </section>
          </>
        )}
      </main>

      <footer className="site-footer">
        <p>
          The Legend of Zelda materials, names, and related intellectual
          property are owned by Nintendo. This is a non-commercial student
          project.
        </p>
      </footer>
    </>
  );
}

function formatDate(value: string): string {
  return new Date(value).toLocaleDateString();
}
