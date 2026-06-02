"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import ProtectedRoute from "@/components/ProtectedRoute";
import SiteHeader from "@/components/SiteHeader";
import {
  CollectionItem,
  CurrentUser,
  getCurrentUser,
  getMyCollection,
} from "@/lib/api";

export default function DashboardPage() {
  const [user, setUser] = useState<CurrentUser | null>(null);
  const [favorites, setFavorites] = useState<CollectionItem[]>([]);
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    let shouldIgnore = false;

    async function loadDashboard() {
      try {
        // Load the signed-in user and their collection at the same time.
        // The collection is our early version of "favorites."
        const [currentUser, collectionData] = await Promise.all([
          getCurrentUser(),
          getMyCollection(),
        ]);

        if (shouldIgnore) {
          return;
        }

        setUser(currentUser);
        setFavorites(collectionData);
      } catch {
        if (!shouldIgnore) {
          setError("Unable to load your dashboard.");
        }
      } finally {
        if (!shouldIgnore) {
          setIsLoading(false);
        }
      }
    }

    loadDashboard();

    return () => {
      shouldIgnore = true;
    };
  }, []);

  // Dashboard sketch says only show up to 4 favorites.
  // If there are more than 4, show a "More" link.
  const previewFavorites = favorites.slice(0, 4);
  const hasMoreFavorites = favorites.length > 4;

  return (
    <ProtectedRoute>
      <SiteHeader />

      <main className="dashboard-page">
        <section className="dashboard-intro">
          <h1>Dashboard</h1>

          <p>
            Welcome back{user?.displayName ? `, ${user.displayName}` : ""}. This
            is your signed-in Hyrule Archives space.
          </p>
        </section>

        {isLoading && <p>Loading dashboard...</p>}
        {error && <p className="error-text">{error}</p>}

        {!isLoading && (
          <section className="dashboard-layout">
            <aside className="dashboard-side-panel">
              <section className="dashboard-card scroll-panel">
                <h2>Trends</h2>

                <p>
                  This will show community trends based on favorites, comments,
                  likes, and dislikes.
                </p>

                <ul>
                  <li>Most saved games</li>
                  <li>Most discussed games</li>
                  <li>Recently active titles</li>
                  <li>Popular platforms</li>
                </ul>

                <Link href="/trends">View trends</Link>
              </section>

              <section className="dashboard-card scroll-panel">
                <h2>Your Likes and Dislikes</h2>

                <p>
                  This will show your liked and disliked Zelda games once the
                  reactions feature is connected.
                </p>

                <ul>
                  <li>Liked games coming soon</li>
                  <li>Disliked games coming soon</li>
                </ul>
              </section>
            </aside>

            <section className="dashboard-main-panel">
              <section className="dashboard-card">
                <div className="dashboard-section-heading">
                  <h2>Your Favorites</h2>

                  {hasMoreFavorites && (
                    <Link href="/my-collection">More</Link>
                  )}
                </div>

                {previewFavorites.length === 0 ? (
                  <p>
                    You have not saved any Zelda games yet. Visit the games page
                    to start building your collection.
                  </p>
                ) : (
                  <div className="favorites-carousel">
                    {previewFavorites.map((item) => (
                      <Link
                        href={`/games/${item.gameId}`}
                        key={item.id}
                        className="favorite-card"
                      >
                        <div className="favorite-card-art">
                          {item.imageUrl ? (
                            <img
                              src={item.imageUrl}
                              alt={`Box art for ${item.gameTitle}`}
                            />
                          ) : (
                            <span>Zelda Favorite</span>
                          )}
                        </div>

                        <h3>{item.gameTitle}</h3>
                        <p>Status: {item.status}</p>
                      </Link>
                    ))}
                  </div>
                )}
              </section>

              <section className="dashboard-card recent-comments-panel">
                <h2>Your Recent Comments</h2>

                <p>
                  This section will show comments you posted while logged in.
                  Each comment will link back to the specific Zelda game page.
                </p>

                <div className="comment-preview-list">
                  <p>No comments loaded yet.</p>
                  <p>
                    Once comments are connected, this box will become scrollable.
                  </p>
                </div>
              </section>
            </section>
          </section>
        )}
      </main>

      <footer className="site-footer">
        <p>
          The Legend of Zelda materials, names, and related intellectual property
          are owned by Nintendo. This is a non-commercial student project.
        </p>
      </footer>
    </ProtectedRoute>
  );
}