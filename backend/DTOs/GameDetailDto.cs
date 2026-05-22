namespace HyruleArchives.DTOs;

public record GameDetailDto(
    Guid Id,
    string Title,
    string Slug,
    string? Summary,
    string? ImageUrl,
    List<GameReleaseDto> Releases
);

public record GameReleaseDto(
    Guid Id,
    string PlatformName,
    string RegionName,
    string RegionCode,
    DateTime ReleaseDate,
    string Source
);