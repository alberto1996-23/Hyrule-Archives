import { useEffect, useState } from 'react';
import GameCard from '../components/GameCard';
import { getMobyGamesZelda } from '../api/client';

function getEarliestReleaseTimestamp(game) {
  // MobyGames gives release hints per platform, and some values are partial dates like
  // 1986 or 1998-11. This helper normalizes those values so the list can be sorted from
  // the oldest Zelda release to the newest one.
  const timestamps = (game.platforms ?? [])
    .map((platform) => normalizeReleaseDate(platform.firstReleaseDate))
    .filter((value) => value !== null);

  return timestamps.length > 0 ? Math.min(...timestamps) : Number.MAX_SAFE_INTEGER;
}

function normalizeReleaseDate(value) {
  if (!value || typeof value !== 'string') {
    return null;
  }

  const trimmed = value.trim();

  if (/^\d{4}$/.test(trimmed)) {
    return Date.parse(`${trimmed}-01-01T00:00:00Z`);
  }

  if (/^\d{4}-\d{2}$/.test(trimmed)) {
    return Date.parse(`${trimmed}-01T00:00:00Z`);
  }

  const parsed = Date.parse(trimmed);
  return Number.isNaN(parsed) ? null : parsed;
}

// The games page shows Zelda titles directly from MobyGames.
export default function GamesPage() {
  const [games, setGames] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    let isMounted = true;

    async function loadGames() {
      try {
        const mobyGames = await getMobyGamesZelda();

        // Sort the returned Zelda titles into chronological order so the page reads like
        // a series timeline, starting with the earliest release and moving toward today.
        const chronologicallySortedGames = [...mobyGames].sort((leftGame, rightGame) => {
          const leftTimestamp = getEarliestReleaseTimestamp(leftGame);
          const rightTimestamp = getEarliestReleaseTimestamp(rightGame);

          if (leftTimestamp !== rightTimestamp) {
            return leftTimestamp - rightTimestamp;
          }

          return leftGame.title.localeCompare(rightGame.title);
        });

        if (isMounted) {
          setGames(chronologicallySortedGames);
        }
      } catch (loadError) {
        if (isMounted) {
          setError(loadError.message);
        }
      } finally {
        if (isMounted) {
          setLoading(false);
        }
      }
    }

    loadGames();

    return () => {
      isMounted = false;
    };
  }, []);

  return (
    <section>
      <h2>All Legend of Zelda titles from MobyGames</h2>
      <p>This list is ordered by earliest release date, from the original 1986 game through newer entries.</p>
      {loading ? <p>Loading Zelda titles...</p> : null}
      {error ? <p role="alert">{error}</p> : null}
      {!loading && games.length === 0 ? <p>No Zelda titles were returned by MobyGames.</p> : null}
      {!loading && games.length > 0 ? (
        <div>
          {games.map((game) => (
            <GameCard key={game.mobyGameId} game={game} />
          ))}
        </div>
      ) : null}
    </section>
  );
}
