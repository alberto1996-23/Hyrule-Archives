"use client";

import { useEffect, useState } from "react";
import ProtectedRoute from "@/components/ProtectedRoute";
import {
  addGameToMyCollection,
  CollectionItem,
  deleteCollectionItem,
  GameSummary,
  getGames,
  getMyCollection,
  updateCollectionItemStatus,
} from "@/lib/api";

export default function MyCollectionPage() {
  const [games, setGames] = useState<GameSummary[]>([]);
  const [collection, setCollection] = useState<CollectionItem[]>([]);
  const [message, setMessage] = useState("");
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(true);

  // This reloads both the public game list and the signed-in user's collection.
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

  // CREATE:
  // Adds a game to the signed-in user's collection.
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

  // UPDATE:
  // Changes the status of a saved collection item.
  async function handleStatusChange(collectionItemId: string, status: string) {
    setMessage("");
    setError("");

    try {
      await updateCollectionItemStatus(collectionItemId, status);
      setMessage("Collection item updated.");
      await refreshCollectionPage();
    } catch (err) {
      if (err instanceof Error) {
        setError(err.message);
      } else {
        setError("Something went wrong.");
      }
    }
  }

  // DELETE:
  // Removes a saved game from the signed-in user's collection.
  async function handleDelete(collectionItemId: string) {
    setMessage("");
    setError("");

    try {
      await deleteCollectionItem(collectionItemId);
      setMessage("Game removed from your collection.");
      await refreshCollectionPage();
    } catch (err) {
      if (err instanceof Error) {
        setError(err.message);
      } else {
        setError("Something went wrong.");
      }
    }
  }

  // This helps us disable the "Add" button for games already saved.
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
                      <strong>{item.gameTitle}</strong>

                      <label>
                        Status:
                        <select
                          value={item.status}
                          onChange={(event) =>
                            handleStatusChange(item.id, event.target.value)
                          }
                        >
                          <option value="Saved">Saved</option>
                          <option value="Playing">Playing</option>
                          <option value="Completed">Completed</option>
                          <option value="Dropped">Dropped</option>
                        </select>
                      </label>

                      <button
                        type="button"
                        onClick={() => handleDelete(item.id)}
                      >
                        Remove
                      </button>
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