import { useEffect, useMemo, useState } from 'react';

const MAX_COMMENT_LENGTH = 2000;

// I use the same comment form for both new comments and comment edits so validation stays consistent.
export default function CommentEditorForm({
  initialValue = '',
  submitLabel,
  busyLabel,
  submitting,
  onSubmit,
  onCancel,
  onChangeValue,
  idPrefix = 'comment-editor',
}) {
  const [value, setValue] = useState(initialValue);
  const [error, setError] = useState('');
  const [touched, setTouched] = useState(false);

  useEffect(() => {
    setValue(initialValue);
    setError('');
    setTouched(false);
  }, [initialValue]);

  const remainingCharacters = useMemo(() => MAX_COMMENT_LENGTH - value.length, [value.length]);

  function validate(nextValue) {
    const trimmed = nextValue.trim();

    if (trimmed.length === 0) {
      return 'Please write a comment before submitting.';
    }

    if (trimmed.length > MAX_COMMENT_LENGTH) {
      return `Comments must stay under ${MAX_COMMENT_LENGTH} characters.`;
    }

    return '';
  }

  function handleChange(event) {
    const nextValue = event.target.value;
    setValue(nextValue);
    onChangeValue?.(nextValue);

    if (touched) {
      setError(validate(nextValue));
    }
  }

  function handleBlur() {
    setTouched(true);
    setError(validate(value));
  }

  async function handleSubmit(event) {
    event.preventDefault();

    const validationError = validate(value);
    setTouched(true);
    setError(validationError);

    if (validationError) {
      return;
    }

    await onSubmit(value.trim());
  }

  const errorId = `${idPrefix}-error`;
  const helpId = `${idPrefix}-help`;

  return (
    <form className="game-detail__comment-form" onSubmit={handleSubmit} noValidate>
      <label className="game-detail__visually-hidden" htmlFor={`${idPrefix}-body`}>
        Write a comment for this Zelda game
      </label>
      <textarea
        id={`${idPrefix}-body`}
        name={`${idPrefix}-body`}
        rows="5"
        aria-describedby={error ? `${helpId} ${errorId}` : helpId}
        aria-invalid={error ? 'true' : 'false'}
        maxLength={MAX_COMMENT_LENGTH + 50}
        placeholder="Share your thoughts about this Zelda game..."
        value={value}
        onBlur={handleBlur}
        onChange={handleChange}
      />
      <div className="game-detail__comment-form-meta">
        <p id={helpId} className="game-detail__comment-helptext">
          {remainingCharacters >= 0
            ? `${remainingCharacters} characters remaining.`
            : `${Math.abs(remainingCharacters)} characters over the limit.`}
        </p>
        {error ? (
          <p id={errorId} className="game-detail__comment-error" role="alert">
            {error}
          </p>
        ) : null}
      </div>
      <div className="game-detail__comment-form-actions">
        <button type="submit" disabled={submitting}>
          {submitting ? busyLabel : submitLabel}
        </button>
        {onCancel ? (
          <button type="button" className="game-detail__secondary-button" onClick={onCancel} disabled={submitting}>
            Cancel
          </button>
        ) : null}
      </div>
    </form>
  );
}
