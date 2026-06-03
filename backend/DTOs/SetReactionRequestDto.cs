using System.ComponentModel.DataAnnotations;

namespace HyruleArchives.DTOs;

public class SetReactionRequest
{
    [Required]
    [RegularExpression("Like|Dislike")]
    public string Value { get; set; } = "Like";
}