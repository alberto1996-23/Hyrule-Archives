using HyruleArchives.Data;
using HyruleArchives.DTOs;
using HyruleArchives.Models;
using Microsoft.EntityFrameworkCore;

namespace HyruleArchives.Services;

public class ZeldaCatalogService
{
    private readonly AppDbContext _dbContext;

    public ZeldaCatalogService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ZeldaCatalogGameDto>> GetAllGamesAsync()
    {
        var games = await LoadCatalogGamesQuery()
            .OrderBy(game => game.Title)
            .ToListAsync();

        return games.Select(MapGame).ToList();
    }

    public async Task<ZeldaCatalogGameDto?> GetBySlugAsync(string slug)
    {
        var game = await LoadCatalogGamesQuery()
            .FirstOrDefaultAsync(game => game.Slug == slug);

        return game is null ? null : MapGame(game);
    }

    public async Task<ZeldaCatalogGameDto?> GetByMobyGamesIdAsync(int mobyGameId)
    {
        var game = await LoadCatalogGamesQuery()
            .FirstOrDefaultAsync(game => game.MobyGameId == mobyGameId);

        return game is null ? null : MapGame(game);
    }

    private IQueryable<ZeldaCatalogGame> LoadCatalogGamesQuery() =>
        _dbContext.ZeldaCatalogGames
            .Include(game => game.AvailabilityEntries)
            .Include(game => game.CollectionAppearances);

    private static ZeldaCatalogGameDto MapGame(ZeldaCatalogGame game)
    {
        var primaryConsole = game.AvailabilityEntries
            .OrderBy(entry => entry.DisplayOrder)
            .First(entry => entry.Kind == ZeldaCatalogAvailabilityKind.PrimaryConsole);

        return new ZeldaCatalogGameDto(
            game.Slug,
            game.MobyGameId,
            game.Title,
            game.BoxArtPath,
            MapAvailability(primaryConsole),
            MapAvailabilityList(game, ZeldaCatalogAvailabilityKind.SecondaryConsole),
            MapAvailabilityList(game, ZeldaCatalogAvailabilityKind.DownloadedPort),
            MapAvailabilityList(game, ZeldaCatalogAvailabilityKind.SubscriptionService),
            game.CollectionAppearances
                .OrderBy(appearance => appearance.DisplayOrder)
                .Select(appearance => new ZeldaCatalogCollectionAppearanceDto(
                    appearance.Title,
                    appearance.BoxArtPath,
                    new ZeldaCatalogAvailabilityDto(
                        appearance.PlatformName,
                        appearance.PlatformImagePath,
                        appearance.Note)))
                .ToList());
    }

    private static List<ZeldaCatalogAvailabilityDto> MapAvailabilityList(
        ZeldaCatalogGame game,
        ZeldaCatalogAvailabilityKind kind) => game.AvailabilityEntries
            .Where(entry => entry.Kind == kind)
            .OrderBy(entry => entry.DisplayOrder)
            .Select(MapAvailability)
            .ToList();

    private static ZeldaCatalogAvailabilityDto MapAvailability(ZeldaCatalogAvailability availability) =>
        new(availability.Name, availability.ImagePath, availability.Note);
}
