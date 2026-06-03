import { Navigate, Route, Routes } from 'react-router-dom';
import AppLayout from './components/AppLayout';
import RequireAuth from './components/RequireAuth';
import AuthCallbackPage from './pages/AuthCallbackPage';
import DashboardPage from './pages/DashboardPage';
import GameDetailPage from './pages/GameDetailPage';
import GamesPage from './pages/GamesPage';
import HomePage from './pages/HomePage';
import LoginPage from './pages/LoginPage';
import TrendsPage from './pages/TrendsPage';

// The App component defines the main route map for the testing UI.
export default function App() {
  return (
    <Routes>
      <Route element={<AppLayout />}>
        <Route
          path="/"
          element={
            <RequireAuth>
              <HomePage />
            </RequireAuth>
          }
        />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/auth/callback" element={<AuthCallbackPage />} />
        <Route
          path="/games"
          element={
            <RequireAuth>
              <GamesPage />
            </RequireAuth>
          }
        />
        <Route
          path="/games/moby/:mobyGameId"
          element={
            <RequireAuth>
              <GameDetailPage />
            </RequireAuth>
          }
        />
        <Route
          path="/trends"
          element={
            <RequireAuth>
              <TrendsPage />
            </RequireAuth>
          }
        />
        <Route
          path="/dashboard"
          element={
            <RequireAuth>
              <DashboardPage />
            </RequireAuth>
          }
        />
        <Route path="*" element={<Navigate to="/login" replace />} />
      </Route>
    </Routes>
  );
}
