namespace HyruleArchives.DTOs;

// This DTO represents the data the frontend sends when a user updates
// a saved game in their collection.
//
// For now, the only editable field is Status.
// Example statuses: "Saved", "Playing", "Completed", "Dropped"
public record UpdateCollectionItemRequest(
    string Status
);