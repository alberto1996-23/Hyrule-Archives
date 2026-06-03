namespace HyruleArchives.DTOs;

// This DTO powers list pages where only summary-level game information is needed.
public record GameSummaryDto(
    Guid Id,
    string Title,
    string Slug,
    string? Summary,
    string? ImageUrl,
    int ReleaseCount
);