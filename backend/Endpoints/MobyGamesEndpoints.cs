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
                return Results.Problem(
                    title: "Unable to load MobyGames data.",
                    detail: error.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        })
        .WithName("GetMobyGamesZeldaData")
        .WithSummary("Gets Zelda game data from the MobyGames API.");

        return group;
    }
}