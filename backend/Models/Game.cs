using System.ComponentModel.DataAnnotations;

namespace HyruleArchives.Models;

// I use this model as the main community-enabled game record inside the local database.

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
    
    public List<CollectionItem> CollectionItems { get; set; } = new();

    public List<Comment> Comments { get; set; } = new();

    public List<Reaction> Reactions { get; set; } = new();

    public List<ActivityEvent> ActivityEvents { get; set; } = new();
}