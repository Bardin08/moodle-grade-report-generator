using CsvHelper.Configuration;

namespace MoodleMarksman.Validation;

/// <summary>
/// Defines a contract for validating gradebook data.
/// </summary>
public interface IGradeBookValidator
{
    /// <summary>
    /// Validates a gradebook file at the specified path.
    /// </summary>
    /// <param name="gradeBookPath">The full path to the gradebook file.</param>
    /// <param name="csvConfiguration"></param>
    /// <returns>A ValidationResult object indicating the validity of the gradebook and any associated errors.</returns>
    ValidationResult Validate(string gradeBookPath, CsvConfiguration? csvConfiguration = null);
}
