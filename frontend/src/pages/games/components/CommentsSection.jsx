// This component shows the comment form and the existing comment list for one game.
export default function CommentsSection({
  currentUser,
  commentDraft,
  comments,
  commentsLoading,
  commentSubmitting,
  commentError,
  onDraftChange,
  onSubmitComment,
  onGoToLogin,
}) {
  return (
    <section className="game-detail__section">
      <h2>Comments</h2>
      {currentUser ? (
        <form className="game-detail__comment-form" onSubmit={onSubmitComment}>
          <label htmlFor="commentBody">Write a comment for this Zelda game</label>
          <textarea
            id="commentBody"
            name="commentBody"
            rows="5"
            value={commentDraft}
            onChange={(event) => onDraftChange(event.target.value)}
          />
          <div>
            <button type="submit" disabled={commentSubmitting || commentDraft.trim().length === 0}>
              {commentSubmitting ? 'Posting comment...' : 'Post comment'}
            </button>
          </div>
        </form>
      ) : (
        <div className="game-detail__login-prompt">
          <p>You must be logged in before posting a comment.</p>
          <button type="button" onClick={onGoToLogin}>
            Go to login
          </button>
        </div>
      )}

      {commentError ? <p role="alert">{commentError}</p> : null}
      {commentsLoading ? <p>Loading comments...</p> : null}
      {!commentsLoading && comments.length === 0 ? <p>No comments yet for this game.</p> : null}
      {!commentsLoading && comments.length > 0 ? (
        <ul className="game-detail__comment-list">
          {comments.map((comment) => (
            <li key={comment.id}>
              <p>
                <strong>{comment.displayName ?? 'Anonymous user'}</strong>
              </p>
              <p>{comment.body}</p>
              <p>
                Posted: {new Date(comment.createdAt).toLocaleString()}
                {comment.updatedAt !== comment.createdAt ? ' (edited)' : ''}
              </p>
            </li>
          ))}
        </ul>
      ) : null}
    </section>
  );
}
