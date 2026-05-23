// frontend/app/activity/page.tsx

import ProtectedRoute from "@/components/ProtectedRoute";

export default function ActivityPage() {
  return (
    <ProtectedRoute>
      <main>
        <h1>My Activity History</h1>

        <p>
          This protected page will eventually show this user&apos;s comments,
          favorites, reactions, and other activity across The Hyrule Archives.
        </p>
      </main>
    </ProtectedRoute>
  );
}