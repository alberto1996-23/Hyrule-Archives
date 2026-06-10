import { useEffect, useState } from 'react';

const ALLOWED_STATUSES = ['Saved', 'Playing', 'Completed', 'Dropped'];

// I use this reusable editor on both the collection and favorites pages so item updates behave the same way everywhere.
export default function CollectionStatusForm({
  item,
  busyAction,
  onSaveStatus,
  onDeleteItem,
  onToggleFavorite,
}) {
  const [status, setStatus] = useState(item.status);
  const [error, setError] = useState('');

  useEffect(() => {
    setStatus(item.status);
    setError('');
  }, [item.status]);

  function validate(nextStatus) {
    if (!ALLOWED_STATUSES.includes(nextStatus)) {
      return 'Choose one of the supported collection statuses.';
    }

    return '';
  }

  async function handleSubmit(event) {
    event.preventDefault();

    const validationError = validate(status);
    setError(validationError);

    if (validationError) {
      return;
    }

    await onSaveStatus(item.id, status);
  }

  return (
    <form className="member-page__item-editor" onSubmit={handleSubmit} noValidate>
      <label htmlFor={`collection-status-${item.id}`}>Status</label>
      <select
        id={`collection-status-${item.id}`}
        value={status}
        onChange={(event) => {
          setStatus(event.target.value);
          setError('');
        }}
      >
        {ALLOWED_STATUSES.map((option) => (
          <option key={option} value={option}>
            {option}
          </option>
        ))}
      </select>
      {error ? (
        <p className="member-page__inline-error" role="alert">
          {error}
        </p>
      ) : null}
      <div className="member-page__item-actions">
        <button type="submit" disabled={busyAction === 'save'}>
          {busyAction === 'save' ? 'Saving...' : 'Save status'}
        </button>
        <button type="button" className="member-page__secondary-button" onClick={() => onToggleFavorite(item)} disabled={busyAction === 'favorite'}>
          {busyAction === 'favorite' ? 'Updating...' : item.isFavorite ? 'Remove favorite' : 'Add favorite'}
        </button>
        <button type="button" className="member-page__danger-button" onClick={() => onDeleteItem(item)} disabled={busyAction === 'delete'}>
          {busyAction === 'delete' ? 'Removing...' : 'Remove'}
        </button>
      </div>
    </form>
  );
}
