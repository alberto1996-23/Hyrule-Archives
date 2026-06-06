// TrendSection renders one leaderboard block so the page can reuse the same markup.
export default function TrendSection({ title, items }) {
  return (
    <section className="trends-page__card">
      <h3>{title}</h3>
      {items.length === 0 ? <p>No games have entered this leaderboard yet.</p> : null}
      {items.length > 0 ? (
        <ol className="trends-page__leaderboard">
          {items.map((item) => (
            <li key={item.gameId}>
              <p className="trends-page__game-title">{item.title}</p>
              <p>Favorites: {item.favoriteCount}</p>
              <p>Comments: {item.commentCount}</p>
              <p>Likes: {item.likeCount}</p>
              <p>Dislikes: {item.dislikeCount}</p>
            </li>
          ))}
        </ol>
      ) : null}
    </section>
  );
}
