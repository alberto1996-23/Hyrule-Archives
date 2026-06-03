using HyruleArchives.Data;
using HyruleArchives.DTOs;
using HyruleArchives.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HyruleArchives.Endpoints;

public static class GameEndpoints
{
    public static RouteGroupBuilder MapGameEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/games")
            .WithTags("Games");

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

            var dto = new GameDetailDto(
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

            return Results.Ok(dto);
        })
        .WithName("GetGameById")
        .WithSummary("Gets one Zelda game with release information.");

        return group;
    }
}