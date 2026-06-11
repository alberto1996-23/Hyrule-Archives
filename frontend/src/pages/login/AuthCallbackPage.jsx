import { useEffect, useState } from 'react';
import { useNavigate, useOutletContext } from 'react-router-dom';
import { getCurrentUser } from '../../api/client';

// After Google login, the backend redirects here.
// This page waits for the shared layout to detect the authenticated cookie,
// then sends the user to the route they originally wanted.
export default function AuthCallbackPage() {
  const navigate = useNavigate();
  const { currentUser, authLoading, setCurrentUser } = useOutletContext();
  const [error, setError] = useState('');

  useEffect(() => {
    if (authLoading) {
      return undefined;
    }

    if (!authLoading && currentUser) {
      const returnTo = sessionStorage.getItem('hyruleArchives.returnTo') ?? '/dashboard';
      sessionStorage.removeItem('hyruleArchives.returnTo');
      navigate(returnTo, { replace: true });
      return undefined;
    }

    let cancelled = false;

    async function completeLogin() {
      const returnTo = sessionStorage.getItem('hyruleArchives.returnTo') ?? '/dashboard';

      // Some browsers expose the freshly written auth cookie a moment after the OAuth redirect
      // completes, so retry briefly before declaring the session missing.
      for (const delay of [0, 250, 500, 1000, 1500]) {
        if (delay > 0) {
          await new Promise((resolve) => {
            window.setTimeout(resolve, delay);
          });
        }

        if (cancelled) {
          return;
        }

        try {
          const user = await getCurrentUser();

          if (cancelled) {
            return;
          }

          setCurrentUser(user);
          sessionStorage.removeItem('hyruleArchives.returnTo');
          navigate(returnTo, { replace: true });
          return;
        } catch (loadError) {
          if (loadError.status === 401) {
            continue;
          }

          if (!cancelled) {
            setError(loadError.message);
          }

          return;
        }
      }

      if (!cancelled) {
        setError('Google sign-in completed, but your app session was not detected. Reload once or try signing in again.');
      }
    }

    completeLogin();

    return () => {
      cancelled = true;
    };
  }, [authLoading, currentUser, navigate, setCurrentUser]);

  return (
    <section className="login-page">
      <div className="login-page__card">
        <p className="login-page__eyebrow">Login</p>
        <h2>Completing login</h2>
        {error ? <p role="alert">{error}</p> : null}
      </div>
    </section>
  );
}
