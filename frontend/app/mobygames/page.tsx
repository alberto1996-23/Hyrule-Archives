"use client";

import { useEffect, useState } from "react";
import { getMobyGamesZeldaData, MobyGame } from "@/lib/api";

export default function MobyGamesPage() {
  const [games, setGames] = useState<MobyGame[]>([]);
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    let shouldIgnore = false;

    async function loadMobyGamesData() {
      try {
        const mobyGames = await getMobyGamesZeldaData();

        if (!shouldIgnore) {
          setGames(mobyGames);
        }
      } catch {
        if (!shouldIgnore) {
          setError("Unable to load MobyGames data.");
        }
      } finally {
        if (!shouldIgnore) {
          setIsLoading(false);
        }
      }
    }

    loadMobyGamesData();

    return () => {
      shouldIgnore = true;
    };
  }, []);

  return (
    <main>
      <h1>MobyGames Zelda Data</h1>

      <p>
        This page displays third-party game metadata from the MobyGames API
        through the ASP.NET Core backend.
      </p>

      <p>
        <strong>Data by MobyGames.com</strong>
      </p>

      {isLoading && <p>Loading MobyGames data...</p>}

      {error && <p>{error}</p>}

      {!isLoading && games.length === 0 && (
        <p>No MobyGames Zelda results were found.</p>
      )}

      <ul>
        {games.map((game) => (
          <li key={game.mobyGameId}>
            <h2>{game.title}</h2>

            <p>
              <strong>MobyGames ID:</strong> {game.mobyGameId}
            </p>

            <p>
              <strong>Moby Score:</strong>{" "}
              {game.mobyScore === null ? "No score listed" : game.mobyScore}
            </p>

            {game.description && <p>{game.description}</p>}

            {game.mobyUrl && (
              <p>
                <a href={game.mobyUrl} target="_blank" rel="noreferrer">
                  View on MobyGames
                </a>
              </p>
            )}

            {game.coverThumbnail && (
              // Using a normal img tag keeps this simple for now.
              // Later, you can switch to Next.js Image if you want.
              // eslint-disable-next-line @next/next/no-img-element
              <img
                src={game.coverThumbnail}
                alt={`Cover thumbnail for ${game.title}`}
                width={160}
              />
            )}

            {!game.coverThumbnail && game.screenshotImage && (
              // eslint-disable-next-line @next/next/no-img-element
              <img
                src={game.screenshotImage}
                alt={`Screenshot for ${game.title}`}
                width={220}
              />
            )}

            <h3>Platforms</h3>

            {game.platforms.length === 0 ? (
              <p>No platform data listed.</p>
            ) : (
              <ul>
                {game.platforms.map((platform) => (
                  <li key={`${game.mobyGameId}-${platform.platformId}`}>
                    {platform.platformName}
                    {platform.firstReleaseDate
                      ? ` — first released ${platform.firstReleaseDate}`
                      : ""}
                  </li>
                ))}
              </ul>
            )}

            <p>
              <em>{game.sourceCredit}</em>
            </p>
          </li>
        ))}
      </ul>
    </main>
  );
}