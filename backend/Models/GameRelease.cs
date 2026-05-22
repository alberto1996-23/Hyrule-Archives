using System.ComponentModel.DataAnnotations;

namespace HyruleArchives.Models;

public class GameRelease
{
    public Guid Id { get; set; }

    public Guid GameId { get; set; }

    public Game? Game { get; set; }

    public Guid PlatformId { get; set; }

    public Platform? Platform { get; set; }

    public Guid RegionId { get; set; }

    public Region? Region { get; set; }

    public DateTime ReleaseDate { get; set; }

    [MaxLength(100)]
    public string Source { get; set; } = "Seed Data";
}