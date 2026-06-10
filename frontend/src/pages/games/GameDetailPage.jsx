import { useEffect, useMemo, useState } from 'react';
import { Link, useOutletContext, useParams } from 'react-router-dom';
import {
  addToCollection,
  createComment,
  deleteComment,
  getGameComments,
  getLocalGame,
  getLocalGameByMobyGameId,
  getLocalGameBySlug,
  getMyCollection,
  getReactionSummary,
  getZeldaCatalogGameByMobyGameId,
  getZeldaCatalogGameBySlug,
  importMobyGame,
  setReaction,
  updateComment,
} from '../../api/client';
import { CommentComposerSection, RecentCommentsSection } from './components/CommentsSection';
import './GameDetailPage.css';

function ReleaseGroup({ title, items }) {
  if (!items || items.length === 0) {
    return null;
  }

  return (
    // Sketch update: each platform family now sits in its own full-width card under the hero area.
    <section className="game-detail__section game-detail__platform-section game-detail__fade game-detail__fade--6">
      <h2 className="game-detail__section-title">{title}</h2>
      <div className="game-detail__logo-row">
        {items.map((item) => (
          <span key={`${title}-${item.name}`} className="game-detail__logo-wrap">
            <img src={item.imagePath} alt={`${item.name} logo`} />
          </span>
        ))}
      </div>
    </section>
  );
}

function CollectorSection({ items }) {
  if (!items || items.length === 0) {
    return null;
  }

  return (
    <section className="game-detail__section game-detail__platform-section game-detail__fade game-detail__fade--6">
      <h2 className="game-detail__section-title">Collector's Edition appearances</h2>
      <div className="game-detail__collector-row">
        {items.map((item) => (
          <span key={item.title} className="game-detail__collector-wrap">
            <img src={item.boxArtPath} alt={`${item.title} box art`} />
          </span>
        ))}
      </div>
    </section>
  );
}

