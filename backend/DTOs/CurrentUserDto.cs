namespace HyruleArchives.DTOs;

// I use this DTO to send the lightweight signed-in profile data needed for the current session.

// CurrentUserDto is the minimal profile information the frontend needs after login.
public record CurrentUserDto(
    Guid Id,
    string? Email,
    string? DisplayName,
    bool IsSimulated,
    DateTimeOffset CreatedAt
);