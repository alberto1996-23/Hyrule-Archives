namespace HyruleArchives.DTOs;

// This DTO gives the frontend a lightweight reaction count snapshot for one game.
public record GameReactionSummaryDto(
    Guid GameId,
    int Likes,
    int Dislikes
);