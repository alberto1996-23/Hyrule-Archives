using System.ComponentModel.DataAnnotations;

namespace HyruleArchives.Models;

public class Comment
{
    public Guid Id { get; set; }

    // The user who wrote the comment.
    public Guid AppUserId { get; set; }

    public AppUser? AppUser { get; set; }

    // The game this comment belongs to.
    public Guid GameId { get; set; }

    public Game? Game { get; set; }

    // The actual comment text.
    [Required]
    [MaxLength(2000)]
    public string Body { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    // Soft delete:
    // Instead of removing the row from the database, we hide it.
    // This helps preserve activity history.
    public bool IsDeleted { get; set; } = false;

    public DateTimeOffset? DeletedAt { get; set; }
}