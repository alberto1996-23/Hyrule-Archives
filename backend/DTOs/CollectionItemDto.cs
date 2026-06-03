namespace HyruleArchives.DTOs;

// This DTO is what the frontend uses to render a user's saved collection entries.
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