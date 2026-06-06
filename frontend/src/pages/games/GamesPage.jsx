import { useEffect, useState } from 'react';
import { getZeldaCatalog } from '../../api/client';
import GameCard from './components/GameCard';
import './GamesPage.css';

// The games page loads the curated Zelda catalog from the backend.
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
    <section className="games-page">
      <div className="games-page__header">
        <p className="games-page__eyebrow">Games</p>
        <h2>Official Legend of Zelda games</h2>
        <p>The list is now grouped in chronological release order from the backend catalog.</p>
      </div>

      {loading ? <p>Loading Zelda catalog...</p> : null}
      {error ? <p role="alert">{error}</p> : null}
      {!loading && !error && games.length > 0 ? (
        <div className="games-page__grid">
          {games.map((game) => (
            <GameCard key={game.slug} game={game} />
          ))}
        </div>
      ) : null}
      {!loading && !error && games.length === 0 ? <p>No Zelda games are currently configured.</p> : null}
    </section>
  );
}
