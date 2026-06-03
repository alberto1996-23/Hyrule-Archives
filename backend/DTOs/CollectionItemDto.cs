namespace HyruleArchives.DTOs;

public record CollectionItemDto(
    Guid Id,
    Guid GameId,
    string GameTitle,
    string GameSlug,
    string? ImageUrl,
    string Status,
    DateTimeOffset CreatedAt,
    bool IsFavorite,
    DateTimeOffset? FavoritedAt,
    DateTimeOffset UpdatedAt
);