using HyruleArchives.Data;
using HyruleArchives.DTOs;
using HyruleArchives.Infrastructure;
using HyruleArchives.Models;
using HyruleArchives.Services;
using Microsoft.EntityFrameworkCore;

namespace HyruleArchives.Endpoints;
// I use this endpoint module to keep comment CRUD routes together and easier to follow.

public static class CommentEndpoints
{
    public static RouteGroupBuilder MapCommentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api")
            .WithTags("Comments");

        // PUBLIC:
        // Anyone can view comments for a game.
        group.MapGet("/games/{gameId:guid}/comments", async (
            Guid gameId,
            AppDbContext dbContext) =>
        {
            // SQLite cannot ORDER BY DateTimeOffset directly.
            // So we load the comments first, then sort them in C#.
            var commentEntities = await dbContext.Comments
                .Include(comment => comment.AppUser)
                .Where(comment => comment.GameId == gameId && !comment.IsDeleted)
                .ToListAsync();

            var comments = commentEntities
                .OrderByDescending(comment => comment.CreatedAt)
                .Select(comment => new CommentDto(
                    comment.Id,
                    comment.GameId,
                    comment.AppUserId,
                    comment.AppUser?.DisplayName,
                    comment.Body,
                    comment.CreatedAt,
                    comment.UpdatedAt
                ))
                .ToList();

            return Results.Ok(comments);
        })
        .WithName("GetGameComments")
        .WithSummary("Gets public comments for a specific game.");

        // PROTECTED:
        // Signed-in users can post comments.
        group.MapPost("/games/{gameId:guid}/comments", async (
            Guid gameId,
            CreateCommentRequest request,
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

            var game = await dbContext.Games
                .FirstOrDefaultAsync(game => game.Id == gameId);

            if (game is null)
            {
                return ApiErrors.NotFound("Game was not found.");
            }

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                AppUserId = user.Id,
                GameId = game.Id,
                Body = request.Body.Trim(),
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            dbContext.Comments.Add(comment);
            await dbContext.SaveChangesAsync();

            await activityEventService.CreateActivityEventAsync(
                user.Id,
                game.Id,
                "CommentCreated",
                $"{user.DisplayName ?? "A user"} commented on {game.Title}.",
                true
            );

            var dto = new CommentDto(
                comment.Id,
                comment.GameId,
                comment.AppUserId,
                user.DisplayName,
                comment.Body,
                comment.CreatedAt,
                comment.UpdatedAt
            );

            return Results.Created($"/api/games/{gameId}/comments/{comment.Id}", dto);
        })
        .RequireAuthorization()
        .WithName("CreateGameComment")
        .WithSummary("Creates a comment for a specific game.");

        // PROTECTED:
        // Signed-in users can edit only their own comments.
        group.MapPut("/comments/{commentId:guid}", async (
            Guid commentId,
            UpdateCommentRequest request,
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

            var comment = await dbContext.Comments
                .Include(comment => comment.Game)
                .FirstOrDefaultAsync(comment => comment.Id == commentId);

            if (comment is null || comment.IsDeleted)
            {
                return ApiErrors.NotFound("Comment was not found.");
            }

            if (comment.AppUserId != user.Id)
            {
                return ApiErrors.Forbidden("You can only edit your own comments.");
            }

            comment.Body = request.Body.Trim();
            comment.UpdatedAt = DateTimeOffset.UtcNow;

            await dbContext.SaveChangesAsync();

            await activityEventService.CreateActivityEventAsync(
                user.Id,
                comment.GameId,
                "CommentUpdated",
                $"{user.DisplayName ?? "A user"} edited a comment on {comment.Game?.Title ?? "a game"}.",
                false
            );

            return Results.Ok(new CommentDto(
                comment.Id,
                comment.GameId,
                comment.AppUserId,
                user.DisplayName,
                comment.Body,
                comment.CreatedAt,
                comment.UpdatedAt
            ));
        })
        .RequireAuthorization()
        .WithName("UpdateComment")
        .WithSummary("Updates a comment owned by the signed-in user.");

        // PROTECTED:
        // Signed-in users can delete only their own comments.
        group.MapDelete("/comments/{commentId:guid}", async (
            Guid commentId,
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

            var comment = await dbContext.Comments
                .Include(comment => comment.Game)
                .FirstOrDefaultAsync(comment => comment.Id == commentId);

            if (comment is null || comment.IsDeleted)
            {
                return ApiErrors.NotFound("Comment was not found.");
            }

            if (comment.AppUserId != user.Id)
            {
                return ApiErrors.Forbidden("You can only delete your own comments.");
            }

            comment.IsDeleted = true;
            comment.DeletedAt = DateTimeOffset.UtcNow;
            comment.UpdatedAt = DateTimeOffset.UtcNow;

            await dbContext.SaveChangesAsync();

            await activityEventService.CreateActivityEventAsync(
                user.Id,
                comment.GameId,
                "CommentDeleted",
                $"{user.DisplayName ?? "A user"} deleted a comment on {comment.Game?.Title ?? "a game"}.",
                false
            );

            return Results.NoContent();
        })
        .RequireAuthorization()
        .WithName("DeleteComment")
        .WithSummary("Soft-deletes a comment owned by the signed-in user.");

        return group;
    }
}