import { useEffect, useState } from 'react';
import GameCard from '../components/GameCard';
import { getZeldaCatalog } from '../api/client';

// The games page now loads the curated Zelda catalog from the backend.
export default function GamesPage() {
  const [games, setGames] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    let isMounted = true;

    async function loadCatalog() {
      try {
        const catalog = await getZeldaCatalog();

        if (isMounted) {
          setGames(catalog);
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

    loadCatalog();

    return () => {
      isMounted = false;
    };
  }, []);

  return (
    <section>
      <h2>Official Legend of Zelda games</h2>
      <p>This page now loads the curated Zelda release catalog from the backend.</p>
      {loading ? <p>Loading Zelda catalog...</p> : null}
      {error ? <p role="alert">{error}</p> : null}
      {!loading && !error && games.length > 0 ? (
        <div>
          {games.map((game) => (
            <GameCard key={game.slug} game={game} />
          ))}
        </div>
      ) : null}
      {!loading && !error && games.length === 0 ? <p>No Zelda games are currently configured.</p> : null}
    </section>
  );
}
