namespace HyruleArchives.DTOs;

// I use this DTO file to define the safe activity payload that the frontend can render.

// This DTO is the public-safe version of an ActivityEvent.
//
// It is used for:
// - the logged-in dashboard activity feed
// - the public trends/recent activity feed
// - SignalR live updates
public record ActivityEventDto(
    Guid Id,
    Guid AppUserId,
    string? DisplayName,
    Guid? GameId,
    string? GameTitle,
    string Type,
    string Message,
    bool IsPublic,
    DateTimeOffset CreatedAt
);