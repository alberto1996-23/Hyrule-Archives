using System.ComponentModel.DataAnnotations;

namespace HyruleArchives.Models;

public class Region
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Code { get; set; } = string.Empty;

    public List<GameRelease> Releases { get; set; } = new();
}