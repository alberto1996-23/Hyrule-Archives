import { Link } from 'react-router-dom';

// GameCard renders one Zelda title from the MobyGames search results.
export default function GameCard({ game }) {
  return (
    <article>
      <h3>{game.title}</h3>
      <p>{game.description ?? 'No description was provided by MobyGames.'}</p>
      <p>Moby score: {game.mobyScore ?? 'N/A'}</p>
      <p>
        Platforms:{' '}
        {game.platforms.length > 0
          ? game.platforms.map((platform) => platform.platformName).join(', ')
          : 'No platforms returned'}
      </p>
      <p>{game.sourceCredit}</p>
      <Link to={`/games/moby/${game.mobyGameId}`}>Open this Zelda game</Link>
    </article>
  );
}
