using HyruleArchives.Data;
using HyruleArchives.DTOs;
using HyruleArchives.Infrastructure;
using HyruleArchives.Models;
using HyruleArchives.Services;
using Microsoft.EntityFrameworkCore;

namespace HyruleArchives.Endpoints;
// I use this endpoint module to handle likes, dislikes, and reaction summary routes.

public static class ReactionEndpoints
{
    public static RouteGroupBuilder MapReactionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api")
            .WithTags("Reactions");

        // PUBLIC:
        // Anyone can see reaction totals for a game.
        group.MapGet("/games/{gameId:guid}/reactions/summary", async (
            Guid gameId,
            AppDbContext dbContext) =>
        {
            var likes = await dbContext.Reactions
                .CountAsync(reaction => reaction.GameId == gameId && reaction.Value == "Like");

            var dislikes = await dbContext.Reactions
                .CountAsync(reaction => reaction.GameId == gameId && reaction.Value == "Dislike");

            return Results.Ok(new GameReactionSummaryDto(gameId, likes, dislikes));
        })
        .WithName("GetGameReactionSummary")
        .WithSummary("Gets like/dislike totals for a game.");

        // PROTECTED:
        // Signed-in users can like or dislike a game.
        group.MapPut("/games/{gameId:guid}/reaction", async (
            Guid gameId,
            SetReactionRequest request,
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

            if (request.Value != "Like" && request.Value != "Dislike")
            {
                return ApiErrors.BadRequest("Reaction must be Like or Dislike.");
            }

            var reaction = await dbContext.Reactions
                .FirstOrDefaultAsync(reaction =>
                    reaction.AppUserId == user.Id &&
                    reaction.GameId == gameId);

            if (reaction is null)
            {
                reaction = new Reaction
                {
                    Id = Guid.NewGuid(),
                    AppUserId = user.Id,
                    GameId = gameId,
                    Value = request.Value,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow
                };

                dbContext.Reactions.Add(reaction);
            }
            else
            {
                reaction.Value = request.Value;
                reaction.UpdatedAt = DateTimeOffset.UtcNow;
            }

            await dbContext.SaveChangesAsync();

            var activityType = request.Value == "Like"
                ? "ReactionLiked"
                : "ReactionDisliked";

            var actionText = request.Value == "Like"
                ? "liked"
                : "disliked";

            await activityEventService.CreateActivityEventAsync(
                user.Id,
                game.Id,
                activityType,
                $"{user.DisplayName ?? "A user"} {actionText} {game.Title}.",
                true
            );

            return Results.Ok(new ReactionDto(
                reaction.Id,
                reaction.GameId,
                reaction.AppUserId,
                reaction.Value,
                reaction.CreatedAt,
                reaction.UpdatedAt
            ));
        })
        .RequireAuthorization()
        .WithName("SetGameReaction")
        .WithSummary("Likes or dislikes a game for the signed-in user.");

        // PROTECTED:
        // Signed-in users can remove their reaction.
        group.MapDelete("/games/{gameId:guid}/reaction", async (
            Guid gameId,
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

            var reaction = await dbContext.Reactions
                .Include(reaction => reaction.Game)
                .FirstOrDefaultAsync(reaction =>
                    reaction.AppUserId == user.Id &&
                    reaction.GameId == gameId);

            if (reaction is null)
            {
                return ApiErrors.NotFound("Reaction was not found.");
            }

            dbContext.Reactions.Remove(reaction);
            await dbContext.SaveChangesAsync();

            await activityEventService.CreateActivityEventAsync(
                user.Id,
                gameId,
                "ReactionRemoved",
                $"{user.DisplayName ?? "A user"} removed a reaction from {reaction.Game?.Title ?? "a game"}.",
                false
            );

            return Results.NoContent();
        })
        .RequireAuthorization()
        .WithName("DeleteGameReaction")
        .WithSummary("Removes the signed-in user's reaction from a game.");

        return group;
    }
}