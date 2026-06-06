import { Link } from 'react-router-dom';

// GameCard renders one curated Zelda title from the catalog.
export default function GameCard({ game }) {
  return (
    <article className="game-card">
      <Link className="game-card__link" to={`/games/${game.slug}`}>
        <img className="game-card__image" src={game.boxArtPath} alt={`${game.title} box art`} />
        <div className="game-card__body">
          <h3>{game.title}</h3>
        </div>
      </Link>
    </article>
  );
}
