"use client";

import { useEffect, useState } from "react";
import ProtectedRoute from "@/components/ProtectedRoute";
import { CurrentUser, getCurrentUser } from "@/lib/api";

export default function ProfilePage() {
  const [user, setUser] = useState<CurrentUser | null>(null);
  const [error, setError] = useState("");

  useEffect(() => {
    let shouldIgnore = false;

    async function loadProfile() {
      try {
        const currentUser = await getCurrentUser();

        if (!shouldIgnore) {
          setUser(currentUser);
        }
      } catch {
        if (!shouldIgnore) {
          setError("Unable to load your profile.");
        }
      }
    }

    loadProfile();

    return () => {
      shouldIgnore = true;
    };
  }, []);

  return (
    <ProtectedRoute>
      <main>
        <h1>My Profile</h1>

        <p>
          This page is protected. It shows information connected to the signed-in
          Google account.
        </p>

        {error && <p>{error}</p>}

        {user ? (
          <section>
            <p>Email: {user.email}</p>
            <p>Display name: {user.displayName}</p>
            <p>Simulated user: {user.isSimulated ? "Yes" : "No"}</p>
            <p>Created at: {user.createdAt}</p>
          </section>
        ) : (
          <p>Loading profile...</p>
        )}
      </main>
    </ProtectedRoute>
  );
}