import { Link } from 'react-router-dom';

// GameCard renders one curated Zelda title from the static catalog.
export default function GameCard({ game }) {
  return (
    <article>
      <Link to={`/games/${game.slug}`}>
        <h3>{game.title}</h3>
        <img src={game.boxArtPath} alt={`${game.title} box art`} />
      </Link>
    </article>
  );
}
