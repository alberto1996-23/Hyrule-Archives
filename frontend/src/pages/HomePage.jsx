import { useEffect, useState } from 'react';
import { Link, useOutletContext } from 'react-router-dom';
import { getCommunityHighlights, getRecentActivity } from '../api/client';

// The home page gives you one place to quickly confirm public data is loading.
export default function HomePage() {
  const { currentUser } = useOutletContext();
  const [highlights, setHighlights] = useState(null);
  const [activity, setActivity] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    let isMounted = true;

    async function loadHomePageData() {
      try {
        const [communityHighlights, recentActivity] = await Promise.all([
          getCommunityHighlights(),
          getRecentActivity(),
        ]);

        if (isMounted) {
          setHighlights(communityHighlights);
          setActivity(recentActivity);
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

    loadHomePageData();

    return () => {
      isMounted = false;
    };
  }, []);

  // The homepage spotlight should only show games that users have actually favorited.
  // For now the list is title-only so it behaves like a simple preview of the future box-art carousel.
  const topFavoritedGames = (highlights?.mostFavoritedGames ?? []).filter((game) => game.favoriteCount > 0);

  return (
    <div>
      <section>
        <h2>Home Page</h2>
        <p>
          Use this page to test the public preview flow before adding styling. The main testing pages are
          <Link to="/games"> Games</Link>, <Link to="/trends">Trends</Link>, and{' '}
          {currentUser ? <Link to="/dashboard">Dashboard</Link> : <Link to="/login">Login</Link>}.
        </p>
      </section>

      <section>
        <h2>Top 5 Zelda Games</h2>
        {loading ? <p>Loading top games...</p> : null}
        {error ? <p role="alert">{error}</p> : null}
        {!loading && !error && topFavoritedGames.length === 0 ? <p>No games have been favorited yet.</p> : null}
        {!loading && highlights ? (
          <ol>
            {topFavoritedGames.map((game) => (
              <li key={game.gameId}>
                <p>{game.title}</p>
              </li>
            ))}
          </ol>
        ) : null}
      </section>

      <section>
        <h2>Recent comments, likes, and dislikes</h2>
        {loading ? <p>Loading community activity...</p> : null}
        {!loading && activity.length === 0 ? <p>No recent public activity yet.</p> : null}
        {!loading && activity.length > 0 ? (
          <ul>
            {activity.slice(0, 10).map((item) => (
              <li key={item.id}>
                <p>{item.message}</p>
                <p>{new Date(item.createdAt).toLocaleString()}</p>
              </li>
            ))}
          </ul>
        ) : null}
      </section>
    </div>
  );
}
