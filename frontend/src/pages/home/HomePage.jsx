import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { getCommunityHighlights, getRecentActivity } from '../../api/client';
import './HomePage.css';

// The public homepage gives visitors a landing page before they sign in.
export default function HomePage() {
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

  const topFavoritedGames = (highlights?.mostFavoritedGames ?? []).filter((game) => game.favoriteCount > 0);

  return (
    <div className="home-page">
      <section className="home-page__hero">
        <h2 className="home-page__eyebrow">Welcome to The Hyrule Archives</h2>
        <p>
          Track Zelda favorites, reactions, comments, and community trends in one place.
          Browse the curated Zelda catalog, check what the community is talking about, and sign in when you are
          ready to save your own collection activity.
        </p>
        <div className="home-page__actions">
          <Link className="home-page__primary-link" to="/login">
            Login
          </Link>
          <Link className="home-page__secondary-link" to="/games">
            View games
          </Link>
          <Link className="home-page__secondary-link" to="/trends">
            View trends
          </Link>
        </div>
      </section>

      <div className="home-page__grid">
        <section className="home-page__card">
          <h3>Top favorited Zelda games</h3>
          {loading ? <p>Loading top games...</p> : null}
          {error ? <p role="alert">{error}</p> : null}
          {!loading && !error && topFavoritedGames.length === 0 ? <p>No games have been favorited yet.</p> : null}
          {!loading && topFavoritedGames.length > 0 ? (
            <ol className="home-page__list">
              {topFavoritedGames.slice(0, 5).map((game) => (
                <li key={game.gameId}>{game.title}</li>
              ))}
            </ol>
          ) : null}
        </section>

        <section className="home-page__card">
          <h3>Recent public activity</h3>
          {loading ? <p>Loading community activity...</p> : null}
          {!loading && activity.length === 0 ? <p>No recent public activity yet.</p> : null}
          {!loading && activity.length > 0 ? (
            <ul className="home-page__activity-list">
              {activity.slice(0, 8).map((item) => (
                <li key={item.id}>
                  <p>{item.message}</p>
                  <time dateTime={item.createdAt}>{new Date(item.createdAt).toLocaleString()}</time>
                </li>
              ))}
            </ul>
          ) : null}
        </section>
      </div>
    </div>
  );
}
