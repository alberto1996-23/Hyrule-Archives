import { useEffect, useState } from 'react';
import { getCommunityHighlights, getRecentActivity } from '../../api/client';
import TrendSection from './components/TrendSection';
import './TrendsPage.css';

// The trends page shows how user interactions change community leaderboards.
export default function TrendsPage() {
  const [highlights, setHighlights] = useState(null);
  const [activity, setActivity] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    let isMounted = true;

    async function loadTrends() {
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

    loadTrends();

    return () => {
      isMounted = false;
    };
  }, []);

  return (
    <div className="trends-page">
      <section className="trends-page__hero">
        <p className="trends-page__eyebrow">Trends</p>
        <h2>Legend of Zelda community trends</h2>
        <p>
          These lists update based on favorites, comments, likes, and dislikes. Games with no
          community activity stay off the leaderboards, and trending gives the most weight to strong like support while
          dislikes push a game downward.
        </p>
      </section>

      {loading ? <p>Loading community trends...</p> : null}
      {error ? <p role="alert">{error}</p> : null}
      {!loading && highlights ? (
        <div className="trends-page__grid">
          <TrendSection title="Trending games" items={highlights.trendingGames} />
          <TrendSection title="Most favorited games" items={highlights.mostFavoritedGames} />
          <TrendSection title="Most commented games" items={highlights.mostCommentedGames} />
          <TrendSection title="Most liked games" items={highlights.mostLikedGames} />
        </div>
      ) : null}

      <section className="trends-page__card">
        <h3>Recent public activity</h3>
        {!loading && activity.length === 0 ? <p>No public activity yet.</p> : null}
        {!loading && activity.length > 0 ? (
          <ul className="trends-page__activity-list">
            {activity.map((item) => (
              <li key={item.id}>
                <p>{item.message}</p>
                <time dateTime={item.createdAt}>{new Date(item.createdAt).toLocaleString()}</time>
              </li>
            ))}
          </ul>
        ) : null}
      </section>
    </div>
  );
}
