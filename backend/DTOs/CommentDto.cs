namespace HyruleArchives.DTOs;

public record CommentDto(
    Guid Id,
    Guid GameId,
    Guid AppUserId,
    string? DisplayName,
    string Body,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);