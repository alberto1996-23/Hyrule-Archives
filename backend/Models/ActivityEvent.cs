using System.ComponentModel.DataAnnotations;

namespace HyruleArchives.Models;

// I use this model to log a user action that can later show up in recent activity feeds.

public class ActivityEvent
{
    public Guid Id { get; set; }

    // The user who performed the action.
    public Guid AppUserId { get; set; }

    public AppUser? AppUser { get; set; }

    // Some activity is attached to a specific game.
    // Example: liked Ocarina of Time.
    //
    // This is nullable because some events may be general later,
    // such as "user joined The Hyrule Archives."
    public Guid? GameId { get; set; }

    public Game? Game { get; set; }

    // Examples:
    // CommentCreated
    // CommentUpdated
    // CommentDeleted
    // ReactionLiked
    // ReactionDisliked
    // CollectionAdded
    // CollectionUpdated
    // CollectionRemoved
    // FavoriteAdded
    // FavoriteRemoved
    [Required]
    [MaxLength(100)]
    public string Type { get; set; } = string.Empty;

    // This is the user-facing text that can be shown in activity feeds.
    // Example: "Hyrule Fan liked The Legend of Zelda: Ocarina of Time."
    [Required]
    [MaxLength(500)]
    public string Message { get; set; } = string.Empty;

    // Public means it can appear in trends/recent community activity.
    // Private means it only appears in the user's own activity feed.
    public bool IsPublic { get; set; } = true;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}