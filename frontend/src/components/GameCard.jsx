import { Link } from 'react-router-dom';

// GameCard renders one Zelda title from the MobyGames search results.
export default function GameCard({ game }) {
  return (
    <article>
      <h3>{game.title}</h3>
      {/* Keep this card intentionally minimal for testing: only the summary, platforms,
          and Moby score are shown alongside the title link into the detail page. */}
      <p>{game.description ?? 'No description was provided by MobyGames.'}</p>
      <p>Moby score: {game.mobyScore ?? 'N/A'}</p>
      <p>
        Platforms:{' '}
        {game.platforms.length > 0
          ? game.platforms.map((platform) => platform.platformName).join(', ')
          : 'No platforms returned'}
      </p>
      <Link to={`/games/moby/${game.mobyGameId}`}>Open this Zelda game</Link>
    </article>
  );
}
