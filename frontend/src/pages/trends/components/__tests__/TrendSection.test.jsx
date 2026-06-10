import { render, screen } from '@testing-library/react';
import TrendSection from '../TrendSection';

describe('TrendSection', () => {
  it('renders an empty state when no leaderboard items are available', () => {
    render(<TrendSection title="Trending games" items={[]} />);

    expect(screen.getByText('No games have entered this leaderboard yet.')).toBeInTheDocument();
  });

  it('renders leaderboard details for populated items', () => {
    render(
      <TrendSection
        title="Trending games"
        items={[
          {
            gameId: 'game-1',
            title: 'The Wind Waker',
            favoriteCount: 5,
            commentCount: 3,
            likeCount: 8,
            dislikeCount: 1,
          },
        ]}
      />,
    );

    expect(screen.getByText('The Wind Waker')).toBeInTheDocument();
    expect(screen.getByText('Favorites: 5')).toBeInTheDocument();
    expect(screen.getByText('Comments: 3')).toBeInTheDocument();
    expect(screen.getByText('Likes: 8')).toBeInTheDocument();
    expect(screen.getByText('Dislikes: 1')).toBeInTheDocument();
  });
});
