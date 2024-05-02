namespace MoodleMarksman.Data;

/// <summary>
/// Provides functionality for importing gradebooks from files.
/// </summary>
public interface IGradeBookImporter
{
    /// <summary>
    /// Imports a gradebook from the specified stream, assuming a CSV (Comma-Separated Values) format.
    /// </summary>
    /// <param name="gradeBookStream">A Stream containing the CSV-formatted gradebook data.</param>
    /// <returns>A GradeBook object representing the imported data.</returns>
    /// <exception cref="FileNotFoundException">Thrown if the specified gradebook file cannot be found.</exception>
    /// <exception cref="InvalidDataException">Thrown if the file is corrupt, contains structural errors, or has data inconsistencies preventing successful parsing.</exception>
    GradeBook Import(Stream gradeBookStream);
}
