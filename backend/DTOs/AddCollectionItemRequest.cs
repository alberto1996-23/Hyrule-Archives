using System.ComponentModel.DataAnnotations;
using HyruleArchives.Validation;

namespace HyruleArchives.DTOs;

// I use this request DTO when the frontend asks to add one game into a user's collection.

public class AddCollectionItemRequest
{
    [NotEmptyGuid]
    public Guid GameId { get; set; }
}