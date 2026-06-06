namespace HyruleArchives.DTOs;

// I use this DTO when I need to return one persisted reaction in an API-safe shape.

// This DTO represents one saved reaction row owned by one user for one game.
public record ReactionDto(
    Guid Id,
    Guid GameId,
    Guid AppUserId,
    string Value,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);