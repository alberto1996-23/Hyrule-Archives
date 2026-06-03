using System.ComponentModel.DataAnnotations;

namespace HyruleArchives.DTOs;

public class CreateCommentRequest
{
    [Required]
    [MaxLength(2000)]
    public string Body { get; set; } = string.Empty;
}