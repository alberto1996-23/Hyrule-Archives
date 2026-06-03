namespace HyruleArchives.DTOs;

public record GameReactionSummaryDto(
    Guid GameId,
    int Likes,
    int Dislikes
);