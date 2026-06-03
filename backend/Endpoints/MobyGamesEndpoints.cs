using HyruleArchives.Infrastructure;
using HyruleArchives.Services;

namespace HyruleArchives.Endpoints;

public static class MobyGamesEndpoints
{
    public static RouteGroupBuilder MapMobyGamesEndpoints(this IEndpointRouteBuilder app)
    {
        // This group starts with /api/mobygames.
        //
        // It is public because visitors should be able to see third-party
        // Zelda metadata without being logged in.
        var group = app.MapGroup("/api/mobygames")
            .WithTags("MobyGames External API");

        // GET /api/mobygames/zelda
        //
        // This endpoint calls MobyGames through the backend.
        // The frontend never sees the API key.
        group.MapGet("/zelda", async (MobyGamesService mobyGamesService) =>
        {
            try
            {
                var games = await mobyGamesService.SearchZeldaGamesAsync();

                return Results.Ok(games);
            }
            catch (InvalidOperationException error)
            {
                return ApiErrors.InternalServerError(error.Message, "Unable to load MobyGames data.");
            }
        })
        .WithName("GetMobyGamesZeldaData")
        .WithSummary("Gets Zelda game data from the MobyGames API.");

        // POST /api/mobygames/import/{mobyGameId}
        //
        // This endpoint bridges third-party data and local community features.
        // Once imported, the returned local game id can be used with comments,
        // reactions, favorites, collections, and trend endpoints.
        group.MapPost("/import/{mobyGameId:int}", async (
            int mobyGameId,
            MobyGamesService mobyGamesService,
            GameService gameService) =>
        {
            try
            {
                // React StrictMode intentionally re-runs effects in development.
                // Reusing an already-imported local game avoids duplicate upstream API calls
                // and prevents rate-limit-related failures when a user opens the detail page.
                var existingImport = await gameService.GetImportResultIfExistsAsync(mobyGameId);

                if (existingImport is not null)
                {
                    return Results.Ok(existingImport);
                }

                var mobyGame = await mobyGamesService.GetGameByIdAsync(mobyGameId);

                if (mobyGame is null)
                {
                    return ApiErrors.NotFound("MobyGames did not return a game for that id.");
                }

                var result = await gameService.UpsertFromMobyGamesAsync(mobyGame);

                return Results.Ok(result);
            }
            catch (InvalidOperationException error)
            {
                return ApiErrors.InternalServerError(error.Message, "Unable to import MobyGames data.");
            }
        })
        .WithName("ImportMobyGamesGame")
        .WithSummary("Imports or updates one MobyGames game into the local Games table for community interactions.");

        return group;
    }
}