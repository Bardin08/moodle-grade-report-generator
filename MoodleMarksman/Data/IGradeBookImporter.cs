namespace MoodleMarksman.Data;

/// <summary>
/// Provides functionality for importing gradebooks from files.
/// </summary>
public interface IGradeBookImporter
{
    /// <summary>
    /// Imports a gradebook from the specified file.
    /// </summary>
    /// <param name="gradeBookPath">The full path to the gradebook file.</param>
    /// <returns>A GradeBook object representing the imported data.</returns>
    /// <exception cref="FileNotFoundException">Thrown if the specified gradebook file cannot be found.</exception>
    /// <exception cref="InvalidDataException">Thrown if the file is corrupt or in an unsupported format.</exception>
    GradeBook Import(string gradeBookPath);
}
