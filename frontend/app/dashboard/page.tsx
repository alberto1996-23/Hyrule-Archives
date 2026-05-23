// frontend/app/dashboard/page.tsx

import ProtectedRoute from "@/components/ProtectedRoute";

export default function DashboardPage() {
  return (
    <ProtectedRoute>
      <main>
        <h1>Dashboard</h1>

        <p>
          Welcome to your Hyrule Archives dashboard. This page is protected, so
          only signed-in users should be able to see it.
        </p>
      </main>
    </ProtectedRoute>
  );
}