namespace HyruleArchives.DTOs;

// I use this DTO to expose normalized region metadata without leaking database entities.

// RegionDto is the read-only API shape for release region metadata.
public record RegionDto(
    Guid Id,
    string Name,
    string Code
);