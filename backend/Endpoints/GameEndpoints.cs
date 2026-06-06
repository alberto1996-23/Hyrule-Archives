using HyruleArchives.Data;
using HyruleArchives.DTOs;
using HyruleArchives.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HyruleArchives.Endpoints;
// I use this endpoint module to expose the core game lookup routes and import bridge helpers.

public static class GameEndpoints
{
    public static RouteGroupBuilder MapGameEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/games")
            .WithTags("Games");

        static GameDetailDto MapGameDetailDto(HyruleArchives.Models.Game game) => new(
            game.Id,
            game.Title,
            game.Slug,
            game.Summary,
            game.ImageUrl,
            game.Releases
                .OrderBy(release => release.ReleaseDate)
                .Select(release => new GameReleaseDto(
                    release.Id,
                    release.Platform?.Name ?? "Unknown Platform",
                    release.Region?.Name ?? "Unknown Region",
                    release.Region?.Code ?? "UNKNOWN",
                    release.ReleaseDate,
                    release.Source
                ))
                .ToList()
        );

        group.MapGet("/", async (
            string? search,
            Guid? platformId,
            Guid? regionId,
            AppDbContext dbContext) =>
        {
            var query = dbContext.Games
                .Include(game => game.Releases)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var trimmedSearch = search.Trim();
                query = query.Where(game =>
                    game.Title.Contains(trimmedSearch) ||
                    (game.Summary != null && game.Summary.Contains(trimmedSearch)));
            }

            if (platformId.HasValue)
            {
                query = query.Where(game =>
                    game.Releases.Any(release => release.PlatformId == platformId.Value));
            }

            if (regionId.HasValue)
            {
                query = query.Where(game =>
                    game.Releases.Any(release => release.RegionId == regionId.Value));
            }

            var games = await query
                .OrderBy(game => game.Title)
                .Select(game => new GameSummaryDto(
                    game.Id,
                    game.Title,
                    game.Slug,
                    game.Summary,
                    game.ImageUrl,
                    game.Releases.Count
                ))
                .ToListAsync();

            return Results.Ok(games);
        })
        .WithName("GetGames")
        .WithSummary("Gets all Zelda games in the archive.");

        group.MapGet("/by-slug/{slug}", async (string slug, AppDbContext dbContext) =>
        {
            var game = await dbContext.Games
                .Include(game => game.Releases)
                    .ThenInclude(release => release.Platform)
                .Include(game => game.Releases)
                    .ThenInclude(release => release.Region)
                .FirstOrDefaultAsync(game => game.Slug == slug);

            if (game is null)
            {
                return ApiErrors.NotFound("Game was not found.");
            }

            return Results.Ok(MapGameDetailDto(game));
        })
        .WithName("GetGameBySlug")
        .WithSummary("Gets one Zelda game by slug with release information.");

        group.MapGet("/by-mobygames-id/{mobyGameId:int}", async (int mobyGameId, AppDbContext dbContext) =>
        {
            var externalApiId = $"mobygames-{mobyGameId}";

            var game = await dbContext.Games
                .Include(game => game.Releases)
                    .ThenInclude(release => release.Platform)
                .Include(game => game.Releases)
                    .ThenInclude(release => release.Region)
                .FirstOrDefaultAsync(game => game.ExternalApiId == externalApiId);

            if (game is null)
            {
                return ApiErrors.NotFound("Game was not found.");
            }

            return Results.Ok(MapGameDetailDto(game));
        })
        .WithName("GetGameByMobyGamesId")
        .WithSummary("Gets one Zelda game by MobyGames id with release information.");

        group.MapGet("/{id:guid}", async (Guid id, AppDbContext dbContext) =>
        {
            var game = await dbContext.Games
                .Include(game => game.Releases)
                    .ThenInclude(release => release.Platform)
                .Include(game => game.Releases)
                    .ThenInclude(release => release.Region)
                .FirstOrDefaultAsync(game => game.Id == id);

            if (game is null)
            {
                return ApiErrors.NotFound("Game was not found.");
            }

            return Results.Ok(MapGameDetailDto(game));
        })
        .WithName("GetGameById")
        .WithSummary("Gets one Zelda game with release information.");

        return group;
    }
}