import { useEffect, useMemo, useState } from 'react';
import { useNavigate, useOutletContext, useParams } from 'react-router-dom';
import CommentsSection from '../components/CommentsSection';
import {
  addToCollection,
  createComment,
  favoriteCollectionItem,
  getGameActivity,
  getGameComments,
  getLocalGame,
  getMyCollection,
  importMobyGame,
  removeReaction,
  setReaction,
  unfavoriteCollectionItem,
  getReactionSummary,
} from '../api/client';

// The game detail page is where the full end-to-end flow happens.
// It imports a MobyGames record into the local database, then loads the local game data
// so comments, reactions, favorites, and activity all work against the same local Guid.
export default function GameDetailPage() {
  const { mobyGameId } = useParams();
  const navigate = useNavigate();
  const { currentUser, authLoading, sendToLogin } = useOutletContext();

  const [importResult, setImportResult] = useState(null);
  const [localGame, setLocalGame] = useState(null);
  const [reactionSummary, setReactionSummary] = useState(null);
  const [comments, setComments] = useState([]);
  const [gameActivity, setGameActivity] = useState([]);
  const [collectionItem, setCollectionItem] = useState(null);
  const [loading, setLoading] = useState(true);
  const [pageError, setPageError] = useState('');
  const [commentDraft, setCommentDraft] = useState('');
  const [commentSubmitting, setCommentSubmitting] = useState(false);
  const [commentError, setCommentError] = useState('');
  const [reactionSubmitting, setReactionSubmitting] = useState(false);
  const [favoriteSubmitting, setFavoriteSubmitting] = useState(false);
  const [actionMessage, setActionMessage] = useState('');

  const returnPath = useMemo(() => `/games/moby/${mobyGameId}`, [mobyGameId]);

  useEffect(() => {
    let isMounted = true;

    async function loadGamePage() {
      setLoading(true);
      setPageError('');

      try {
        const imported = await importMobyGame(mobyGameId);

        if (!isMounted) {
          return;
        }

        setImportResult(imported);
        await hydrateGameState(imported.localGameId, isMounted);
      } catch (loadError) {
        if (isMounted) {
          setPageError(loadError.message);
        }
      } finally {
        if (isMounted) {
          setLoading(false);
        }
      }
    }

    loadGamePage();

    return () => {
      isMounted = false;
    };
  }, [mobyGameId, currentUser]);

  async function hydrateGameState(localGameId, isMountedFlag = true) {
    const [game, summary, commentItems, activityItems] = await Promise.all([
      getLocalGame(localGameId),
      getReactionSummary(localGameId),
      getGameComments(localGameId),
      getGameActivity(localGameId),
    ]);

    if (!isMountedFlag) {
      return;
    }

    setLocalGame(game);
    setReactionSummary(summary);
    setComments(commentItems);
    setGameActivity(activityItems);

    // Collection state only exists for authenticated users, so skip it for public visitors.
    if (currentUser) {
      try {
        const myCollection = await getMyCollection();
        const existingCollectionItem = myCollection.find((item) => item.gameId === localGameId) ?? null;
        setCollectionItem(existingCollectionItem);
      } catch (collectionError) {
        if (collectionError.status === 401) {
          setCollectionItem(null);
        } else {
          throw collectionError;
        }
      }
    } else {
      setCollectionItem(null);
    }
  }

  function goToLogin() {
    sendToLogin(returnPath);
  }

  async function handleReaction(value) {
    if (!currentUser) {
      goToLogin();
      return;
    }

    if (!localGame) {
      return;
    }

    setReactionSubmitting(true);
    setActionMessage('');
    setCommentError('');

    try {
      await setReaction(localGame.id, value);
      setActionMessage(`Saved a ${value.toLowerCase()} reaction.`);
      await hydrateGameState(localGame.id);
    } catch (error) {
      if (error.status === 401) {
        goToLogin();
      } else {
        setCommentError(error.message);
      }
    } finally {
      setReactionSubmitting(false);
    }
  }

  async function handleRemoveReaction() {
    if (!currentUser) {
      goToLogin();
      return;
    }

    if (!localGame) {
      return;
    }

    setReactionSubmitting(true);
    setActionMessage('');

    try {
      await removeReaction(localGame.id);
      setActionMessage('Removed your reaction.');
      await hydrateGameState(localGame.id);
    } catch (error) {
      if (error.status === 401) {
        goToLogin();
      } else {
        setCommentError(error.message);
      }
    } finally {
      setReactionSubmitting(false);
    }
  }

  async function handleFavoriteToggle() {
    if (!currentUser) {
      goToLogin();
      return;
    }

    if (!localGame) {
      return;
    }

    setFavoriteSubmitting(true);
    setActionMessage('');

    try {
      let workingCollectionItem = collectionItem;

      // Favorites are stored on collection items, so the game must exist in the user's collection first.
      if (!workingCollectionItem) {
        const createdItem = await addToCollection(localGame.id);
        workingCollectionItem = createdItem;
      }

      if (workingCollectionItem.isFavorite) {
        await unfavoriteCollectionItem(workingCollectionItem.id);
        setActionMessage('Removed this game from your favorites.');
      } else {
        await favoriteCollectionItem(workingCollectionItem.id);
        setActionMessage('Added this game to your favorites.');
      }

      await hydrateGameState(localGame.id);
    } catch (error) {
      if (error.status === 401) {
        goToLogin();
      } else {
        setCommentError(error.message);
      }
    } finally {
      setFavoriteSubmitting(false);
    }
  }

  async function handleCommentSubmit(event) {
    event.preventDefault();

    if (!currentUser) {
      goToLogin();
      return;
    }

    if (!localGame || commentDraft.trim().length === 0) {
      return;
    }

    setCommentSubmitting(true);
    setCommentError('');
    setActionMessage('');

    try {
      await createComment(localGame.id, commentDraft.trim());
      setCommentDraft('');
      setActionMessage('Comment posted successfully.');
      await hydrateGameState(localGame.id);
    } catch (error) {
      if (error.status === 401) {
        goToLogin();
      } else {
        setCommentError(error.message);
      }
    } finally {
      setCommentSubmitting(false);
    }
  }

  return (
    <div>
      <section>
        <h2>Specific Zelda Game Page</h2>
      </section>

      {loading ? <p>Importing and loading this Zelda game...</p> : null}
      {pageError ? <p role="alert">{pageError}</p> : null}

      {!loading && importResult ? (
        <section>
          <h3>Import status</h3>
          <p>Local game ID: {importResult.localGameId}</p>
          <p>Was created now: {importResult.wasCreated ? 'Yes' : 'No, it already existed'}</p>
          <p>Releases imported: {importResult.releasesImported}</p>
          <p>Platforms seen from MobyGames: {importResult.platformsSeen}</p>
        </section>
      ) : null}

      {!loading && localGame ? (
        <div>
          <section>
            <h2>{localGame.title}</h2>
            <p>{localGame.summary ?? 'No summary is available yet.'}</p>
            <p>Slug: {localGame.slug}</p>
            <p>Image URL: {localGame.imageUrl ?? 'No image URL returned'}</p>
          </section>

          <section>
            <h2>Release information</h2>
            {localGame.releases.length === 0 ? <p>No release rows were imported.</p> : null}
            {localGame.releases.length > 0 ? (
              <ul>
                {localGame.releases.map((release) => (
                  <li key={release.id}>
                    <p>Platform: {release.platformName}</p>
                    <p>Region: {release.regionName}</p>
                    <p>Release date: {new Date(release.releaseDate).toLocaleDateString()}</p>
                    <p>Source: {release.source}</p>
                  </li>
                ))}
              </ul>
            ) : null}
          </section>

          <section>
            <h2>Likes and dislikes of this game</h2>
            <p>Likes: {reactionSummary?.likes ?? 0}</p>
            <p>Dislikes: {reactionSummary?.dislikes ?? 0}</p>
            <div>
              <button type="button" onClick={() => handleReaction('Like')} disabled={reactionSubmitting}>
                Like
              </button>
              <button type="button" onClick={() => handleReaction('Dislike')} disabled={reactionSubmitting}>
                Dislike
              </button>
              <button type="button" onClick={handleRemoveReaction} disabled={reactionSubmitting}>
                Remove reaction
              </button>
            </div>
          </section>

          <section>
            <h2>Favorite this game</h2>
            <p>
              Current favorite status:{' '}
              {collectionItem?.isFavorite ? 'This game is in your favorites.' : 'This game is not in your favorites yet.'}
            </p>
            <button type="button" onClick={handleFavoriteToggle} disabled={favoriteSubmitting || authLoading}>
              {collectionItem?.isFavorite ? 'Remove from favorites' : 'Add to favorites'}
            </button>
          </section>

          {actionMessage ? <p>{actionMessage}</p> : null}

          <CommentsSection
            currentUser={currentUser}
            commentDraft={commentDraft}
            comments={comments}
            commentsLoading={loading}
            commentSubmitting={commentSubmitting}
            commentError={commentError}
            onDraftChange={setCommentDraft}
            onSubmitComment={handleCommentSubmit}
            onGoToLogin={goToLogin}
          />

          <section>
            <h2>Recent public activity for this game</h2>
            {gameActivity.length === 0 ? <p>No public activity for this game yet.</p> : null}
            {gameActivity.length > 0 ? (
              <ul>
                {gameActivity.map((item) => (
                  <li key={item.id}>
                    <p>{item.message}</p>
                    <p>{new Date(item.createdAt).toLocaleString()}</p>
                  </li>
                ))}
              </ul>
            ) : null}
          </section>

          <section>
            <button type="button" onClick={() => navigate('/trends')}>
              See overall trends
            </button>
          </section>
        </div>
      ) : null}
    </div>
  );
}
