using System.ComponentModel.DataAnnotations;

namespace HyruleArchives.Models;

public class ZeldaCatalogAvailability
{
    public Guid Id { get; set; }

    public Guid ZeldaCatalogGameId { get; set; }

    public ZeldaCatalogAvailabilityKind Kind { get; set; }

    public int DisplayOrder { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string ImagePath { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Note { get; set; }

    public ZeldaCatalogGame? ZeldaCatalogGame { get; set; }
}

public enum ZeldaCatalogAvailabilityKind
{
    PrimaryConsole = 1,
    SecondaryConsole = 2,
    DownloadedPort = 3,
    SubscriptionService = 4,
}
