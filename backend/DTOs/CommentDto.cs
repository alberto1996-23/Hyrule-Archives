namespace HyruleArchives.DTOs;

// CommentDto is returned for public game comments and user-owned comment updates.
public record CommentDto(
    Guid Id,
    Guid GameId,
    Guid AppUserId,
    string? DisplayName,
    string Body,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);