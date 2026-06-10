import { render, screen, waitFor } from '@testing-library/react';
import { MemoryRouter, Outlet, Route, Routes } from 'react-router-dom';
import RequireAuth from '../RequireAuth';

function ContextLayout({ context }) {
  return <Outlet context={context} />;
}

describe('RequireAuth', () => {
  it('shows a loading message while auth is still being resolved', () => {
    render(
      <MemoryRouter initialEntries={['/private']}>
        <Routes>
          <Route element={<ContextLayout context={{ authLoading: true, currentUser: null, sendToLogin: vi.fn() }} />}>
            <Route
              path="/private"
              element={
                <RequireAuth>
                  <p>Secret content</p>
                </RequireAuth>
              }
            />
          </Route>
        </Routes>
      </MemoryRouter>,
    );

    expect(screen.getByText('Checking whether you are logged in...')).toBeInTheDocument();
  });

  it('redirects unauthenticated users to login with the current path', async () => {
    const sendToLogin = vi.fn();

    render(
      <MemoryRouter initialEntries={['/private']}>
        <Routes>
          <Route element={<ContextLayout context={{ authLoading: false, currentUser: null, sendToLogin }} />}>
            <Route
              path="/private"
              element={
                <RequireAuth>
                  <p>Secret content</p>
                </RequireAuth>
              }
            />
          </Route>
        </Routes>
      </MemoryRouter>,
    );

    expect(screen.getByText('Redirecting to login...')).toBeInTheDocument();
    await waitFor(() => expect(sendToLogin).toHaveBeenCalledWith('/private'));
  });

  it('renders children for authenticated users', () => {
    render(
      <MemoryRouter initialEntries={['/private']}>
        <Routes>
          <Route
            element={<ContextLayout context={{ authLoading: false, currentUser: { id: 'user-1' }, sendToLogin: vi.fn() }} />}
          >
            <Route
              path="/private"
              element={
                <RequireAuth>
                  <p>Secret content</p>
                </RequireAuth>
              }
            />
          </Route>
        </Routes>
      </MemoryRouter>,
    );

    expect(screen.getByText('Secret content')).toBeInTheDocument();
  });
});
