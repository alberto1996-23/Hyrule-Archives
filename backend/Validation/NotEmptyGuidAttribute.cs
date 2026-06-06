using System.ComponentModel.DataAnnotations;

namespace HyruleArchives.Validation;
// I use this validation attribute when Guid.Empty should be rejected before an endpoint does extra work.

// This custom validation attribute is useful for request DTOs where Guid.Empty
// should be treated as invalid input instead of a meaningful id.
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class NotEmptyGuidAttribute : ValidationAttribute
{
    public NotEmptyGuidAttribute()
        : base("The field {0} must be a non-empty GUID.")
    {
    }

    public override bool IsValid(object? value)
    {
        // Validation attributes return true when the value is acceptable.
        return value is Guid guid && guid != Guid.Empty;
    }
}