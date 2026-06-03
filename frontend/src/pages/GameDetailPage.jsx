import { useEffect, useMemo, useState } from 'react';
import { useOutletContext, useParams } from 'react-router-dom';
import CommentsSection from '../components/CommentsSection';
import {
  createComment,
  getGameComments,
  getLocalGame,
  importMobyGame,
  setReaction,
  getReactionSummary,
} from '../api/client';

// The detail page still imports the selected MobyGames entry into the local database,
// but the UI has been intentionally reduced to only the features requested for testing:
// title, like/dislike totals with actions, and comments with a comment form.
export default function GameDetailPage() {
  const { mobyGameId } = useParams();
  const { currentUser, sendToLogin } = useOutletContext();

  const [localGame, setLocalGame] = useState(null);
  const [reactionSummary, setReactionSummary] = useState(null);
  const [comments, setComments] = useState([]);
  const [loading, setLoading] = useState(true);
  const [pageError, setPageError] = useState('');
  const [commentDraft, setCommentDraft] = useState('');
  const [commentSubmitting, setCommentSubmitting] = useState(false);
  const [commentError, setCommentError] = useState('');
  const [reactionSubmitting, setReactionSubmitting] = useState(false);

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
      {loading ? <p>Importing and loading this Zelda game...</p> : null}
      {pageError ? <p role="alert">{pageError}</p> : null}

      {!loading && localGame ? (
        <div>
          <section>
            {/* The title is the only game metadata shown on this page now.
                Everything else has been removed to keep the test surface focused. */}
            <h2>{localGame.title}</h2>
          </section>

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
