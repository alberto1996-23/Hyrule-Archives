namespace HyruleArchives.DTOs;

// I use this DTO to return the comment information the UI needs without exposing EF entities directly.

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