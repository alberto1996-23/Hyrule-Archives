using System.ComponentModel.DataAnnotations;

namespace HyruleArchives.Models;

// I use this join model to connect one user with one game in the personal collection feature.

public class CollectionItem
{
    public Guid Id { get; set; }

    // The user who owns this collection item.
    // This makes the record private/user-owned.
    public Guid AppUserId { get; set; }

    public AppUser? AppUser { get; set; }

    // The Zelda game saved in this user's collection.
    public Guid GameId { get; set; }

    public Game? Game { get; set; }

    // Tracks where the user is with this game.
    // Examples: Saved, Playing, Completed, Dropped.
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Saved";

    // This is how we implement the favorites system.
    // If true, this collection item appears in the user's Favorites area.
    public bool IsFavorite { get; set; } = false;

    // Optional timestamp for when the user marked this game as a favorite.
    // Null means the game is not currently favorited.
    public DateTimeOffset? FavoritedAt { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}