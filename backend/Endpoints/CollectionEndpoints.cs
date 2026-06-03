using HyruleArchives.Data;
using HyruleArchives.DTOs;
using HyruleArchives.Infrastructure;
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
                return ApiErrors.Unauthorized();
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

        group.MapGet("/favorites", async (
            HttpContext httpContext,
            AppDbContext dbContext,
            AuthUserService authUserService) =>
        {
            var user = await authUserService.GetOrCreateCurrentUserAsync(httpContext.User);

            if (user is null)
            {
                return ApiErrors.Unauthorized();
            }

            var favoriteItems = await dbContext.CollectionItems
                .Include(item => item.Game)
                .Where(item => item.AppUserId == user.Id && item.IsFavorite)
                .ToListAsync();

            var items = favoriteItems
                .OrderByDescending(item => item.FavoritedAt ?? item.UpdatedAt)
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
        .WithName("GetMyFavoriteGames")
        .WithSummary("Gets the signed-in user's favorite Zelda games.");

        // POST /api/me/collection
        //
        // CREATE operation:
        // Adds a game to the signed-in user's collection.
        group.MapPost("/", async (
            AddCollectionItemRequest request,
            HttpContext httpContext,
            AppDbContext dbContext,
            AuthUserService authUserService,
            ActivityEventService activityEventService) =>
        {
            var user = await authUserService.GetOrCreateCurrentUserAsync(httpContext.User);

            if (user is null)
            {
                return ApiErrors.Unauthorized();
            }

            // Make sure the requested game actually exists.
            var game = await dbContext.Games
                .FirstOrDefaultAsync(existingGame => existingGame.Id == request.GameId);

            if (game is null)
            {
                return ApiErrors.NotFound("Game was not found.");
            }

            // Prevent the same user from saving the same game twice.
            var alreadySaved = await dbContext.CollectionItems
                .AnyAsync(item =>
                    item.AppUserId == user.Id &&
                    item.GameId == request.GameId);

            if (alreadySaved)
            {
                return ApiErrors.Conflict("This game is already in your collection.");
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
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            dbContext.CollectionItems.Add(collectionItem);
            await dbContext.SaveChangesAsync();

            await activityEventService.CreateActivityEventAsync(
                user.Id,
                game.Id,
                "CollectionAdded",
                $"{user.DisplayName ?? "A user"} added {game.Title} to their collection.",
                false
            );

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
            AuthUserService authUserService,
            ActivityEventService activityEventService) =>
        {
            var user = await authUserService.GetOrCreateCurrentUserAsync(httpContext.User);

            if (user is null)
            {
                return ApiErrors.Unauthorized();
            }

            // Only search for the item if it belongs to the signed-in user.
            // This prevents User A from updating User B's collection item.
            var item = await dbContext.CollectionItems
                .Include(collectionItem => collectionItem.Game)
                .FirstOrDefaultAsync(collectionItem =>
                    collectionItem.Id == id &&
                    collectionItem.AppUserId == user.Id);

            if (item is null)
            {
                return ApiErrors.NotFound("Collection item was not found for this user.");
            }

            // Keep the allowed statuses simple and controlled.
            var allowedStatuses = new[] { "Saved", "Playing", "Completed", "Dropped" };

            if (!allowedStatuses.Contains(request.Status))
            {
                return ApiErrors.BadRequest("Status must be Saved, Playing, Completed, or Dropped.");
            }

            item.Status = request.Status;
            item.UpdatedAt = DateTimeOffset.UtcNow;

            await dbContext.SaveChangesAsync();

            await activityEventService.CreateActivityEventAsync(
                user.Id,
                item.GameId,
                "CollectionUpdated",
                $"{user.DisplayName ?? "A user"} updated {item.Game?.Title ?? "a saved game"} to {item.Status}.",
                false
            );

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
            AuthUserService authUserService,
            ActivityEventService activityEventService) =>
        {
            var user = await authUserService.GetOrCreateCurrentUserAsync(httpContext.User);

            if (user is null)
            {
                return ApiErrors.Unauthorized();
            }

            // Again, only find the item if it belongs to the signed-in user.
            // This is what protects private user data.
            var item = await dbContext.CollectionItems
                .Include(collectionItem => collectionItem.Game)
                .FirstOrDefaultAsync(collectionItem =>
                    collectionItem.Id == id &&
                    collectionItem.AppUserId == user.Id);

            if (item is null)
            {
                return ApiErrors.NotFound("Collection item was not found for this user.");
            }

            var gameId = item.GameId;
            var gameTitle = item.Game?.Title ?? "a saved game";

            dbContext.CollectionItems.Remove(item);
            await dbContext.SaveChangesAsync();

            await activityEventService.CreateActivityEventAsync(
                user.Id,
                gameId,
                "CollectionRemoved",
                $"{user.DisplayName ?? "A user"} removed {gameTitle} from their collection.",
                false
            );

            return Results.NoContent();
        })
        .WithName("DeleteCollectionItem")
        .WithSummary("Removes a saved game from the signed-in user's collection.");

        group.MapPut("/{id:guid}/favorite", async (
            Guid id,
            HttpContext httpContext,
            AppDbContext dbContext,
            AuthUserService authUserService,
            ActivityEventService activityEventService) =>
        {
            var user = await authUserService.GetOrCreateCurrentUserAsync(httpContext.User);

            if (user is null)
            {
                return ApiErrors.Unauthorized();
            }

            var item = await dbContext.CollectionItems
                .Include(collectionItem => collectionItem.Game)
                .FirstOrDefaultAsync(collectionItem =>
                    collectionItem.Id == id &&
                    collectionItem.AppUserId == user.Id);

            if (item is null)
            {
                return ApiErrors.NotFound("Collection item was not found for this user.");
            }

            item.IsFavorite = true;
            item.FavoritedAt = DateTimeOffset.UtcNow;
            item.UpdatedAt = DateTimeOffset.UtcNow;

            await dbContext.SaveChangesAsync();

            await activityEventService.CreateActivityEventAsync(
                user.Id,
                item.GameId,
                "FavoriteAdded",
                $"{user.DisplayName ?? "A user"} favorited {item.Game?.Title ?? "a game"}.",
                true
            );

            return Results.Ok(new CollectionItemDto(
                item.Id,
                item.GameId,
                item.Game?.Title ?? "Unknown Game",
                item.Game?.Slug ?? string.Empty,
                item.Game?.ImageUrl,
                item.Status,
                item.CreatedAt,
                item.IsFavorite,
                item.FavoritedAt,
                item.UpdatedAt
            ));
        })
        .WithName("FavoriteCollectionItem")
        .WithSummary("Marks one of the signed-in user's saved games as a favorite.");

        group.MapDelete("/{id:guid}/favorite", async (
            Guid id,
            HttpContext httpContext,
            AppDbContext dbContext,
            AuthUserService authUserService,
            ActivityEventService activityEventService) =>
        {
            var user = await authUserService.GetOrCreateCurrentUserAsync(httpContext.User);

            if (user is null)
            {
                return ApiErrors.Unauthorized();
            }

            var item = await dbContext.CollectionItems
                .Include(collectionItem => collectionItem.Game)
                .FirstOrDefaultAsync(collectionItem =>
                    collectionItem.Id == id &&
                    collectionItem.AppUserId == user.Id);

            if (item is null)
            {
                return ApiErrors.NotFound("Collection item was not found for this user.");
            }

            item.IsFavorite = false;
            item.FavoritedAt = null;
            item.UpdatedAt = DateTimeOffset.UtcNow;

            await dbContext.SaveChangesAsync();

            await activityEventService.CreateActivityEventAsync(
                user.Id,
                item.GameId,
                "FavoriteRemoved",
                $"{user.DisplayName ?? "A user"} removed {item.Game?.Title ?? "a game"} from favorites.",
                false
            );

            return Results.Ok(new CollectionItemDto(
                item.Id,
                item.GameId,
                item.Game?.Title ?? "Unknown Game",
                item.Game?.Slug ?? string.Empty,
                item.Game?.ImageUrl,
                item.Status,
                item.CreatedAt,
                item.IsFavorite,
                item.FavoritedAt,
                item.UpdatedAt
            ));
        })
        .WithName("UnfavoriteCollectionItem")
        .WithSummary("Removes one of the signed-in user's saved games from favorites.");

        return group;
    }
}