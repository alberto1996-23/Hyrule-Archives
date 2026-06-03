namespace HyruleArchives.DTOs;

// This DTO groups several leaderboard lists into one response for a dashboard-like screen.
public record CommunityHighlightsDto(
    List<CommunityGameMetricDto> TrendingGames,
    List<CommunityGameMetricDto> MostFavoritedGames,
    List<CommunityGameMetricDto> MostCommentedGames,
    List<CommunityGameMetricDto> MostLikedGames
);

// Each metric row contains the counts used to rank a game in community views.
public record CommunityGameMetricDto(
    Guid GameId,
    string Title,
    string Slug,
    string? ImageUrl,
    int FavoriteCount,
    int CommentCount,
    int LikeCount,
    int DislikeCount,
    int TrendingScore
);