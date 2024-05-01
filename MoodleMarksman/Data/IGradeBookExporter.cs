namespace MoodleMarksman.Data;

/// <summary>
/// Provides functionality for exporting grade books to files.
/// </summary>
public interface IGradeBookExporter
{
    /// <summary>
    /// Exports the specified gradebook to a file.
    /// </summary>
    /// <param name="prevGradeBookPath">The full path to the previous version of the gradebook.</param>
    /// <param name="gradeBook">The GradeBook object to be exported.</param>
    /// <returns>The path to the newly created CSV file.</returns>
    /// <exception cref="FileNotFoundException">Thrown if the previous gradebook file cannot be found.</exception>
    /// <exception cref="InvalidDataException">Thrown if the data in the gradeBook object is invalid or incompatible with the previous version.</exception>
    string Export(string prevGradeBookPath, GradeBook gradeBook);
}