// The detail page still imports the selected MobyGames entry into the local database,
// but the UI keeps the requested title, summary, platform availability, reactions, and comments.
export default function GameDetailPage() {
  const { gameSlug, mobyGameId, localGameId } = useParams();
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
  const [commentActionId, setCommentActionId] = useState('');
  const [commentActionMode, setCommentActionMode] = useState('');
  const [collectionItem, setCollectionItem] = useState(null);
  const [collectionSubmitting, setCollectionSubmitting] = useState(false);
  const [collectionMessage, setCollectionMessage] = useState('');
  const [reactionSubmitting, setReactionSubmitting] = useState(false);

  const returnPath = useMemo(() => {
    if (localGameId) {
      return `/games/local/${localGameId}`;
    }

    if (selectedGame) {
      return `/games/${selectedGame.slug}`;
    }

    if (gameSlug) {
      return `/games/${gameSlug}`;
    }

    return `/games/moby/${mobyGameId}`;
  }, [gameSlug, localGameId, mobyGameId, selectedGame]);

  function extractMobyGameId(externalApiId) {
    if (!externalApiId?.startsWith('mobygames-')) {
      return null;
    }

    const parsed = Number.parseInt(externalApiId.replace('mobygames-', ''), 10);
    return Number.isNaN(parsed) ? null : parsed;
  }

  // Sketch update: recent comments are shown newest-first in the lower full-width section.
  const recentComments = useMemo(
    () => [...comments].sort((left, right) => new Date(right.createdAt) - new Date(left.createdAt)),
    [comments],
  );

  useEffect(() => {
    let isMounted = true;

    async function loadGamePage() {
      setLoading(true);
      setPageError('');
      setSelectedGame(null);
      setLocalGame(null);
      setReactionSummary(null);
      setComments([]);
      setCollectionItem(null);
      setCollectionMessage('');

      try {
        if (localGameId) {
          const existingLocalGame = await getLocalGame(localGameId);

          if (!isMounted) {
            return;
          }

          const localMobyGameId = extractMobyGameId(existingLocalGame.externalApiId);

          if (localMobyGameId) {
            try {
              const catalogGame = await getZeldaCatalogGameByMobyGameId(localMobyGameId);

              if (isMounted) {
                setSelectedGame(catalogGame);
              }
            } catch (catalogLookupError) {
              if (catalogLookupError.status !== 404) {
                throw catalogLookupError;
              }
            }
          }

          if (!isMounted) {
            return;
          }

          await hydrateGameState(existingLocalGame.id, isMounted);
          return;
        }

        const catalogGame = gameSlug
          ? await getZeldaCatalogGameBySlug(gameSlug)
          : await getZeldaCatalogGameByMobyGameId(mobyGameId);

        if (!isMounted) {
          return;
        }

        setSelectedGame(catalogGame);

        let resolvedLocalGameId;

        try {
          const existingLocalGame = await getLocalGameByMobyGameId(catalogGame.mobyGameId);
          resolvedLocalGameId = existingLocalGame.id;
        } catch (mobyLookupError) {
          if (mobyLookupError.status !== 404) {
            throw mobyLookupError;
          }

          try {
            const imported = await importMobyGame(catalogGame.mobyGameId);
            resolvedLocalGameId = imported.localGameId;
          } catch (importError) {
            try {
              const seededLocalGame = await getLocalGameBySlug(catalogGame.slug);
              resolvedLocalGameId = seededLocalGame.id;
            } catch {
              throw importError;
            }
          }
        }

        if (!isMounted) {
          return;
        }

        await hydrateGameState(resolvedLocalGameId, isMounted);
      } catch (loadError) {
        if (isMounted) {
          if (loadError.status === 404) {
            setPageError(localGameId ? 'This Zelda game could not be found in the archive.' : 'This Zelda game could not be found in the catalog.');
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
  }, [currentUser, gameSlug, localGameId, mobyGameId]);

  async function hydrateGameState(localGameId, isMountedFlag = true) {
    const [game, summary, commentItems, collectionItems] = await Promise.all([
      getLocalGame(localGameId),
      getReactionSummary(localGameId),
      getGameComments(localGameId),
      currentUser ? getMyCollection() : Promise.resolve([]),
    ]);

    if (!isMountedFlag) {
      return;
    }

    setLocalGame(game);
    setReactionSummary(summary);
    setComments(commentItems);
    setCollectionItem(collectionItems.find((item) => item.gameId === localGameId) ?? null);
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

  async function handleCommentSubmit(eventOrBody) {
    const explicitBody = typeof eventOrBody === 'string' ? eventOrBody : null;

    if (!explicitBody) {
      eventOrBody.preventDefault();
    }

    if (!currentUser) {
      goToLogin();
      return;
    }

    const nextCommentBody = explicitBody ?? commentDraft.trim();

    if (!localGame || nextCommentBody.length === 0) {
      return;
    }

    setCommentSubmitting(true);
    setCommentError('');

    try {
      await createComment(localGame.id, nextCommentBody);
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

  async function handleAddToCollection() {
    if (!currentUser) {
      goToLogin();
      return;
    }

    if (!localGame) {
      return;
    }

    setCollectionSubmitting(true);
    setCollectionMessage('');

    try {
      await addToCollection(localGame.id);
      setCollectionMessage('Added to your collection.');
      await hydrateGameState(localGame.id);
    } catch (error) {
      if (error.status === 401) {
        goToLogin();
      } else {
        setCollectionMessage(error.message);
      }
    } finally {
      setCollectionSubmitting(false);
    }
  }

  async function handleUpdateComment(commentId, body) {
    if (!localGame) {
      return;
    }

    setCommentActionId(commentId);
    setCommentActionMode('save');
    setCommentError('');

    try {
      await updateComment(commentId, body);
      await hydrateGameState(localGame.id);
    } catch (error) {
      setCommentError(error.message);
    } finally {
      setCommentActionId('');
      setCommentActionMode('');
    }
  }

  async function handleDeleteComment(commentId) {
    if (!localGame) {
      return;
    }

    setCommentActionId(commentId);
    setCommentActionMode('delete');
    setCommentError('');

    try {
      await deleteComment(commentId);
      await hydrateGameState(localGame.id);
    } catch (error) {
      setCommentError(error.message);
    } finally {
      setCommentActionId('');
      setCommentActionMode('');
    }
  }

  return (
    <div className="game-detail">
      {selectedGame || localGame ? (
        <>
          <section className="game-detail__hero game-detail__fade game-detail__fade--1">
            {/* Sketch update: the cover art now floats beside the copy so the summary wraps around it. */}
            <div className="game-detail__hero-story">
              <img
                className="game-detail__hero-boxart game-detail__fade game-detail__fade--2"
                src={selectedGame?.boxArtPath ?? localGame?.imageUrl ?? '/boxart/ocarinaOfTime.jpg'}
                alt={`${localGame?.title ?? selectedGame?.title ?? 'Zelda game'} box art`}
              />
              <h1 className="game-detail__title game-detail__fade game-detail__fade--3">
                {localGame?.title ?? selectedGame?.title}
              </h1>
              {!loading && localGame?.summary ? (
                <p className="game-detail__summary game-detail__fade game-detail__fade--4">{localGame.summary}</p>
              ) : null}
              {!loading && localGame ? (
                <div className="game-detail__hero-actions game-detail__fade game-detail__fade--5">
                  <button
                    type="button"
                    onClick={handleAddToCollection}
                    disabled={collectionSubmitting || Boolean(collectionItem)}
                  >
                    {collectionSubmitting
                      ? 'Saving...'
                      : collectionItem
                        ? 'Already in your collection'
                        : 'Add to my collection'}
                  </button>
                  {collectionItem ? <Link to="/my-collection">Open my collection</Link> : null}
                </div>
              ) : null}
              {collectionMessage ? <p className="game-detail__collection-message">{collectionMessage}</p> : null}
            </div>
          </section>

          {selectedGame?.primaryConsole ? (
            <section className="game-detail__section game-detail__platform-section game-detail__fade game-detail__fade--5">
              <h2 className="game-detail__section-title game-detail__section-title--center">Primary Console</h2>
              <div className="game-detail__logo-row game-detail__logo-row--centered">
                <span className="game-detail__logo-wrap game-detail__logo-wrap--primary">
                  <img src={selectedGame.primaryConsole.imagePath} alt={`${selectedGame.primaryConsole.name} logo`} />
                </span>
              </div>
            </section>
          ) : null}
        </>
      ) : null}

      {loading ? <p>Importing and loading this Zelda game...</p> : null}
      {pageError ? <p role="alert">{pageError}</p> : null}

      {!loading && localGame ? (
        <>
          <div className="game-detail__content-grid game-detail__content-grid--community">
            <div className="game-detail__platform-column">
              <ReleaseGroup title="Secondary Consoles" items={selectedGame?.secondaryConsoles} />
              <ReleaseGroup title="Downloaded Ports" items={selectedGame?.downloadedPorts} />
              <ReleaseGroup title="Nintendo Switch Online" items={selectedGame?.subscriptionServices} />
              <CollectorSection items={selectedGame?.collectionAppearances} />
            </div>

            <aside className="game-detail__community-column">
              <section className="game-detail__section game-detail__engagement-panel game-detail__fade game-detail__fade--6">
                <div className="game-detail__reaction-grid">
                  <div className="game-detail__reaction-card">
                    <h3>Likes</h3>
                    <p className="game-detail__reaction-total">{reactionSummary?.likes ?? 0}</p>
                  </div>
                  <div className="game-detail__reaction-card">
                    <h3>Dislikes</h3>
                    <p className="game-detail__reaction-total">{reactionSummary?.dislikes ?? 0}</p>
                  </div>
                </div>
                <div className="game-detail__actions">
                  <button type="button" onClick={() => handleReaction('Like')} disabled={reactionSubmitting}>
                    {reactionSubmitting ? 'Saving...' : 'Like'}
                  </button>
                  <button type="button" onClick={() => handleReaction('Dislike')} disabled={reactionSubmitting}>
                    {reactionSubmitting ? 'Saving...' : 'Dislike'}
                  </button>
                </div>
              </section>

              <CommentComposerSection
                currentUser={currentUser}
                commentDraft={commentDraft}
                commentSubmitting={commentSubmitting}
                commentError={commentError}
                onDraftChange={setCommentDraft}
                onSubmitComment={handleCommentSubmit}
                onGoToLogin={goToLogin}
              />
            </aside>
          </div>

          <RecentCommentsSection
            comments={recentComments}
            commentsLoading={loading}
            currentUser={currentUser}
            commentActionId={commentActionId}
            commentActionMode={commentActionMode}
            onDeleteComment={handleDeleteComment}
            onUpdateComment={handleUpdateComment}
          />
        </>
      ) : null}
    </div>
  );
}
