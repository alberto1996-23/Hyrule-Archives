"use client";

import { useEffect, useState } from "react";
import ProtectedRoute from "@/components/ProtectedRoute";
import {
  addGameToMyCollection,
  CollectionItem,
  GameSummary,
  getGames,
  getMyCollection,
} from "@/lib/api";

export default function MyCollectionPage() {
  const [games, setGames] = useState<GameSummary[]>([]);
  const [collection, setCollection] = useState<CollectionItem[]>([]);
  const [message, setMessage] = useState("");
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(true);

  async function refreshCollectionPage() {
    const [gamesData, collectionData] = await Promise.all([
      getGames(),
      getMyCollection(),
    ]);

    setGames(gamesData);
    setCollection(collectionData);
  }

  useEffect(() => {
    let shouldIgnore = false;

    async function loadInitialPageData() {
      try {
        const [gamesData, collectionData] = await Promise.all([
          getGames(),
          getMyCollection(),
        ]);

        if (shouldIgnore) {
          return;
        }

        setGames(gamesData);
        setCollection(collectionData);
      } catch {
        if (!shouldIgnore) {
          setError("Unable to load your collection page.");
        }
      } finally {
        if (!shouldIgnore) {
          setIsLoading(false);
        }
      }
    }

    loadInitialPageData();

    return () => {
      shouldIgnore = true;
    };
  }, []);

  async function handleAddGame(gameId: string) {
    setMessage("");
    setError("");

    try {
      await addGameToMyCollection(gameId);
      setMessage("Game added to your collection.");
      await refreshCollectionPage();
    } catch (err) {
      if (err instanceof Error) {
        setError(err.message);
      } else {
        setError("Something went wrong.");
      }
    }
  }

  const savedGameIds = new Set(collection.map((item) => item.gameId));

  return (
    <ProtectedRoute>
      <main>
        <h1>My Legend of Zelda Collection</h1>

        <p>
          Add Zelda games to your personal collection. This data belongs to your
          signed-in account and is saved in the backend database.
        </p>

        {isLoading && <p>Loading your collection...</p>}

        {message && <p>{message}</p>}
        {error && <p>{error}</p>}

        {!isLoading && (
          <>
            <section>
              <h2>Saved Games</h2>

              {collection.length === 0 ? (
                <p>You have not saved any games yet.</p>
              ) : (
                <ul>
                  {collection.map((item) => (
                    <li key={item.id}>
                      {item.gameTitle} — {item.status}
                    </li>
                  ))}
                </ul>
              )}
            </section>

            <section>
              <h2>Add a Game</h2>

              {games.length === 0 ? (
                <p>No games are available yet.</p>
              ) : (
                <ul>
                  {games.map((game) => {
                    const alreadySaved = savedGameIds.has(game.id);

                    return (
                      <li key={game.id}>
                        <strong>{game.title}</strong>

                        <p>{game.summary}</p>

                        <button
                          type="button"
                          disabled={alreadySaved}
                          onClick={() => handleAddGame(game.id)}
                        >
                          {alreadySaved
                            ? "Already saved"
                            : "Add to My Collection"}
                        </button>
                      </li>
                    );
                  })}
                </ul>
              )}
            </section>
          </>
        )}
      </main>
    </ProtectedRoute>
  );
}