import SiteHeader from "@/components/SiteHeader";

export default function TrendsPage() {
  return (
    <>
      <SiteHeader />

      <main className="dashboard-page">
        <h1>Trends</h1>

        <p>
          This page will show community trends based on favorites, collection
          statuses, comments, likes, and dislikes.
        </p>

        <section className="dashboard-card">
          <h2>Coming Soon</h2>

          <ul>
            <li>Most favorited Zelda games</li>
            <li>Most completed Zelda games</li>
            <li>Most discussed games</li>
            <li>Popular platforms</li>
          </ul>
        </section>
      </main>
    </>
  );
}