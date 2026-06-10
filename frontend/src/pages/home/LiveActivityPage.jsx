import { useEffect, useState } from 'react';
import LiveActivityFeed from '../../components/LiveActivityFeed';
import useLiveActivityFeed from '../../hooks/useLiveActivityFeed';
import { getRecentActivity } from '../../api/client';
import './MemberPages.css';

// I use this page to demonstrate the real-time SignalR feed in a dedicated place that is easy to verify across two browser sessions.
export default function LiveActivityPage() {
  const [initialItems, setInitialItems] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    async function loadActivity() {
      setLoading(true);
      setError('');

      try {
        const items = await getRecentActivity();
        setInitialItems(items);
      } catch (loadError) {
        setError(loadError.message);
      } finally {
        setLoading(false);
      }
    }

    loadActivity();
  }, []);

  const { items, connectionStatus } = useLiveActivityFeed(initialItems, {
    enabled: !loading && !error,
    maxItems: 30,
  });

  return (
    <div className="member-page">
      <section className="member-page__hero">
        <h1>Live community activity</h1>
      </section>

      {error ? <p role="alert">{error}</p> : null}
      <LiveActivityFeed
        title="Public activity stream"
        items={items}
        loading={loading}
        emptyMessage="No public activity has been recorded yet."
        connectionStatus={connectionStatus}
        headingLevel="h2"
      />
    </div>
  );
}
