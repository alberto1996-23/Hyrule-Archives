using System.ComponentModel.DataAnnotations;

namespace HyruleArchives.Models;

public class ZeldaCatalogGame
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Slug { get; set; } = string.Empty;

    public int MobyGameId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string BoxArtPath { get; set; } = string.Empty;

    public List<ZeldaCatalogAvailability> AvailabilityEntries { get; set; } = new();

    public List<ZeldaCatalogCollectionAppearance> CollectionAppearances { get; set; } = new();
}
