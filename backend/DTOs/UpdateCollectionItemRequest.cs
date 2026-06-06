using System.ComponentModel.DataAnnotations;

namespace HyruleArchives.DTOs;

// I use this request DTO to validate collection item edits before they hit the database.

// This DTO represents the data the frontend sends when a user updates
// a saved game in their collection.
//
// For now, the only editable field is Status.
// Example statuses: "Saved", "Playing", "Completed", "Dropped"
public class UpdateCollectionItemRequest
{
    [Required]
    [RegularExpression("Saved|Playing|Completed|Dropped")]
    public string Status { get; set; } = "Saved";
}