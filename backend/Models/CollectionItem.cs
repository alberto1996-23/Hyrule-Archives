using System.ComponentModel.DataAnnotations;

namespace HyruleArchives.Models;

public class CollectionItem
{
    public Guid Id { get; set; }

    public Guid AppUserId { get; set; }

    public AppUser? AppUser { get; set; }

    public Guid GameId { get; set; }

    public Game? Game { get; set; }

    [MaxLength(50)]
    public string Status { get; set; } = "Saved";

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}