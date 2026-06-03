using System.ComponentModel.DataAnnotations;

namespace HyruleArchives.DTOs;

public class UpdateCommentRequest
{
    [Required]
    [MaxLength(2000)]
    public string Body { get; set; } = string.Empty;
}