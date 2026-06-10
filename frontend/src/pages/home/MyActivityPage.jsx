import { useEffect, useState } from 'react';
import LiveActivityFeed from '../../components/LiveActivityFeed';
import { getMyActivity } from '../../api/client';
import './MemberPages.css';

// I use this page for a clean, private timeline of only my own Zelda activity.
export default function MyActivityPage() {
  const [activityItems, setActivityItems] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    async function loadActivity() {
      setLoading(true);
      setError('');

      try {
        const items = await getMyActivity();
        setActivityItems(items);
      } catch (loadError) {
        setError(loadError.message);
      } finally {
        setLoading(false);
      }
    }

    loadActivity();
  }, []);

  return (
    <div className="member-page">
      <section className="member-page__hero">
        <h1>Activity timeline</h1>
      </section>

      {error ? <p role="alert">{error}</p> : null}
      <LiveActivityFeed
        title="Recent private activity"
        items={activityItems}
        loading={loading}
        emptyMessage="You have not generated any private activity yet."
        headingLevel="h2"
      />
    </div>
  );
}
