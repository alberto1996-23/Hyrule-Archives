using HyruleArchives.Data;
using HyruleArchives.DTOs;
using Microsoft.EntityFrameworkCore;

namespace HyruleArchives.Endpoints;
// I use this endpoint module to return platform metadata to the frontend in a small read-only shape.

// Platform endpoints expose the consoles/platforms that Zelda games can appear on.
public static class PlatformEndpoints
{
    public static RouteGroupBuilder MapPlatformEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/platforms")
            .WithTags("Platforms");

        group.MapGet("/", async (AppDbContext dbContext) =>
        {
            // This is a simple read-only query that projects database rows into DTOs.
            var platforms = await dbContext.Platforms
                .OrderBy(platform => platform.Name)
                .Select(platform => new PlatformDto(
                    platform.Id,
                    platform.Name,
                    platform.Slug
                ))
                .ToListAsync();

            return Results.Ok(platforms);
        })
        .WithName("GetPlatforms")
        .WithSummary("Gets all platforms used by Zelda games in the archive.");

        return group;
    }
}