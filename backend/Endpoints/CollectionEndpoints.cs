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
        // Everything in this group starts with /api/me/collection.
        //
        // RequireAuthorization() protects every endpoint in this group.
        // If the user is not logged in, ASP.NET Core returns 401 Unauthorized.
        var group = app.MapGroup("/api/me/collection")
            .WithTags("My Collection")
            .RequireAuthorization();

        // GET /api/me/collection
        //
        // READ operation:
        // Gets only the signed-in user's collection items.
        group.MapGet("/", async (
            HttpContext httpContext,
            AppDbContext dbContext,
            AuthUserService authUserService) =>
        {
            // Find or create the current AppUser based on the Google login cookie.
            var user = await authUserService.GetOrCreateCurrentUserAsync(httpContext.User);

            // If no authenticated user exists, return 401.
            if (user is null)
            {
                return Results.Unauthorized();
            }

            // Get only the collection items owned by the signed-in user.
            // Include(item => item.Game) lets us access game title, slug, and image.
            var collectionItems = await dbContext.CollectionItems
                .Include(item => item.Game)
                .Where(item => item.AppUserId == user.Id)
                .ToListAsync();
            
            // Only load collection items that belong to this signed-in user.
            // This is the user isolation rule.
            var items = collectionItems
                .OrderByDescending(item => item.CreatedAt)
                .Select(item => new CollectionItemDto(
                    item.Id,
                    item.GameId,
                    item.Game!.Title,
                    item.Game.Slug,
                    item.Game.ImageUrl,
                    item.Status,
                    item.CreatedAt,
                    item.IsFavorite,
                    item.FavoritedAt,
                    item.UpdatedAt
                ))
                .ToList();

            return Results.Ok(items);
        })
        .WithName("GetMyCollection")
        .WithSummary("Gets the signed-in user's saved Zelda games.");

        // POST /api/me/collection
        //
        // CREATE operation:
        // Adds a game to the signed-in user's collection.
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

            // Make sure the requested game actually exists.
            var gameExists = await dbContext.Games
                .AnyAsync(game => game.Id == request.GameId);

            if (!gameExists)
            {
                return Results.NotFound(new
                {
                    message = "Game was not found."
                });
            }

            // Prevent the same user from saving the same game twice.
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

            // Create a new user-owned collection record.
            // Notice: AppUserId comes from the logged-in user, not from the frontend.
            // That is important for security.
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
                collectionItem.CreatedAt,
                collectionItem.IsFavorite,
                collectionItem.FavoritedAt,
                collectionItem.UpdatedAt
            });
        })
        .WithName("AddGameToMyCollection")
        .WithSummary("Adds a Zelda game to the signed-in user's collection.");

        // PUT /api/me/collection/{id}
        //
        // UPDATE operation:
        // Changes the status of a saved game.
        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateCollectionItemRequest request,
            HttpContext httpContext,
            AppDbContext dbContext,
            AuthUserService authUserService) =>
        {
            var user = await authUserService.GetOrCreateCurrentUserAsync(httpContext.User);

            if (user is null)
            {
                return Results.Unauthorized();
            }

            // Only search for the item if it belongs to the signed-in user.
            // This prevents User A from updating User B's collection item.
            var item = await dbContext.CollectionItems
                .FirstOrDefaultAsync(collectionItem =>
                    collectionItem.Id == id &&
                    collectionItem.AppUserId == user.Id);

            if (item is null)
            {
                return Results.NotFound(new
                {
                    message = "Collection item was not found for this user."
                });
            }

            // Keep the allowed statuses simple and controlled.
            var allowedStatuses = new[] { "Saved", "Playing", "Completed", "Dropped" };

            if (!allowedStatuses.Contains(request.Status))
            {
                return Results.BadRequest(new
                {
                    message = "Status must be Saved, Playing, Completed, or Dropped."
                });
            }

            item.Status = request.Status;

            await dbContext.SaveChangesAsync();

            return Results.Ok(new
            {
                item.Id,
                item.GameId,
                item.Status,
                item.CreatedAt,
                item.IsFavorite,
                item.FavoritedAt,
                item.UpdatedAt
            });
        })
        .WithName("UpdateCollectionItem")
        .WithSummary("Updates the status of a saved game in the signed-in user's collection.");

        // DELETE /api/me/collection/{id}
        //
        // DELETE operation:
        // Removes a game from the signed-in user's collection.
        group.MapDelete("/{id:guid}", async (
            Guid id,
            HttpContext httpContext,
            AppDbContext dbContext,
            AuthUserService authUserService) =>
        {
            var user = await authUserService.GetOrCreateCurrentUserAsync(httpContext.User);

            if (user is null)
            {
                return Results.Unauthorized();
            }

            // Again, only find the item if it belongs to the signed-in user.
            // This is what protects private user data.
            var item = await dbContext.CollectionItems
                .FirstOrDefaultAsync(collectionItem =>
                    collectionItem.Id == id &&
                    collectionItem.AppUserId == user.Id);

            if (item is null)
            {
                return Results.NotFound(new
                {
                    message = "Collection item was not found for this user."
                });
            }

            dbContext.CollectionItems.Remove(item);
            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        })
        .WithName("DeleteCollectionItem")
        .WithSummary("Removes a saved game from the signed-in user's collection.");

        return group;
    }
}