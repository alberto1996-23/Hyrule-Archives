namespace HyruleArchives.DTOs;

public record CurrentUserDto(
    Guid Id,
    string? Email,
    string? DisplayName,
    bool IsSimulated,
    DateTimeOffset CreatedAt
);