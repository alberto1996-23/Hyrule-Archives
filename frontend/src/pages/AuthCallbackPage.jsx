import { useEffect } from 'react';
import { useNavigate, useOutletContext } from 'react-router-dom';

// After Google login, the backend redirects here.
// This page waits for the shared layout to detect the authenticated cookie,
// then sends the user to the route they originally wanted.
export default function AuthCallbackPage() {
  const navigate = useNavigate();
  const { currentUser, authLoading } = useOutletContext();

  useEffect(() => {
    if (!authLoading && currentUser) {
      const returnTo = sessionStorage.getItem('hyruleArchives.returnTo') ?? '/dashboard';
      sessionStorage.removeItem('hyruleArchives.returnTo');
      navigate(returnTo, { replace: true });
    }
  }, [authLoading, currentUser, navigate]);

  return (
    <section>
      <h2>Completing login</h2>
      <p>The app is checking your login cookie and will redirect you shortly.</p>
    </section>
  );
}
