const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5154';

// This helper sends every request with cookies enabled so the React app can work
// with the ASP.NET Core cookie-based login flow.
async function apiFetch(path, options = {}) {
  let response;

  try {
    response = await fetch(`${API_BASE_URL}${path}`, {
      credentials: 'include',
      headers: {
        Accept: 'application/json',
        'Content-Type': 'application/json',
        ...(options.headers ?? {}),
      },
      ...options,
    });
  } catch {
    // Browser-level fetch failures usually mean the backend is down or a redirect/CORS issue occurred
    // before JavaScript could read the response. Give the UI a clearer message than the raw fetch error.
    const error = new Error(
      'Unable to reach the backend. Make sure the ASP.NET server is running and restart it after backend changes.',
    );
    error.status = 0;
    throw error;
  }

  const contentType = response.headers.get('content-type') ?? '';
  const isJson = contentType.includes('application/json');
  const data = isJson ? await response.json() : null;

  if (!response.ok) {
    const error = new Error(data?.detail ?? `Request failed with status ${response.status}`);
    error.status = response.status;
    error.data = data;
    throw error;
  }

  return data;
}

// The frontend uses this URL when the user clicks the login button.
export function getLoginUrl() {
  return `${API_BASE_URL}/auth/login`;
}

export function getApiBaseUrl() {
  return API_BASE_URL;
}

export function getCurrentUser() {
  return apiFetch('/api/me/');
}

export function logout() {
  return apiFetch('/auth/logout', {
    method: 'POST',
  });
}

export function getMobyGamesZelda() {
  return apiFetch('/api/mobygames/zelda');
}

export function importMobyGame(mobyGameId) {
  return apiFetch(`/api/mobygames/import/${mobyGameId}`, {
    method: 'POST',
  });
}

export function getLocalGame(localGameId) {
  return apiFetch(`/api/games/${localGameId}`);
}

export function getGameComments(localGameId) {
  return apiFetch(`/api/games/${localGameId}/comments`);
}

export function createComment(localGameId, body) {
  return apiFetch(`/api/games/${localGameId}/comments`, {
    method: 'POST',
    body: JSON.stringify({ body }),
  });
}

export function getReactionSummary(localGameId) {
  return apiFetch(`/api/games/${localGameId}/reactions/summary`);
}

export function setReaction(localGameId, value) {
  return apiFetch(`/api/games/${localGameId}/reaction`, {
    method: 'PUT',
    body: JSON.stringify({ value }),
  });
}

export function removeReaction(localGameId) {
  return apiFetch(`/api/games/${localGameId}/reaction`, {
    method: 'DELETE',
  });
}

export function getMyCollection() {
  return apiFetch('/api/me/collection/');
}

export function getMyFavorites() {
  return apiFetch('/api/me/collection/favorites');
}

export function addToCollection(localGameId) {
  return apiFetch('/api/me/collection/', {
    method: 'POST',
    body: JSON.stringify({ gameId: localGameId }),
  });
}

export function favoriteCollectionItem(collectionItemId) {
  return apiFetch(`/api/me/collection/${collectionItemId}/favorite`, {
    method: 'PUT',
  });
}

export function unfavoriteCollectionItem(collectionItemId) {
  return apiFetch(`/api/me/collection/${collectionItemId}/favorite`, {
    method: 'DELETE',
  });
}

export function getCommunityHighlights() {
  return apiFetch('/api/community/highlights');
}

export function getRecentActivity() {
  return apiFetch('/api/activity/recent');
}

export function getGameActivity(localGameId) {
  return apiFetch(`/api/activity/games/${localGameId}`);
}

export function getMyActivity() {
  return apiFetch('/api/me/activity/');
}
