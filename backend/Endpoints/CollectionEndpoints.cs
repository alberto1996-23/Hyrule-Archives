using HyruleArchives.Data;
using HyruleArchives.DTOs;
using HyruleArchives.Models;
using HyruleArchives.Services;
using Microsoft.EntityFrameworkCore;

namespace HyruleArchives.Endpoints;

public static class CollectionEndpoints
{
    public static RouteGroupBuilder MapCollectionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/me/collection")
            .WithTags("My Collection")
            .RequireAuthorization();

        group.MapGet("/", async (
            HttpContext httpContext,
            AppDbContext dbContext,
            AuthUserService authUserService) =>
        {
            var user = await authUserService.GetOrCreateCurrentUserAsync(httpContext.User);

            if (user is null)
            {
                return Results.Unauthorized();
            }

            var items = await dbContext.CollectionItems
                .Include(item => item.Game)
                .Where(item => item.AppUserId == user.Id)
                .OrderByDescending(item => item.CreatedAt)
                .Select(item => new CollectionItemDto(
                    item.Id,
                    item.GameId,
                    item.Game!.Title,
                    item.Game.Slug,
                    item.Game.ImageUrl,
                    item.Status,
                    item.CreatedAt
                ))
                .ToListAsync();

            return Results.Ok(items);
        })
        .WithName("GetMyCollection")
        .WithSummary("Gets the signed-in user's saved Zelda games.");

        group.MapPost("/", async (
            AddCollectionItemRequest request,
            HttpContext httpContext,
            AppDbContext dbContext,
            AuthUserService authUserService) =>
        {
            var user = await authUserService.GetOrCreateCurrentUserAsync(httpContext.User);

            if (user is null)
            {
                return Results.Unauthorized();
            }

            var gameExists = await dbContext.Games
                .AnyAsync(game => game.Id == request.GameId);

            if (!gameExists)
            {
                return Results.NotFound(new
                {
                    message = "Game was not found."
                });
            }

            var alreadySaved = await dbContext.CollectionItems
                .AnyAsync(item =>
                    item.AppUserId == user.Id &&
                    item.GameId == request.GameId);

            if (alreadySaved)
            {
                return Results.Conflict(new
                {
                    message = "This game is already in your collection."
                });
            }

            var collectionItem = new CollectionItem
            {
                Id = Guid.NewGuid(),
                AppUserId = user.Id,
                GameId = request.GameId,
                Status = "Saved",
                CreatedAt = DateTimeOffset.UtcNow
            };

            dbContext.CollectionItems.Add(collectionItem);
            await dbContext.SaveChangesAsync();

            return Results.Created($"/api/me/collection/{collectionItem.Id}", new
            {
                collectionItem.Id,
                collectionItem.GameId,
                collectionItem.Status,
                collectionItem.CreatedAt
            });
        })
        .WithName("AddGameToMyCollection")
        .WithSummary("Adds a Zelda game to the signed-in user's collection.");

        return group;
    }
}