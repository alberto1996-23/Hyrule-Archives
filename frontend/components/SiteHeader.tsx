"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { CurrentUser, getCurrentUser } from "@/lib/api";

export default function SiteHeader() {
  const [user, setUser] = useState<CurrentUser | null>(null);

  useEffect(() => {
    let shouldIgnore = false;

    async function checkUser() {
      try {
        // This asks the backend if someone is currently signed in.
        // If /api/me returns 200, we get a user.
        // If /api/me returns 401, getCurrentUser returns null.
        const currentUser = await getCurrentUser();

        if (!shouldIgnore) {
          setUser(currentUser);
        }
      } catch {
        if (!shouldIgnore) {
          setUser(null);
        }
      }
    }

    checkUser();

    return () => {
      shouldIgnore = true;
    };
  }, []);

  // If logged in, "My Legend of Zelda" goes to dashboard.
  // If not logged in, it sends the user to the login page.
  const myZeldaHref = user ? "/dashboard" : "/login";

  return (
    <header className="site-header">
      <Link href="/" className="site-logo">
        The Hyrule Archives
      </Link>

      <nav className="site-nav" aria-label="Main navigation">
        <Link href={myZeldaHref}>My Legend of Zelda</Link>
        <Link href="/games">Games</Link>

        {user && <Link href="/my-collection">Favorites</Link>}

        <Link href="/trends">Trends</Link>
        <Link href="/mobygames">Encyclopedia</Link>
      </nav>
    </header>
  );
}