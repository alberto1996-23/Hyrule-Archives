import { Navigate, Route, Routes } from 'react-router-dom';
import AppLayout from './components/AppLayout';
import RequireAuth from './components/RequireAuth';
import {
  HomeRoutePage,
  DashboardPage,
  LiveActivityPage,
  MyActivityPage,
  MyCollectionPage,
  MyFavoritesPage,
} from './pages/home';
import { AuthCallbackPage, LoginPage } from './pages/login';
import { GameDetailPage, GamesPage } from './pages/games';
import { TrendsPage } from './pages/trends';

// The App component defines the main route map for the testing UI.
export default function App() {
  return (
    <Routes>
      <Route element={<AppLayout />}>
        <Route path="/" element={<HomeRoutePage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/auth/callback" element={<AuthCallbackPage />} />
        <Route path="/games" element={<GamesPage />} />
        <Route path="/games/:gameSlug" element={<GameDetailPage />} />
        <Route path="/games/moby/:mobyGameId" element={<GameDetailPage />} />
        <Route path="/games/local/:localGameId" element={<GameDetailPage />} />
        <Route path="/trends" element={<TrendsPage />} />
        <Route
          path="/dashboard"
          element={
            <RequireAuth>
              <DashboardPage />
            </RequireAuth>
          }
        />
        <Route
          path="/my-collection"
          element={
            <RequireAuth>
              <MyCollectionPage />
            </RequireAuth>
          }
        />
        <Route
          path="/my-favorites"
          element={
            <RequireAuth>
              <MyFavoritesPage />
            </RequireAuth>
          }
        />
        <Route
          path="/my-activity"
          element={
            <RequireAuth>
              <MyActivityPage />
            </RequireAuth>
          }
        />
        <Route
          path="/community/live"
          element={
            <RequireAuth>
              <LiveActivityPage />
            </RequireAuth>
          }
        />
        <Route path="*" element={<Navigate to="/" replace />} />
      </Route>
    </Routes>
  );
}
