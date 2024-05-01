namespace MoodleMarksman.Validation;

/// <summary>
/// Represents the result of a gradebook validation operation.
/// </summary>
/// <param name="IsValid">Indicates whether the gradebook is considered valid.</param>
/// <param name="Errors">A collection of error messages describing any validation failures.</param>
public record ValidationResult(
    bool IsValid,
    IReadOnlySet<string>? Errors = null);
