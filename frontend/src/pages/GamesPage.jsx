import { useEffect, useState } from 'react';
import GameCard from '../components/GameCard';
import { getMobyGamesZelda } from '../api/client';

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

        if (isMounted) {
          setGames(mobyGames);
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
