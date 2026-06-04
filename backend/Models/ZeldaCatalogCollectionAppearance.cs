using System.ComponentModel.DataAnnotations;

namespace HyruleArchives.Models;

public class ZeldaCatalogCollectionAppearance
{
    public Guid Id { get; set; }

    public Guid ZeldaCatalogGameId { get; set; }

    public int DisplayOrder { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string BoxArtPath { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string PlatformName { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string PlatformImagePath { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Note { get; set; }

    public ZeldaCatalogGame? ZeldaCatalogGame { get; set; }
}
