namespace HyruleArchives.DTOs;

// RegionDto is the read-only API shape for release region metadata.
public record RegionDto(
    Guid Id,
    string Name,
    string Code
);