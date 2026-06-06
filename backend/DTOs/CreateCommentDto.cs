using System.ComponentModel.DataAnnotations;

namespace HyruleArchives.DTOs;

// I use this request DTO to validate the body text when someone submits a new comment.

public class CreateCommentRequest
{
    [Required]
    [MinLength(1)]
    [MaxLength(2000)]
    public string Body { get; set; } = string.Empty;
}