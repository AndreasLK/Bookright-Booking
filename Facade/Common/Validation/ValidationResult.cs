namespace Facade.Common.Validation
{
    /// <summary>
    /// Represents the outcome of a validation or update attempt.
    /// </summary>
    public record ValidationResult(bool IsValid, string? ErrorMessage = null);
}
