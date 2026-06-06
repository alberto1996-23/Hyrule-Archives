namespace HyruleArchives.DTOs;

// I use this response DTO to hand the frontend the local game id after an import finishes.

// This response makes the import endpoint easy to use in Swagger.
// The returned local Guid can immediately be used with comments, reactions,
// favorites, collection items, and community trend endpoints.
public record MobyGameImportResultDto(
    Guid LocalGameId,
    string ExternalApiId,
    string Title,
    string Slug,
    bool WasCreated,
    int ReleasesImported,
    int PlatformsSeen
);