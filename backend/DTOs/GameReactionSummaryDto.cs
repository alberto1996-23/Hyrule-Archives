namespace HyruleArchives.DTOs;

// I use this DTO to return fast reaction totals without sending every underlying reaction row.

// This DTO gives the frontend a lightweight reaction count snapshot for one game.
public record GameReactionSummaryDto(
    Guid GameId,
    int Likes,
    int Dislikes
);