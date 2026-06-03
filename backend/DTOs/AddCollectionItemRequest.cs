using System.ComponentModel.DataAnnotations;
using HyruleArchives.Validation;

namespace HyruleArchives.DTOs;

public class AddCollectionItemRequest
{
    [NotEmptyGuid]
    public Guid GameId { get; set; }
}