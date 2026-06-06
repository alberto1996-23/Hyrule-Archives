using System.ComponentModel.DataAnnotations;

namespace HyruleArchives.Models;

// I use this entity as the root record for one curated Zelda game in the catalog.
// It keeps the shared metadata that the API needs before I branch into availability and collection details.

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

    // I store the original launch date here so I can return the public catalog in chronological order.
    public DateTime OriginalReleaseDate { get; set; }

    public List<ZeldaCatalogAvailability> AvailabilityEntries { get; set; } = new();

    public List<ZeldaCatalogCollectionAppearance> CollectionAppearances { get; set; } = new();
}
