using System.ComponentModel.DataAnnotations;

namespace HyruleArchives.DTOs;

public class UpdateCommentRequest
{
    [Required]
    [MinLength(1)]
    [MaxLength(2000)]
    public string Body { get; set; } = string.Empty;
}