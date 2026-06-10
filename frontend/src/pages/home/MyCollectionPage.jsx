import { useEffect, useMemo, useState } from 'react';
import { Link } from 'react-router-dom';
import CollectionStatusForm from '../../components/CollectionStatusForm';
import { deleteCollectionItem, favoriteCollectionItem, getMyCollection, unfavoriteCollectionItem, updateCollectionItem } from '../../api/client';
import './MemberPages.css';

// I use this page as the main private collection manager for saved Zelda games.
export default function MyCollectionPage() {
  const [items, setItems] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [query, setQuery] = useState('');
  const [statusFilter, setStatusFilter] = useState('All');
  const [sortOrder, setSortOrder] = useState('recent');
  const [busyItemId, setBusyItemId] = useState('');
  const [busyAction, setBusyAction] = useState('');

  useEffect(() => {
    loadCollection();
  }, []);

  async function loadCollection() {
    setLoading(true);
    setError('');

    try {
      const collectionItems = await getMyCollection();
      setItems(collectionItems);
    } catch (loadError) {
      setError(loadError.message);
    } finally {
      setLoading(false);
    }
  }

  async function handleSaveStatus(itemId, nextStatus) {
    setBusyItemId(itemId);
    setBusyAction('save');

    try {
      const updatedItem = await updateCollectionItem(itemId, nextStatus);
      setItems((currentItems) =>
        currentItems.map((item) => (item.id === itemId ? updatedItem : item)),
      );
    } catch (updateError) {
      setError(updateError.message);
    } finally {
      setBusyItemId('');
      setBusyAction('');
    }
  }

  async function handleDeleteItem(item) {
    setBusyItemId(item.id);
    setBusyAction('delete');

    try {
      await deleteCollectionItem(item.id);
      setItems((currentItems) => currentItems.filter((currentItem) => currentItem.id !== item.id));
    } catch (deleteError) {
      setError(deleteError.message);
    } finally {
      setBusyItemId('');
      setBusyAction('');
    }
  }

  async function handleToggleFavorite(item) {
    setBusyItemId(item.id);
    setBusyAction('favorite');

    try {
      const updatedItem = item.isFavorite
        ? await unfavoriteCollectionItem(item.id)
        : await favoriteCollectionItem(item.id);

      setItems((currentItems) =>
        currentItems.map((currentItem) => (currentItem.id === item.id ? updatedItem : currentItem)),
      );
    } catch (favoriteError) {
      setError(favoriteError.message);
    } finally {
      setBusyItemId('');
      setBusyAction('');
    }
  }

  const filteredItems = useMemo(() => {
    const normalizedQuery = query.trim().toLowerCase();

    const matchingItems = items.filter((item) => {
      const matchesQuery =
        normalizedQuery.length === 0 ||
        item.gameTitle.toLowerCase().includes(normalizedQuery) ||
        item.status.toLowerCase().includes(normalizedQuery);

      const matchesStatus = statusFilter === 'All' || item.status === statusFilter;

      return matchesQuery && matchesStatus;
    });

    return [...matchingItems].sort((left, right) => {
      if (sortOrder === 'title') {
        return left.gameTitle.localeCompare(right.gameTitle);
      }

      return new Date(right.updatedAt) - new Date(left.updatedAt);
    });
  }, [items, query, statusFilter, sortOrder]);

  return (
    <div className="member-page">
      <section className="member-page__hero">
        <p className="member-page__eyebrow">Private collection</p>
        <h1>My Zelda collection</h1>
        <div className="member-page__hero-actions">
          <Link to="/games">Browse more games</Link>
          <Link className="member-page__secondary-button" to="/my-favorites">
            Open favorites
          </Link>
        </div>
      </section>

      <section className="member-page__card">
        <h2>Filter and sort</h2>
        <div className="member-page__toolbar">
          <label htmlFor="collection-search">
            Search
            <input
              id="collection-search"
              type="search"
              value={query}
              onChange={(event) => setQuery(event.target.value)}
              placeholder="Search by title or status"
            />
          </label>
          <label htmlFor="collection-status-filter">
            Status filter
            <select id="collection-status-filter" value={statusFilter} onChange={(event) => setStatusFilter(event.target.value)}>
              <option value="All">All statuses</option>
              <option value="Saved">Saved</option>
              <option value="Playing">Playing</option>
              <option value="Completed">Completed</option>
              <option value="Dropped">Dropped</option>
            </select>
          </label>
          <label htmlFor="collection-sort-order">
            Sort by
            <select id="collection-sort-order" value={sortOrder} onChange={(event) => setSortOrder(event.target.value)}>
              <option value="recent">Recently updated</option>
              <option value="title">Title</option>
            </select>
          </label>
        </div>
      </section>

      <section className="member-page__card">
        <h2>Saved games</h2>
        {loading ? <p>Loading your collection...</p> : null}
        {error ? <p role="alert">{error}</p> : null}
        {!loading && !error && filteredItems.length === 0 ? (
          <p>No collection items matched your current filters.</p>
        ) : null}
        {!loading && !error && filteredItems.length > 0 ? (
          <ul className="member-page__list">
            {filteredItems.map((item) => (
              <li key={item.id}>
                <article className="member-page__item-card">
                  <div className="member-page__item-media">
                    <img src={item.imageUrl ?? '/boxart/ocarinaOfTime.jpg'} alt={`${item.gameTitle} box art`} />
                  </div>
                  <div className="member-page__item-copy">
                    <h3>{item.gameTitle}</h3>
                    <div className="member-page__meta">
                      <p>Current status: {item.status}</p>
                      <p>Favorite: {item.isFavorite ? 'Yes' : 'No'}</p>
                      <p>Last updated: {new Date(item.updatedAt).toLocaleString()}</p>
                      <p>
                        Detail page: <Link to={`/games/${item.gameSlug}`}>open {item.gameTitle}</Link>
                      </p>
                    </div>
                    <CollectionStatusForm
                      item={item}
                      busyAction={busyItemId === item.id ? busyAction : ''}
                      onSaveStatus={handleSaveStatus}
                      onDeleteItem={handleDeleteItem}
                      onToggleFavorite={handleToggleFavorite}
                    />
                  </div>
                </article>
              </li>
            ))}
          </ul>
        ) : null}
      </section>
    </div>
  );
}
