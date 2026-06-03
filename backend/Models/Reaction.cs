using System.ComponentModel.DataAnnotations;

namespace HyruleArchives.Models;

public class Reaction
{
    public Guid Id { get; set; }

    // The user who reacted.
    public Guid AppUserId { get; set; }

    public AppUser? AppUser { get; set; }

    // The Zelda game that was liked or disliked.
    public Guid GameId { get; set; }

    public Game? Game { get; set; }

    // Keep this controlled.
    // Allowed values should be: Like or Dislike.
    [Required]
    [MaxLength(20)]
    public string Value { get; set; } = "Like";

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}