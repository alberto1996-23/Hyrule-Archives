import { useEffect, useMemo, useState } from 'react';
import { useOutletContext, useParams } from 'react-router-dom';
import CommentsSection from '../components/CommentsSection';
import {
  createComment,
  getGameComments,
  getLocalGame,
  getLocalGameByMobyGameId,
  getLocalGameBySlug,
  getZeldaCatalogGameByMobyGameId,
  getZeldaCatalogGameBySlug,
  importMobyGame,
  setReaction,
  getReactionSummary,
} from '../api/client';

function ReleaseGroup({ title, items }) {
  if (!items || items.length === 0) {
    return null;
  }

  return (
    <section>
      <h3>{title}</h3>
      <ul>
        {items.map((item) => (
          <li key={`${title}-${item.name}`}>
            <p>{item.name}</p>
            <img src={item.src} alt={`${item.name} logo`} />
            {item.note ? <p>{item.note}</p> : null}
          </li>
        ))}
      </ul>
    </section>
  );
}

function CollectorSection({ items }) {
  if (!items || items.length === 0) {
    return null;
  }

  return (
    <section>
      <h3>Collector's Edition appearances</h3>
      <ul>
        {items.map((item) => (
          <li key={item.title}>
            <p>{item.title}</p>
            <img src={item.boxArtPath} alt={`${item.title} box art`} />
            <img src={item.platform.src} alt={`${item.platform.name} logo`} />
            {item.platform.note ? <p>{item.platform.note}</p> : null}
          </li>
        ))}
      </ul>
    </section>
  );
}

// The detail page still imports the selected MobyGames entry into the local database,
// but the UI keeps the requested title, MobyGames summary, platform availability,
// like/dislike totals, and comments with a comment form.
export default function GameDetailPage() {
  const { gameSlug, mobyGameId } = useParams();
  const { currentUser, sendToLogin } = useOutletContext();

  const [selectedGame, setSelectedGame] = useState(null);
  const [localGame, setLocalGame] = useState(null);
  const [reactionSummary, setReactionSummary] = useState(null);
  const [comments, setComments] = useState([]);
  const [loading, setLoading] = useState(true);
  const [pageError, setPageError] = useState('');
  const [commentDraft, setCommentDraft] = useState('');
  const [commentSubmitting, setCommentSubmitting] = useState(false);
  const [commentError, setCommentError] = useState('');
  const [reactionSubmitting, setReactionSubmitting] = useState(false);

  const returnPath = useMemo(() => {
    if (selectedGame) {
      return `/games/${selectedGame.slug}`;
    }

    if (gameSlug) {
      return `/games/${gameSlug}`;
    }

    return `/games/moby/${mobyGameId}`;
  }, [gameSlug, mobyGameId, selectedGame]);

  useEffect(() => {
    let isMounted = true;

    async function loadGamePage() {
      setLoading(true);
      setPageError('');
      setSelectedGame(null);
      setLocalGame(null);
      setReactionSummary(null);
      setComments([]);

      try {
        const catalogGame = gameSlug
          ? await getZeldaCatalogGameBySlug(gameSlug)
          : await getZeldaCatalogGameByMobyGameId(mobyGameId);

        if (!isMounted) {
          return;
        }

        setSelectedGame(catalogGame);

        let localGameId;

        try {
          const existingLocalGame = await getLocalGameByMobyGameId(catalogGame.mobyGameId);
          localGameId = existingLocalGame.id;
        } catch (mobyLookupError) {
          if (mobyLookupError.status !== 404) {
            throw mobyLookupError;
          }

          try {
            const imported = await importMobyGame(catalogGame.mobyGameId);
            localGameId = imported.localGameId;
          } catch (importError) {
            // Best-effort fallback for older locally-seeded rows whose slug already matches
            // the curated route, even if they were never linked to MobyGames yet.
            try {
              const seededLocalGame = await getLocalGameBySlug(catalogGame.slug);
              localGameId = seededLocalGame.id;
            } catch {
              throw importError;
            }
          }
        }

        if (!isMounted) {
          return;
        }

        await hydrateGameState(localGameId, isMounted);
      } catch (loadError) {
        if (isMounted) {
          if (loadError.status === 404) {
            setPageError('This Zelda game could not be found in the catalog.');
          } else if (loadError.status === 429) {
            setPageError('MobyGames temporarily rate-limited the import request. Already-saved local games still work, but new imports need a short wait before retrying.');
          } else {
            setPageError(loadError.message);
          }
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
  }, [currentUser, gameSlug, mobyGameId]);

  async function hydrateGameState(localGameId, isMountedFlag = true) {
    const [game, summary, commentItems] = await Promise.all([
      getLocalGame(localGameId),
      getReactionSummary(localGameId),
      getGameComments(localGameId),
    ]);

    if (!isMountedFlag) {
      return;
    }

    setLocalGame(game);
    setReactionSummary(summary);
    setComments(commentItems);
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
    setCommentError('');

    try {
      await setReaction(localGame.id, value);
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

    try {
      await createComment(localGame.id, commentDraft.trim());
      setCommentDraft('');
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
      {selectedGame ? (
        <section>
          <h2>{localGame?.title ?? selectedGame.title}</h2>
          <img src={selectedGame.boxArtPath} alt={`${selectedGame.title} box art`} />
          {!loading && localGame?.summary ? <p>{localGame.summary}</p> : null}

          <section>
            <h3>Primary console</h3>
            <p>{selectedGame.primaryConsole.name}</p>
            <img src={selectedGame.primaryConsole.src} alt={`${selectedGame.primaryConsole.name} logo`} />
            {selectedGame.primaryConsole.note ? <p>{selectedGame.primaryConsole.note}</p> : null}
          </section>

          <ReleaseGroup title="Secondary consoles" items={selectedGame.secondaryConsoles} />
          <ReleaseGroup title="Downloaded ports" items={selectedGame.downloadedPorts} />
          <ReleaseGroup title="Nintendo Switch Online" items={selectedGame.subscriptionServices} />
          <CollectorSection items={selectedGame.collectionAppearances} />
        </section>
      ) : null}

      {loading ? <p>Importing and loading this Zelda game...</p> : null}
      {pageError ? <p role="alert">{pageError}</p> : null}

      {!loading && localGame ? (
        <div>
          <section>
            <h2>Likes and dislikes of this game</h2>
            <p>Likes: {reactionSummary?.likes ?? 0}</p>
            <p>Dislikes: {reactionSummary?.dislikes ?? 0}</p>
            <div>
              {/* The page keeps only the direct like/dislike actions requested for testing. */}
              <button type="button" onClick={() => handleReaction('Like')} disabled={reactionSubmitting}>
                Like
              </button>
              <button type="button" onClick={() => handleReaction('Dislike')} disabled={reactionSubmitting}>
                Dislike
              </button>
            </div>
          </section>

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
        </div>
      ) : null}
    </div>
  );
}
