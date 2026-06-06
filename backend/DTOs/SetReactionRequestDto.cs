using System.ComponentModel.DataAnnotations;

namespace HyruleArchives.DTOs;

// I use this request DTO to validate the one reaction choice coming from the client.

// This request is sent when a user likes or dislikes a game.
// Validation keeps the API limited to the two allowed values.
public class SetReactionRequest
{
    [Required]
    [RegularExpression("Like|Dislike")]
    public string Value { get; set; } = "Like";
}