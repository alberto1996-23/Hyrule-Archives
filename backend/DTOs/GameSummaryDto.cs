namespace HyruleArchives.DTOs;

public record GameSummaryDto(
    Guid Id,
    string Title,
    string Slug,
    string? Summary,
    string? ImageUrl,
    int ReleaseCount
);