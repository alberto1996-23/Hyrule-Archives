"use client";

import Link from "next/link";
import { useEffect, useMemo, useState } from "react";
import SiteHeader from "@/components/SiteHeader";
import { GameDetail, GameSummary, getGameById, getGames } from "@/lib/api";

type SortOption = "title" | "oldest" | "newest";

export default function GamesPage() {
  const [games, setGames] = useState<GameSummary[]>([]);
  const [gameDetails, setGameDetails] = useState<Record<string, GameDetail>>({});
  const [sortOption, setSortOption] = useState<SortOption>("title");
  const [platformFilter, setPlatformFilter] = useState("All");
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    let shouldIgnore = false;

    async function loadGamesPage() {
      try {
        // First, load the public game list.
        const gameData = await getGames();

        if (shouldIgnore) {
          return;
        }

        setGames(gameData);

        // Then load details for each game so this page can show
        // release dates and platforms.
        //
        // This is fine for now because your real Zelda list is small.
        // Later, we can make the backend return release/platform summary
        // directly from GET /api/games to avoid extra requests.
        const details = await Promise.all(
          gameData.map((game) => getGameById(game.id))
        );

        if (shouldIgnore) {
          return;
        }

        const detailMap: Record<string, GameDetail> = {};

        for (const detail of details) {
          detailMap[detail.id] = detail;
        }

        setGameDetails(detailMap);
      } catch {
        if (!shouldIgnore) {
          setError("Unable to load Zelda games.");
        }
      } finally {
        if (!shouldIgnore) {
          setIsLoading(false);
        }
      }
    }

    loadGamesPage();

    return () => {
      shouldIgnore = true;
    };
  }, []);

  const platformOptions = useMemo(() => {
    const platforms = new Set<string>();

    for (const detail of Object.values(gameDetails)) {
      for (const release of detail.releases) {
        platforms.add(release.platformName);
      }
    }

    return ["All", ...Array.from(platforms).sort()];
  }, [gameDetails]);

  const visibleGames = useMemo(() => {
    const copiedGames = [...games];

    const filteredGames = copiedGames.filter((game) => {
      if (platformFilter === "All") {
        return true;
      }

      const detail = gameDetails[game.id];

      if (!detail) {
        return false;
      }

      return detail.releases.some(
        (release) => release.platformName === platformFilter
      );
    });

    filteredGames.sort((firstGame, secondGame) => {
      const firstDetail = gameDetails[firstGame.id];
      const secondDetail = gameDetails[secondGame.id];

      if (sortOption === "title") {
        return firstGame.title.localeCompare(secondGame.title);
      }

      const firstDate = getFirstReleaseDate(firstDetail);
      const secondDate = getFirstReleaseDate(secondDetail);

      if (sortOption === "oldest") {
        return firstDate.getTime() - secondDate.getTime();
      }

      return secondDate.getTime() - firstDate.getTime();
    });

    return filteredGames;
  }, [games, gameDetails, sortOption, platformFilter]);

  return (
    <>
      <SiteHeader />

      <main className="games-page">
        <section className="page-intro">
          <h1>Games</h1>

          <p>
            Browse Legend of Zelda games by release date and platform. Clicking
            a game opens its specific game page.
          </p>
        </section>

        <section className="games-controls" aria-label="Game sorting and filters">
          <label>
            Sort by:
            <select
              value={sortOption}
              onChange={(event) => setSortOption(event.target.value as SortOption)}
            >
              <option value="title">Name</option>
              <option value="oldest">Oldest to Newest</option>
              <option value="newest">Newest to Oldest</option>
            </select>
          </label>

          <label>
            Filter by platform:
            <select
              value={platformFilter}
              onChange={(event) => setPlatformFilter(event.target.value)}
            >
              {platformOptions.map((platform) => (
                <option key={platform} value={platform}>
                  {platform}
                </option>
              ))}
            </select>
          </label>
        </section>

        {isLoading && <p>Loading games...</p>}
        {error && <p className="error-text">{error}</p>}

        {!isLoading && visibleGames.length === 0 && (
          <p>No games match this filter.</p>
        )}

        <section className="games-list" aria-label="Legend of Zelda games">
          {visibleGames.map((game) => {
            const detail = gameDetails[game.id];
            const firstRelease = getFirstRelease(detail);
            const platforms = getPlatformNames(detail);

            return (
              <Link key={game.id} href={`/games/${game.id}`} className="game-row">
                <div className="game-row-art">
                  {game.imageUrl ? (
                    <img src={game.imageUrl} alt={`Box art for ${game.title}`} />
                  ) : (
                    <span>Zelda game box art</span>
                  )}
                </div>

                <div className="game-row-content">
                  <h2>{game.title}</h2>

                  <p>
                    <strong>Release date:</strong>{" "}
                    {firstRelease ? formatDate(firstRelease.releaseDate) : "Unknown"}
                  </p>

                  <p>
                    <strong>Platform:</strong>{" "}
                    {platforms.length > 0 ? platforms.join(", ") : "Unknown"}
                  </p>

                  <p>{game.summary ?? "No summary available."}</p>
                </div>
              </Link>
            );
          })}
        </section>
      </main>
    </>
  );
}

function getFirstRelease(detail: GameDetail | undefined): GameDetail["releases"][number] | null {
  if (!detail || detail.releases.length === 0) {
    return null;
  }

  return [...detail.releases].sort(
    (first, second) =>
      new Date(first.releaseDate).getTime() -
      new Date(second.releaseDate).getTime()
  )[0];
}

function getFirstReleaseDate(detail: GameDetail | undefined): Date {
  const release = getFirstRelease(detail);

  if (!release) {
    return new Date("9999-12-31");
  }

  return new Date(release.releaseDate);
}

function getPlatformNames(detail: GameDetail | undefined): string[] {
  if (!detail) {
    return [];
  }

  const platforms = new Set<string>();

  for (const release of detail.releases) {
    platforms.add(release.platformName);
  }

  return Array.from(platforms).sort();
}

function formatDate(value: string): string {
  return new Date(value).toLocaleDateString();
}