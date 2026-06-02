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

    // Important:
    // This sends the login cookie to the backend.
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

// Updates the status of a saved game in the user's collection.
// Example status values: "Saved", "Playing", "Completed", "Dropped"
export async function updateCollectionItemStatus(
  collectionItemId: string,
  status: string
): Promise<void> {
  const response = await fetch(
    `${API_BASE_URL}/api/me/collection/${collectionItemId}`,
    {
      method: "PUT",
      credentials: "include",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ status }),
    }
  );

  if (!response.ok) {
    throw new Error("Unable to update this collection item.");
  }
}

// Removes a saved game from the user's collection.
export async function deleteCollectionItem(
  collectionItemId: string
): Promise<void> {
  const response = await fetch(
    `${API_BASE_URL}/api/me/collection/${collectionItemId}`,
    {
      method: "DELETE",
      credentials: "include",
    }
  );

  if (!response.ok) {
    throw new Error("Unable to remove this game from your collection.");
  }
}

// This type matches MobyPlatformDto from the backend.
export type MobyPlatform = {
  platformId: number;
  platformName: string;
  firstReleaseDate: string | null;
};

// This type matches MobyGameDto from the backend.
export type MobyGame = {
  mobyGameId: number;
  title: string;
  description: string | null;
  mobyScore: number | null;
  mobyUrl: string | null;
  coverImage: string | null;
  coverThumbnail: string | null;
  screenshotImage: string | null;
  platforms: MobyPlatform[];
  sourceCredit: string;
};

// This calls our ASP.NET backend.
// The backend calls MobyGames, so the API key stays protected.
export async function getMobyGamesZeldaData(): Promise<MobyGame[]> {
  const response = await fetch(`${API_BASE_URL}/api/mobygames/zelda`, {
    method: "GET",
  });

  if (!response.ok) {
    throw new Error("Unable to load MobyGames Zelda data.");
  }

  return response.json();
}

// This type matches GameReleaseDto from the backend.
// A game can have many release records because it may release on
// different platforms or in different regions.
export type GameRelease = {
  id: string;
  platformName: string;
  regionName: string;
  regionCode: string;
  releaseDate: string;
  source: string;
};

// This type matches GameDetailDto from the backend.
// This is used on the specific game page.
export type GameDetail = {
  id: string;
  title: string;
  slug: string;
  summary: string | null;
  imageUrl: string | null;
  releases: GameRelease[];
};

// This gets one specific game from the backend by ID.
// We use this on /games/[id].
export async function getGameById(gameId: string): Promise<GameDetail> {
  const response = await fetch(`${API_BASE_URL}/api/games/${gameId}`, {
    method: "GET",
    credentials: "include",
  });

  if (!response.ok) {
    throw new Error("Unable to load this game.");
  }

  return response.json();
}