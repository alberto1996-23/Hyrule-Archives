namespace HyruleArchives.DTOs;

// This DTO is the simplified version of MobyGames data
// that we want to send to the React frontend.
//
// We do not send the entire MobyGames API response because it is large.
// Instead, we keep the fields that help prove the third-party API is visible.
public record MobyGameDto(
    int MobyGameId,
    string Title,
    string? Description,
    double? MobyScore,
    string? MobyUrl,
    string? CoverImage,
    string? CoverThumbnail,
    string? ScreenshotImage,
    List<MobyPlatformDto> Platforms,
    string SourceCredit
);

// This represents one platform/release entry from MobyGames.
// Example: Nintendo Switch, first released 2017-03-03.
public record MobyPlatformDto(
    int PlatformId,
    string PlatformName,
    string? FirstReleaseDate
);