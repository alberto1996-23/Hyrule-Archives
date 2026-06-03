import { useEffect } from 'react';
import { useLocation, useOutletContext } from 'react-router-dom';

// RequireAuth protects routes that should only render for signed-in users.
export default function RequireAuth({ children }) {
  const { currentUser, authLoading, sendToLogin } = useOutletContext();
  const location = useLocation();

  useEffect(() => {
    if (!authLoading && !currentUser) {
      sendToLogin(location.pathname);
    }
  }, [authLoading, currentUser, location.pathname, sendToLogin]);

  if (authLoading) {
    return <p>Checking whether you are logged in...</p>;
  }

  if (!currentUser) {
    return <p>Redirecting to login...</p>;
  }

  return children;
}
