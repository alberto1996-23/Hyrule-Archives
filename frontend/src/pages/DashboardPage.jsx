import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { getMyActivity, getMyFavorites } from '../api/client';

// The dashboard is a protected page that helps test logged-in-only endpoints.
export default function DashboardPage() {
  const [favorites, setFavorites] = useState([]);
  const [activity, setActivity] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    let isMounted = true;

    async function loadDashboard() {
      try {
        const [favoriteItems, myActivity] = await Promise.all([getMyFavorites(), getMyActivity()]);

        if (isMounted) {
          setFavorites(favoriteItems);
          setActivity(myActivity);
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

    loadDashboard();

    return () => {
      isMounted = false;
    };
  }, []);

  return (
    <div>
      <section>
        <h2>Dashboard</h2>
        <p>This page helps test protected routes, favorites, and user-specific activity.</p>
      </section>

      <section>
        <h2>Your favorites</h2>
        {loading ? <p>Loading favorites...</p> : null}
        {error ? <p role="alert">{error}</p> : null}
        {!loading && favorites.length === 0 ? <p>You do not have any favorites yet.</p> : null}
        {!loading && favorites.length > 0 ? (
          <ul>
            {favorites.map((favorite) => (
              <li key={favorite.id}>
                <p>{favorite.gameTitle}</p>
                <p>Status: {favorite.status}</p>
                <p>
                  <Link to="/games">Go back to the games page to reopen the MobyGames source record.</Link>
                </p>
              </li>
            ))}
          </ul>
        ) : null}
      </section>

      <section>
        <h2>Your recent activity</h2>
        {loading ? <p>Loading your activity...</p> : null}
        {!loading && activity.length === 0 ? <p>No personal activity yet.</p> : null}
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
