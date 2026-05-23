// frontend/app/login/page.tsx

"use client";

import { loginWithGoogle } from "@/lib/api";

export default function LoginPage() {
  return (
    <main>
      <h1>Sign in to The Hyrule Archives</h1>

      <p>
        Sign in to create your Zelda collection, favorite games, react to titles,
        and view your activity history.
      </p>

      <button type="button" onClick={loginWithGoogle}>
        Sign in with Google
      </button>
    </main>
  );
}