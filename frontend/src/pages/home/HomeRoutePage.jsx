import { useOutletContext } from 'react-router-dom';
import DashboardPage from './DashboardPage';
import HomePage from './HomePage';

// The root home route swaps between the public landing page and the signed-in dashboard.
export default function HomeRoutePage() {
  const { currentUser, authLoading } = useOutletContext();

  if (authLoading) {
    return <p>Checking login status...</p>;
  }

  return currentUser ? <DashboardPage /> : <HomePage />;
}
