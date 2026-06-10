import { useEffect, useMemo, useRef, useState } from 'react';
import { getZeldaCatalog } from '../../api/client';
import GameCard from './components/GameCard';
import './GamesPage.css';

// The games page loads the curated Zelda catalog from the backend.
export default function GamesPage() {
  const [games, setGames] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [visibleRows, setVisibleRows] = useState({});
  const rowRefs = useRef([]);

  const gameRows = useMemo(() => {
    const rows = [];

    for (let index = 0; index < games.length; index += 4) {
      rows.push(games.slice(index, index + 4));
    }

    return rows;
  }, [games]);

  useEffect(() => {
    let isMounted = true;

    async function loadCatalog() {
      try {
        const catalog = await getZeldaCatalog();

        if (isMounted) {
          setGames(catalog);
        }
      } catch (loadError) {
        if (isMounted) {
          setError(loadError.message);
        }
      } finally {
        if (isMounted) {
          setLoading(false);
        }
      }
    }

    loadCatalog();

    return () => {
      isMounted = false;
    };
  }, []);

  useEffect(() => {
    rowRefs.current = rowRefs.current.slice(0, gameRows.length);

    if (gameRows.length === 0) {
      return undefined;
    }

    const observer = new IntersectionObserver(
      (entries) => {
        entries.forEach((entry) => {
          if (!entry.isIntersecting) {
            return;
          }

          const rowIndex = Number(entry.target.getAttribute('data-row-index'));

          setVisibleRows((currentRows) => {
            if (currentRows[rowIndex]) {
              return currentRows;
            }

            return { ...currentRows, [rowIndex]: true };
          });

          observer.unobserve(entry.target);
        });
      },
      {
        threshold: 0.2,
        rootMargin: '0px 0px -10% 0px',
      },
    );

    rowRefs.current.forEach((rowElement) => {
      if (rowElement) {
        observer.observe(rowElement);
      }
    });

    return () => {
      observer.disconnect();
    };
  }, [gameRows]);

  return (
    <section className="games-page">
      <div className="games-page__header">
        <h1>Official Legend of Zelda games</h1>
      </div>

      {loading ? <p>Loading Zelda catalog...</p> : null}
      {error ? <p role="alert">{error}</p> : null}
      {!loading && !error && games.length > 0 ? (
        <div className="games-page__rows">
          {gameRows.map((row, rowIndex) => (
            <div
              key={`row-${row[0]?.slug ?? rowIndex}`}
              ref={(element) => {
                rowRefs.current[rowIndex] = element;
              }}
              className={`games-page__row ${visibleRows[rowIndex] ? 'games-page__row--visible' : ''}`}
              data-row-index={rowIndex}
            >
              {row.map((game, cardIndex) => (
                <GameCard key={game.slug} game={game} delayIndex={cardIndex} />
              ))}
            </div>
          ))}
        </div>
      ) : null}
      {!loading && !error && games.length === 0 ? <p>No Zelda games are currently configured.</p> : null}
    </section>
  );
}
