import { useEffect, useMemo, useState } from 'react';
import { Link, Outlet, useLocation, useNavigate } from 'react-router-dom';
import { getCurrentUser, getLoginUrl, logout } from '../api/client';
import './AppLayout.css';

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

  return (
    <div className="app-shell">
      <header className="app-shell__header">
        <div className="app-shell__header-inner">
          <Link className="app-shell__brand" to="/">
            <img src="/Zelda_the_hyrule_archives_logo.png" alt="The Hyrule Archives logo" />
          </Link>

          <nav aria-label="Primary navigation">
            <ul className="app-shell__nav-list">
              <li>
                <Link className="app-shell__nav-link" to="/games">
                  Games
                </Link>
              </li>
              <li>
                <Link className="app-shell__nav-link" to="/trends">
                  Trends
                </Link>
              </li>
              <li>
                <Link className="app-shell__nav-link" to="/my-collection">
                  My Collection
                </Link>
              </li>
              <li>
                <Link className="app-shell__nav-link" to="/community/live">
                  Live Feed
                </Link>
              </li>
            </ul>
          </nav>

          <section className="app-shell__auth" aria-label="Authentication status">
            {authLoading ? <p>Checking login status...</p> : null}
            {!authLoading && currentUser ? (
              <>
                <p>
                  Logged in as <strong>{currentUser.displayName ?? currentUser.email ?? 'Unknown user'}</strong>
                </p>
                <button type="button" onClick={handleLogout}>
                  Logout
                </button>
              </>
            ) : null}
            {!authLoading && !currentUser ? (
              <>
                <p>You are not currently logged in.</p>
                <a href={getLoginUrl()}>Login with Google</a>
              </>
            ) : null}
          </section>
        </div>
      </header>

      <main className="app-shell__main">
        <Outlet context={appContext} />
      </main>

      <footer className="app-shell__footer">
        <p>Legend of Zelda material is owned by Nintendo. This project is for educational testing only.</p>
      </footer>
    </div>
  );
}
