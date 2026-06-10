import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import CollectionStatusForm from '../../components/CollectionStatusForm';
import { deleteCollectionItem, favoriteCollectionItem, getMyFavorites, unfavoriteCollectionItem, updateCollectionItem } from '../../api/client';
import './MemberPages.css';

// I use this page as a focused view of only the games I have explicitly favorited.
export default function MyFavoritesPage() {
  const [items, setItems] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [busyItemId, setBusyItemId] = useState('');
  const [busyAction, setBusyAction] = useState('');

  useEffect(() => {
    loadFavorites();
  }, []);

  async function loadFavorites() {
    setLoading(true);
    setError('');

    try {
      const favoriteItems = await getMyFavorites();
      setItems(favoriteItems);
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
      setItems((currentItems) => currentItems.map((item) => (item.id === itemId ? updatedItem : item)));
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
      if (item.isFavorite) {
        await unfavoriteCollectionItem(item.id);
        setItems((currentItems) => currentItems.filter((currentItem) => currentItem.id !== item.id));
      } else {
        const updatedItem = await favoriteCollectionItem(item.id);
        setItems((currentItems) => currentItems.map((currentItem) => (currentItem.id === item.id ? updatedItem : currentItem)));
      }
    } catch (favoriteError) {
      setError(favoriteError.message);
    } finally {
      setBusyItemId('');
      setBusyAction('');
    }
  }

  return (
    <div className="member-page">
      <section className="member-page__hero">
        <p className="member-page__eyebrow">Favorites</p>
        <h1>My favorite Zelda games</h1>
        <p>
          This is the short list I can check when I want to focus on the games I care about most.
        </p>
        <div className="member-page__hero-actions">
          <Link to="/my-collection">Back to collection</Link>
        </div>
      </section>

      <section className="member-page__card">
        <h2>Favorite entries</h2>
        {loading ? <p>Loading your favorites...</p> : null}
        {error ? <p role="alert">{error}</p> : null}
        {!loading && !error && items.length === 0 ? <p>You have not favorited any Zelda games yet.</p> : null}
        {!loading && !error && items.length > 0 ? (
          <ul className="member-page__list">
            {items.map((item) => (
              <li key={item.id}>
                <article className="member-page__item-card">
                  <div className="member-page__item-media">
                    <img src={item.imageUrl ?? '/boxart/ocarinaOfTime.jpg'} alt={`${item.gameTitle} box art`} />
                  </div>
                  <div className="member-page__item-copy">
                    <h3>{item.gameTitle}</h3>
                    <div className="member-page__meta">
                      <p>Status: {item.status}</p>
                      <p>Favorited: {item.favoritedAt ? new Date(item.favoritedAt).toLocaleString() : 'Not yet'}</p>
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
