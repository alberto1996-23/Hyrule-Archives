import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { getMyActivity, getMyFavorites } from '../../api/client';
import './DashboardPage.css';

// The dashboard is the signed-in version of the home area.
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
    <div className="dashboard-page">
      <section className="dashboard-page__hero">
        <p className="dashboard-page__eyebrow">Dashboard</p>
        <h2>Your Zelda activity at a glance</h2>
        <p>This version of home is for signed-in users who want to jump back into favorites and recent activity.</p>
      </section>

      <div className="dashboard-page__grid">
        <section className="dashboard-page__card">
          <h3>Your favorites</h3>
          {loading ? <p>Loading favorites...</p> : null}
          {error ? <p role="alert">{error}</p> : null}
          {!loading && favorites.length === 0 ? <p>You do not have any favorites yet.</p> : null}
          {!loading && favorites.length > 0 ? (
            <ul className="dashboard-page__list">
              {favorites.map((favorite) => (
                <li key={favorite.id}>
                  <p className="dashboard-page__title">{favorite.gameTitle}</p>
                  <p>Status: {favorite.status}</p>
                  <Link to="/games">Open the games page</Link>
                </li>
              ))}
            </ul>
          ) : null}
        </section>

        <section className="dashboard-page__card">
          <h3>Your recent activity</h3>
          {loading ? <p>Loading your activity...</p> : null}
          {!loading && activity.length === 0 ? <p>No personal activity yet.</p> : null}
          {!loading && activity.length > 0 ? (
            <ul className="dashboard-page__list">
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
    </div>
  );
}
