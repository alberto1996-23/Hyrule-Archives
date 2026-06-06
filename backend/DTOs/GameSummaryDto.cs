namespace HyruleArchives.DTOs;

// I use this DTO for list screens where only the top-level game metadata should cross the API boundary.

// This DTO powers list pages where only summary-level game information is needed.
public record GameSummaryDto(
    Guid Id,
    string Title,
    string Slug,
    string? Summary,
    string? ImageUrl,
    int ReleaseCount
);