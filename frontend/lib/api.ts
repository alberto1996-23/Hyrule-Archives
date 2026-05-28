// frontend/lib/api.ts

// This reads the backend URL from .env.local.
// For you, this should be: http://localhost:5154
export const API_BASE_URL =
  process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:5154";

// This type matches the CurrentUserDto from your ASP.NET backend.
export type CurrentUser = {
  id: string;
  email: string | null;
  displayName: string | null;
  isSimulated: boolean;
  createdAt: string;
};

// This function asks the backend:
// "Who is currently logged in?"
//
// The important part is credentials: "include".
// That tells the browser to send the auth cookie to the backend.
export async function getCurrentUser(): Promise<CurrentUser | null> {
  const response = await fetch(`${API_BASE_URL}/api/me`, {
    method: "GET",
    credentials: "include",
  });

  // 401 means the user is not logged in.
  if (response.status === 401) {
    return null;
  }

  // If another error happens, throw an error so the UI can show a message.
  if (!response.ok) {
    throw new Error("Unable to check signed-in user.");
  }

  return response.json();
}

// This sends the user to the backend login endpoint.
// The backend then redirects the user to Google.
export function loginWithGoogle() {
  window.location.href = `${API_BASE_URL}/auth/login`;
}

// This logs out from the backend.
// The backend clears the authentication cookie.
export async function logout() {
  const response = await fetch(`${API_BASE_URL}/auth/logout`, {
    method: "POST",
    credentials: "include",
  });

  if (!response.ok) {
    throw new Error("Unable to log out.");
  }
}

export type GameSummary = {
  id: string;
  title: string;
  slug: string;
  summary: string | null;
  imageUrl: string | null;
  releaseCount: number;
};

export type CollectionItem = {
  id: string;
  gameId: string;
  gameTitle: string;
  gameSlug: string;
  imageUrl: string | null;
  status: string;
  createdAt: string;
};

export async function getGames(): Promise<GameSummary[]> {
  const response = await fetch(`${API_BASE_URL}/api/games`, {
    method: "GET",
    credentials: "include",
  });

  if (!response.ok) {
    throw new Error("Unable to load Zelda games.");
  }

  return response.json();
}

export async function getMyCollection(): Promise<CollectionItem[]> {
  const response = await fetch(`${API_BASE_URL}/api/me/collection`, {
    method: "GET",
    credentials: "include",
  });

  if (response.status === 401) {
    throw new Error("You must be signed in to view your collection.");
  }

  if (!response.ok) {
    throw new Error("Unable to load your collection.");
  }

  return response.json();
}

export async function addGameToMyCollection(gameId: string): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/api/me/collection`, {
    method: "POST",
    credentials: "include",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({ gameId }),
  });

  if (response.status === 409) {
    throw new Error("This game is already in your collection.");
  }

  if (!response.ok) {
    throw new Error("Unable to add this game to your collection.");
  }
}