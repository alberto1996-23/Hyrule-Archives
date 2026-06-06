using System.ComponentModel.DataAnnotations;

namespace HyruleArchives.DTOs;

// I use this request DTO to validate the replacement text when a comment is edited.

public class UpdateCommentRequest
{
    [Required]
    [MinLength(1)]
    [MaxLength(2000)]
    public string Body { get; set; } = string.Empty;
}