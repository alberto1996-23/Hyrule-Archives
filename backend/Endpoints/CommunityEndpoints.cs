using HyruleArchives.Data;
using HyruleArchives.DTOs;
using Microsoft.EntityFrameworkCore;

namespace HyruleArchives.Endpoints;
// I use this endpoint module to build the community dashboard response from aggregate queries.

// Community endpoints compute aggregate data that makes the site feel alive.
public static class CommunityEndpoints
{
    public static RouteGroupBuilder MapCommunityEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/community")
            .WithTags("Community");

        group.MapGet("/highlights", async (AppDbContext dbContext) =>
        {
            // Each game gets a bundle of metrics that can later be sorted in different ways.
            var metrics = await dbContext.Games
                // Hide large-scale simulated seed rows from public trend sections so the
                // testing UI highlights recognizable Zelda titles instead of fake records.
                .Where(game => game.ExternalApiId == null || !game.ExternalApiId.StartsWith("seed-"))
                .Select(game => new CommunityGameMetricDto(
                    game.Id,
                    game.Title,
                    game.Slug,
                    game.ImageUrl,
                    game.CollectionItems.Count(item => item.IsFavorite),
                    game.Comments.Count(comment => !comment.IsDeleted),
                    game.Reactions.Count(reaction => reaction.Value == "Like"),
                    game.Reactions.Count(reaction => reaction.Value == "Dislike"),
                    // Keep the score aligned with the ranking logic shown on the trends page.
                    // Net approval gets the strongest weight so extra comments or favorites
                    // cannot fully cancel out a bad like/dislike balance.
                    ((game.Reactions.Count(reaction => reaction.Value == "Like")
                    - game.Reactions.Count(reaction => reaction.Value == "Dislike")) * 10000)
                    + (game.Reactions.Count(reaction => reaction.Value == "Like") * 100)
                    + (game.Comments.Count(comment => !comment.IsDeleted) * 10)
                    + game.CollectionItems.Count(item => item.IsFavorite)
                ))
                .ToListAsync();

            // Ignore games that have no community signal at all.
            // This keeps the trend page focused on titles that users have actually interacted with.
            var activeMetrics = metrics
                .Where(metric =>
                    metric.FavoriteCount > 0 ||
                    metric.CommentCount > 0 ||
                    metric.LikeCount > 0 ||
                    metric.DislikeCount > 0)
                .ToList();

            // The trending leaderboard should prioritize approval first.
            // 1. Net likes (likes minus dislikes) comes before everything else so games with
            //    heavier dislike pressure fall below games with stronger positive support.
            // 2. Raw likes break ties next.
            // 3. Comments and favorites help separate equally liked games after that.
            // This matches the requested behavior where dislikes can drag a game down even if
            // it has many comments.
            var response = new CommunityHighlightsDto(
                activeMetrics
                    .OrderByDescending(metric => metric.LikeCount - metric.DislikeCount)
                    .ThenByDescending(metric => metric.LikeCount)
                    .ThenByDescending(metric => metric.CommentCount)
                    .ThenByDescending(metric => metric.FavoriteCount)
                    .ThenBy(metric => metric.DislikeCount)
                    .ThenBy(metric => metric.Title)
                    .Take(5)
                    .ToList(),
                activeMetrics
                    .OrderByDescending(metric => metric.FavoriteCount)
                    .ThenByDescending(metric => metric.LikeCount)
                    .ThenByDescending(metric => metric.CommentCount)
                    .ThenBy(metric => metric.Title)
                    .Take(5)
                    .ToList(),
                activeMetrics
                    .OrderByDescending(metric => metric.CommentCount)
                    .ThenByDescending(metric => metric.LikeCount)
                    .ThenByDescending(metric => metric.FavoriteCount)
                    .ThenBy(metric => metric.Title)
                    .Take(5)
                    .ToList(),
                activeMetrics
                    .OrderByDescending(metric => metric.LikeCount)
                    .ThenBy(metric => metric.DislikeCount)
                    .ThenByDescending(metric => metric.CommentCount)
                    .ThenByDescending(metric => metric.FavoriteCount)
                    .ThenBy(metric => metric.Title)
                    .Take(5)
                    .ToList()
            );

            return Results.Ok(response);
        })
        .WithName("GetCommunityHighlights")
        .WithSummary("Gets public community highlights such as trending, most favorited, most commented, and most liked games.");

        return group;
    }
}