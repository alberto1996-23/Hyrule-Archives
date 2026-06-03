namespace HyruleArchives.DTOs;

// CurrentUserDto is the minimal profile information the frontend needs after login.
public record CurrentUserDto(
    Guid Id,
    string? Email,
    string? DisplayName,
    bool IsSimulated,
    DateTimeOffset CreatedAt
);