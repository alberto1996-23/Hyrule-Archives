using System.ComponentModel.DataAnnotations;

namespace HyruleArchives.Models;

public class Game
{
    public Guid Id { get; set; }

    [MaxLength(100)]
    public string? ExternalApiId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Slug { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Summary { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public List<GameRelease> Releases { get; set; } = new();
}