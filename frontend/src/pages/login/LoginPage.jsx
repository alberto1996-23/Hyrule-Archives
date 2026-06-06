import { useSearchParams } from 'react-router-dom';
import { getLoginUrl } from '../../api/client';
import './LoginPage.css';

// The login page explains the cookie-based Google sign-in flow.
export default function LoginPage() {
  const [searchParams] = useSearchParams();
  const returnTo = searchParams.get('returnTo') ?? '/dashboard';

  function handleLoginClick() {
    // Store the intended destination so the callback page can send the user back there.
    sessionStorage.setItem('hyruleArchives.returnTo', returnTo);
    window.location.assign(getLoginUrl());
  }

  return (
    <section className="login-page">
      <div className="login-page__card">
        <p className="login-page__eyebrow">Login</p>
        <h2>Sign in with Google</h2>
        <p>
          Use your Google account to create a session, save collection activity, add reactions, and leave comments.
        </p>
        <button className="login-page__button" type="button" onClick={handleLoginClick}>
          OAuth 2.0 login
        </button>
      </div>
    </section>
  );
}
