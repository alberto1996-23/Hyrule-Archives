// frontend/app/my-collection/page.tsx

import ProtectedRoute from "@/components/ProtectedRoute";

export default function MyCollectionPage() {
  return (
    <ProtectedRoute>
      <main>
        <h1>My Legend of Zelda Collection</h1>

        <p>
          This protected page will eventually show the Zelda games this user has
          saved, favorited, liked, or disliked.
        </p>
      </main>
    </ProtectedRoute>
  );
}