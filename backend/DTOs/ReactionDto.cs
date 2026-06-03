namespace HyruleArchives.DTOs;

// This DTO represents one saved reaction row owned by one user for one game.
public record ReactionDto(
    Guid Id,
    Guid GameId,
    Guid AppUserId,
    string Value,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);