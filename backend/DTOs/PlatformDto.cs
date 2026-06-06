namespace HyruleArchives.DTOs;

// I use this DTO to expose only the platform fields that list and detail pages actually need.

// DTOs are API-safe shapes returned to the client.
// This one exposes only the platform fields the frontend needs.
public record PlatformDto(
    Guid Id,
    string Name,
    string Slug
);