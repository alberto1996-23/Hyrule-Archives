namespace HyruleArchives.DTOs;

public record ReactionDto(
    Guid Id,
    Guid GameId,
    Guid AppUserId,
    string Value,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);