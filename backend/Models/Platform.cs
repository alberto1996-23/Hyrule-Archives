using System.ComponentModel.DataAnnotations;

namespace HyruleArchives.Models;

// I use this model to define one playable platform so release rows can point to it cleanly.

// Platforms describe consoles or systems such as NES, Switch, or GameCube.
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