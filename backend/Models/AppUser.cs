using System.ComponentModel.DataAnnotations;

namespace HyruleArchives.Models;

public class AppUser
{
    public Guid Id { get; set; }

    [MaxLength(100)]
    public string OAuthProvider { get; set; } = string.Empty;

    [MaxLength(200)]
    public string OAuthSubjectId { get; set; } = string.Empty;

    [MaxLength(320)]
    public string? Email { get; set; }

    [MaxLength(150)]
    public string? DisplayName { get; set; }

    public bool IsSimulated { get; set; } = false;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}