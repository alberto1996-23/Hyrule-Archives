const LOCAL_BACKEND_BY_PROTOCOL = {
  'http:': 'http://localhost:5154',
  'https:': 'https://localhost:7244',
};

function resolveApiBaseUrl() {
  const configuredApiBaseUrl = import.meta.env.VITE_API_BASE_URL?.trim();

  if (typeof window === 'undefined') {
    return configuredApiBaseUrl || LOCAL_BACKEND_BY_PROTOCOL['http:'];
  }

  const isLocalhost = ['localhost', '127.0.0.1'].includes(window.location.hostname);

  if (!isLocalhost) {
    return configuredApiBaseUrl || LOCAL_BACKEND_BY_PROTOCOL['http:'];
  }

  // For localhost development the frontend must talk to the backend over the same scheme
  // the page was loaded with. That prevents Safari/Chrome from treating the auth cookie as
  // cross-site simply because one tab used http://localhost and another used https://localhost.
  const protocolMatchedLocalBaseUrl =
    LOCAL_BACKEND_BY_PROTOCOL[window.location.protocol] ?? LOCAL_BACKEND_BY_PROTOCOL['http:'];

  if (!configuredApiBaseUrl) {
    return protocolMatchedLocalBaseUrl;
  }

  try {
    const configuredUrl = new URL(configuredApiBaseUrl);
    const usesLocalhost = ['localhost', '127.0.0.1'].includes(configuredUrl.hostname);

    if (usesLocalhost && configuredUrl.protocol !== window.location.protocol) {
      return protocolMatchedLocalBaseUrl;
    }

    return configuredApiBaseUrl;
  } catch {
    return configuredApiBaseUrl;
  }
}

const API_BASE_URL = resolveApiBaseUrl();

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
  if (typeof window === 'undefined') {
    return `${API_BASE_URL}/auth/login`;
  }

  // Send the active frontend origin to the backend so the OAuth redirect lands back on the
  // same localhost scheme and browser tab family that initiated login.
  const loginUrl = new URL(`${API_BASE_URL}/auth/login`);
  loginUrl.searchParams.set('frontendOrigin', window.location.origin);
  return loginUrl.toString();
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

export function getZeldaCatalog() {
  return apiFetch('/api/catalog/zelda');
}

export function getZeldaCatalogGameBySlug(slug) {
  return apiFetch(`/api/catalog/zelda/${slug}`);
}

export function getZeldaCatalogGameByMobyGameId(mobyGameId) {
  return apiFetch(`/api/catalog/zelda/by-mobygames-id/${mobyGameId}`);
}

export function importMobyGame(mobyGameId) {
  return apiFetch(`/api/mobygames/import/${mobyGameId}`, {
    method: 'POST',
  });
}

export function getLocalGame(localGameId) {
  return apiFetch(`/api/games/${localGameId}`);
}

export function getLocalGameBySlug(slug) {
  return apiFetch(`/api/games/by-slug/${slug}`);
}

export function getLocalGameByMobyGameId(mobyGameId) {
  return apiFetch(`/api/games/by-mobygames-id/${mobyGameId}`);
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

export function updateComment(commentId, body) {
  return apiFetch(`/api/comments/${commentId}`, {
    method: 'PUT',
    body: JSON.stringify({ body }),
  });
}

export function deleteComment(commentId) {
  return apiFetch(`/api/comments/${commentId}`, {
    method: 'DELETE',
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

export function updateCollectionItem(collectionItemId, status) {
  return apiFetch(`/api/me/collection/${collectionItemId}`, {
    method: 'PUT',
    body: JSON.stringify({ status }),
  });
}

export function deleteCollectionItem(collectionItemId) {
  return apiFetch(`/api/me/collection/${collectionItemId}`, {
    method: 'DELETE',
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
