import { useEffect, useMemo, useState } from 'react';
import { Link, Outlet, useLocation, useNavigate } from 'react-router-dom';
import { getCurrentUser, getLoginUrl, getApiBaseUrl, logout } from '../api/client';

// The layout wraps every page with the shared logo, navigation, and login state controls.
export default function AppLayout() {
  const [currentUser, setCurrentUser] = useState(null);
  const [authLoading, setAuthLoading] = useState(true);
  const [authError, setAuthError] = useState('');
  const location = useLocation();
  const navigate = useNavigate();

  // On first load we ask the backend whether a cookie-authenticated user already exists.
  useEffect(() => {
    let isMounted = true;

    async function loadCurrentUser() {
      if (isMounted) {
        setAuthLoading(true);
      }

      try {
        const user = await getCurrentUser();

        if (isMounted) {
          setCurrentUser(user);
          setAuthError('');
        }
      } catch (error) {
        if (isMounted) {
          if (error.status === 401) {
            setCurrentUser(null);
            setAuthError('');
          } else {
            setAuthError(error.message);
          }
        }
      } finally {
        if (isMounted) {
          setAuthLoading(false);
        }
      }
    }

    loadCurrentUser();

    return () => {
      isMounted = false;
    };
  }, [location.pathname]);

  const appContext = useMemo(
    () => ({
      currentUser,
      authLoading,
      setCurrentUser,
      sendToLogin(returnTo) {
        navigate(`/login?returnTo=${encodeURIComponent(returnTo)}`);
      },
    }),
    [authLoading, currentUser, navigate],
  );

  async function handleLogout() {
    setAuthError('');

    try {
      await logout();
      setCurrentUser(null);
      navigate('/login');
    } catch (error) {
      setAuthError(error.message);
    }
  }

  const homeTarget = currentUser ? '/' : '/login';

  return (
    <div>
      <header>
        <h1>The Hyrule Archives</h1>

        <nav aria-label="Primary navigation">
          <ul>
            <li>
              <Link to={homeTarget}>My Legend of Zelda</Link>
            </li>
            <li>
              <Link to="/games">Games</Link>
            </li>
            <li>
              <Link to="/trends">Trends</Link>
            </li>
            {currentUser ? (
              <li>
                <Link to="/dashboard">Dashboard</Link>
              </li>
            ) : null}
          </ul>
        </nav>

        <section aria-label="Authentication status">
          <h2>Authentication</h2>
          {authLoading ? <p>Checking login status...</p> : null}
          {!authLoading && currentUser ? (
            <div>
              <p>
                Logged in as <strong>{currentUser.displayName ?? currentUser.email ?? 'Unknown user'}</strong>
              </p>
              <button type="button" onClick={handleLogout}>
                Logout
              </button>
            </div>
          ) : null}
          {!authLoading && !currentUser ? (
            <div>
              <p>You are not currently logged in.</p>
              <a href={getLoginUrl()}>Login with Google</a>
            </div>
          ) : null}
          {authError ? <p role="alert">{authError}</p> : null}
        </section>
      </header>

      <main>
        <Outlet context={appContext} />
      </main>

      <footer>
        <p>Legend of Zelda material is owned by Nintendo. This project is for educational testing only.</p>
      </footer>
    </div>
  );
}
