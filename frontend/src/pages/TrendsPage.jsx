import { useEffect, useState } from 'react';
import TrendSection from '../components/TrendSection';
import { getCommunityHighlights, getRecentActivity } from '../api/client';

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
    <div>
      <section>
        <h2>Legend of Zelda Trends</h2>
        <p>
          These lists update based on favorites, comments, likes, and dislikes stored in the backend.
          Games with no community activity stay off the leaderboards, and trending gives the most weight to
          strong like support while dislikes push a game downward.
        </p>
      </section>

      {loading ? <p>Loading community trends...</p> : null}
      {error ? <p role="alert">{error}</p> : null}
      {!loading && highlights ? (
        <div>
          <TrendSection title="Trending games" items={highlights.trendingGames} />
          <TrendSection title="Most favorited games" items={highlights.mostFavoritedGames} />
          <TrendSection title="Most commented games" items={highlights.mostCommentedGames} />
          <TrendSection title="Most liked games" items={highlights.mostLikedGames} />
        </div>
      ) : null}

      <section>
        <h2>Recent public activity</h2>
        {!loading && activity.length === 0 ? <p>No public activity yet.</p> : null}
        {!loading && activity.length > 0 ? (
          <ul>
            {activity.map((item) => (
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
