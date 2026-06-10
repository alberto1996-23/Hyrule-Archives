import { render, screen } from '@testing-library/react';
import { MemoryRouter, Outlet, Route, Routes } from 'react-router-dom';
import GameDetailPage from '../GameDetailPage';
import {
  addToCollection,
  createComment,
  deleteComment,
  getGameComments,
  getLocalGame,
  getLocalGameByMobyGameId,
  getLocalGameBySlug,
  getMyCollection,
  getReactionSummary,
  getZeldaCatalogGameByMobyGameId,
  getZeldaCatalogGameBySlug,
  importMobyGame,
  setReaction,
  updateComment,
} from '../../../api/client';

vi.mock('../../../api/client', () => ({
  addToCollection: vi.fn(),
  createComment: vi.fn(),
  deleteComment: vi.fn(),
  getGameComments: vi.fn(),
  getLocalGame: vi.fn(),
  getLocalGameByMobyGameId: vi.fn(),
  getLocalGameBySlug: vi.fn(),
  getMyCollection: vi.fn(),
  getReactionSummary: vi.fn(),
  getZeldaCatalogGameByMobyGameId: vi.fn(),
  getZeldaCatalogGameBySlug: vi.fn(),
  importMobyGame: vi.fn(),
  setReaction: vi.fn(),
  updateComment: vi.fn(),
}));

function Layout() {
  return <Outlet context={{ currentUser: null, sendToLogin: vi.fn() }} />;
}

function renderGameDetailPage(route = '/games/ocarina-of-time') {
  return render(
    <MemoryRouter initialEntries={[route]}>
      <Routes>
        <Route element={<Layout />}>
          <Route path="/games/:gameSlug" element={<GameDetailPage />} />
          <Route path="/games/moby/:mobyGameId" element={<GameDetailPage />} />
          <Route path="/games/local/:localGameId" element={<GameDetailPage />} />
        </Route>
      </Routes>
    </MemoryRouter>,
  );
}

describe('GameDetailPage', () => {
  beforeEach(() => {
    vi.resetAllMocks();
    addToCollection.mockResolvedValue({});
    createComment.mockResolvedValue({});
    deleteComment.mockResolvedValue({});
    getGameComments.mockResolvedValue([]);
    getLocalGame.mockResolvedValue({});
    getLocalGameByMobyGameId.mockResolvedValue({});
    getLocalGameBySlug.mockResolvedValue({});
    getMyCollection.mockResolvedValue([]);
    getReactionSummary.mockResolvedValue({ likes: 0, dislikes: 0 });
    getZeldaCatalogGameByMobyGameId.mockResolvedValue({});
    getZeldaCatalogGameBySlug.mockResolvedValue({});
    importMobyGame.mockResolvedValue({});
    setReaction.mockResolvedValue({});
    updateComment.mockResolvedValue({});
  });

  it('surfaces the original catalog load error instead of crashing on the detail route', async () => {
    getZeldaCatalogGameBySlug.mockRejectedValue(new Error('Catalog is down'));

    renderGameDetailPage('/games/ocarina-of-time');

    expect(await screen.findByRole('alert')).toHaveTextContent('Catalog is down');
  });
});