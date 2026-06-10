// I keep the live activity list presentational so the same feed UI can be reused on trends, dashboards, and live pages.
export default function LiveActivityFeed({ title, items, loading, emptyMessage, connectionStatus, headingLevel = 'h2' }) {
  const HeadingTag = headingLevel;

  return (
    <section className="member-page__card member-page__card--activity">
      <div className="member-page__card-header">
        <HeadingTag>{title}</HeadingTag>
        {connectionStatus ? (
          <p className="member-page__connection-status" aria-live="polite">
            Live status: {connectionStatus}
          </p>
        ) : null}
      </div>
      {loading ? <p>Loading activity...</p> : null}
      {!loading && items.length === 0 ? <p>{emptyMessage}</p> : null}
      {!loading && items.length > 0 ? (
        <ul className="member-page__activity-list">
          {items.map((item) => (
            <li key={item.id}>
              <p>{item.message}</p>
              <time dateTime={item.createdAt}>{new Date(item.createdAt).toLocaleString()}</time>
            </li>
          ))}
        </ul>
      ) : null}
    </section>
  );
}
