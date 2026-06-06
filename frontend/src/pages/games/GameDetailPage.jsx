import { useEffect, useMemo, useState } from 'react';
import { useOutletContext, useParams } from 'react-router-dom';
import {
  createComment,
  getGameComments,
  getLocalGame,
  getLocalGameByMobyGameId,
  getLocalGameBySlug,
  getReactionSummary,
  getZeldaCatalogGameByMobyGameId,
  getZeldaCatalogGameBySlug,
  importMobyGame,
  setReaction,
} from '../../api/client';
import CommentsSection from './components/CommentsSection';
import './GameDetailPage.css';

function ReleaseGroup({ title, items }) {
  if (!items || items.length === 0) {
    return null;
  }

  return (
    <section className="game-detail__section">
      <h3>{title}</h3>
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
    <section className="game-detail__section">
      <h3>Collector's Edition appearances</h3>
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
    <div className="game-detail">
      {selectedGame ? (
        <section className="game-detail__hero">
          <div className="game-detail__hero-media">
            <img src={selectedGame.boxArtPath} alt={`${selectedGame.title} box art`} />
          </div>
          <div className="game-detail__hero-copy">
            <p className="game-detail__eyebrow">Game detail</p>
            <h2>{localGame?.title ?? selectedGame.title}</h2>
            {!loading && localGame?.summary ? <p>{localGame.summary}</p> : null}

            <section className="game-detail__section">
              <h3>Primary console</h3>
              <div className="game-detail__logo-row">
                <span className="game-detail__logo-wrap">
                  <img src={selectedGame.primaryConsole.imagePath} alt={`${selectedGame.primaryConsole.name} logo`} />
                </span>
              </div>
            </section>

            <ReleaseGroup title="Secondary consoles" items={selectedGame.secondaryConsoles} />
            <ReleaseGroup title="Downloaded ports" items={selectedGame.downloadedPorts} />
            <ReleaseGroup title="Nintendo Switch Online" items={selectedGame.subscriptionServices} />
            <CollectorSection items={selectedGame.collectionAppearances} />
          </div>
        </section>
      ) : null}

      {loading ? <p>Importing and loading this Zelda game...</p> : null}
      {pageError ? <p role="alert">{pageError}</p> : null}

      {!loading && localGame ? (
        <div className="game-detail__community-grid">
          <section className="game-detail__section">
            <h2>Likes and dislikes of this game</h2>
            <p>Likes: {reactionSummary?.likes ?? 0}</p>
            <p>Dislikes: {reactionSummary?.dislikes ?? 0}</p>
            <div className="game-detail__actions">
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
