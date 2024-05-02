using CsvHelper.Configuration;

namespace MoodleMarksman.Validation;

/// <summary>
/// Defines a contract for validating gradebook data.
/// </summary>
public interface IGradeBookValidator
{
    /// <summary>
    /// Validates a gradebook represented by the provided stream, ensuring structural integrity,
    /// correct data types, and potential custom rules (if specified in the csvConfiguration).
    /// </summary>
    /// <param name="gradeBookStream">A Stream containing gradebook data in CSV format.</param>
    /// <param name="csvConfiguration">An optional CsvConfiguration object to customize column
    /// mapping, data type expectations, or additional validation rules.</param>
    /// <returns>A ValidationResult object indicating the validity of the gradebook and any associated errors.</returns>
    ValidationResult Validate(Stream gradeBookStream, CsvConfiguration? csvConfiguration = null);
}
