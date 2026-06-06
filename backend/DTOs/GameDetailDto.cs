namespace HyruleArchives.DTOs;

// I use this DTO when the frontend needs the fuller nested game view instead of a simple list row.

// This DTO powers a detail page where the frontend needs nested release metadata.
public record GameDetailDto(
    Guid Id,
    string Title,
    string Slug,
    string? Summary,
    string? ImageUrl,
    List<GameReleaseDto> Releases
);

// Each release row tells the client where and when one version of the game launched.
public record GameReleaseDto(
    Guid Id,
    string PlatformName,
    string RegionName,
    string RegionCode,
    DateTime ReleaseDate,
    string Source
);