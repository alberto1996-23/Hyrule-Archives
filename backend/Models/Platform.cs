using System.ComponentModel.DataAnnotations;

namespace HyruleArchives.Models;

public class Platform
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Slug { get; set; } = string.Empty;

    public List<GameRelease> Releases { get; set; } = new();
}