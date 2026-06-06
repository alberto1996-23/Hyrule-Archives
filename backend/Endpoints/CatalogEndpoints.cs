using HyruleArchives.Infrastructure;
using HyruleArchives.Services;

namespace HyruleArchives.Endpoints;
// I use this endpoint module to serve the curated Zelda catalog over simple read-only routes.

public static class CatalogEndpoints
{
    public static RouteGroupBuilder MapCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/catalog")
            .WithTags("Catalog");

        group.MapGet("/zelda", async (ZeldaCatalogService catalogService) =>
        {
            var games = await catalogService.GetAllGamesAsync();
            return Results.Ok(games);
        })
        .WithName("GetZeldaCatalog")
        .WithSummary("Gets the curated Legend of Zelda catalog with release availability metadata.");

        group.MapGet("/zelda/{slug}", async (string slug, ZeldaCatalogService catalogService) =>
        {
            var game = await catalogService.GetBySlugAsync(slug);
            return game is null
                ? ApiErrors.NotFound("Catalog game was not found.")
                : Results.Ok(game);
        })
        .WithName("GetZeldaCatalogGameBySlug")
        .WithSummary("Gets one curated Legend of Zelda catalog entry by slug.");

        group.MapGet("/zelda/by-mobygames-id/{mobyGameId:int}", async (int mobyGameId, ZeldaCatalogService catalogService) =>
        {
            var game = await catalogService.GetByMobyGamesIdAsync(mobyGameId);
            return game is null
                ? ApiErrors.NotFound("Catalog game was not found.")
                : Results.Ok(game);
        })
        .WithName("GetZeldaCatalogGameByMobyGamesId")
        .WithSummary("Gets one curated Legend of Zelda catalog entry by MobyGames id.");

        return group;
    }
}
