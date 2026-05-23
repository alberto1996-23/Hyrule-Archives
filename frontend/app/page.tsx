// frontend/app/page.tsx

import Link from "next/link";

export default function HomePage() {
  return (
    <main>
      <h1>The Hyrule Archives</h1>

      <p>
        A community archive for exploring the history, games, platforms, release
        regions, and fan activity of The Legend of Zelda series.
      </p>

      <nav aria-label="Main navigation">
        <ul>
          <li>
            <Link href="/login">Sign in</Link>
          </li>
          <li>
            <Link href="/dashboard">Dashboard protected</Link>
          </li>
          <li>
            <Link href="/my-collection">My Collection protected</Link>
          </li>
          <li>
            <Link href="/activity">Activity protected</Link>
          </li>
        </ul>
      </nav>
    </main>
  );
}