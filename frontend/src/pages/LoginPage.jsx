import { useSearchParams } from 'react-router-dom';
import { getLoginUrl } from '../api/client';

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
    <section>
      <h2>Login</h2>
      <p>Please use your Google credentials to create an account and login.</p>
      <p>
        After Google finishes, the backend will send you back to the frontend callback page and the app will
        check the login cookie.
      </p>
      <button type="button" onClick={handleLoginClick}>
        OAuth 2.0 login
      </button>
    </section>
  );
}
