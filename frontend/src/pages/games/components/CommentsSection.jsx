import { useState } from 'react';
import CommentEditorForm from './CommentEditorForm';

function formatCommentTimestamp(value) {
  return new Date(value).toLocaleString([], {
    dateStyle: 'medium',
    timeStyle: 'short',
  });
}

// Sketch update: the form is now its own card so it can sit beside the platform sections.
export function CommentComposerSection({
  currentUser,
  commentDraft,
  commentSubmitting,
  commentError,
  onDraftChange,
  onSubmitComment,
  onGoToLogin,
}) {
  return (
    <section className="game-detail__section game-detail__comment-composer game-detail__fade game-detail__fade--7">
      <h2 className="game-detail__section-title game-detail__section-title--center">Post a Comment!</h2>
      {currentUser ? (
        <CommentEditorForm
          initialValue={commentDraft}
          submitLabel="Post"
          busyLabel="Posting..."
          submitting={commentSubmitting}
          idPrefix="create-comment"
          onChangeValue={onDraftChange}
          onSubmit={onSubmitComment}
        />
      ) : (
        <div className="game-detail__login-prompt">
          <p>You must be logged in before posting a comment.</p>
          <button type="button" onClick={onGoToLogin}>
            Go to login
          </button>
        </div>
      )}

      {commentError ? <p role="alert">{commentError}</p> : null}
    </section>
  );
}

// Sketch update: recent comments moved into a separate full-width section under the two-column layout.
export function RecentCommentsSection({
  comments,
  commentsLoading,
  currentUser,
  commentActionId,
  commentActionMode,
  onDeleteComment,
  onUpdateComment,
}) {
  const [editingCommentId, setEditingCommentId] = useState('');

  return (
    <section className="game-detail__section game-detail__recent-comments game-detail__fade game-detail__fade--8">
      <h2 className="game-detail__section-title game-detail__section-title--center">Recent Comments:</h2>
      {commentsLoading ? <p>Loading comments...</p> : null}
      {!commentsLoading && comments.length === 0 ? <p>No comments yet for this game.</p> : null}
      {!commentsLoading && comments.length > 0 ? (
        <ul className="game-detail__comment-list">
          {comments.map((comment, index) => {
            const isOwner = currentUser?.id === comment.appUserId;
            const isEditing = editingCommentId === comment.id;
            const isBusy = commentActionId === comment.id;

            return (
              <li key={comment.id} className="game-detail__comment-card" style={{ animationDelay: `${index * 120}ms` }}>
                <p className="game-detail__comment-author">{comment.displayName ?? 'Anonymous user'}:</p>
                {isEditing ? (
                  <CommentEditorForm
                    initialValue={comment.body}
                    submitLabel="Save edit"
                    busyLabel="Saving..."
                    submitting={isBusy && commentActionMode === 'save'}
                    idPrefix={`edit-comment-${comment.id}`}
                    onCancel={() => setEditingCommentId('')}
                    onSubmit={async (body) => {
                      await onUpdateComment(comment.id, body);
                      setEditingCommentId('');
                    }}
                  />
                ) : (
                  <>
                    <p className="game-detail__comment-body">{comment.body}</p>
                    <p className="game-detail__comment-meta">
                      on {formatCommentTimestamp(comment.createdAt)}
                      {comment.updatedAt !== comment.createdAt ? ' • edited' : ''}
                    </p>
                  </>
                )}
                {isOwner && !isEditing ? (
                  <div className="game-detail__comment-card-actions">
                    <button type="button" className="game-detail__secondary-button" onClick={() => setEditingCommentId(comment.id)}>
                      Edit
                    </button>
                    <button
                      type="button"
                      className="game-detail__danger-button"
                      onClick={() => onDeleteComment(comment.id)}
                      disabled={isBusy && commentActionMode === 'delete'}
                    >
                      {isBusy && commentActionMode === 'delete' ? 'Deleting...' : 'Delete'}
                    </button>
                  </div>
                ) : null}
              </li>
            );
          })}
        </ul>
      ) : null}
    </section>
  );
}

export default function CommentsSection(props) {
  return (
    <>
      <CommentComposerSection {...props} />
      <RecentCommentsSection comments={props.comments} commentsLoading={props.commentsLoading} />
    </>
  );
}
