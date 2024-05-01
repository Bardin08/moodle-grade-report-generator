using CsvHelper;

namespace MoodleMarksman;

public static class StudentInfoColNames
{
    /// <summary>
    /// Column name for the student's first name.
    /// </summary>
    public const string FirstName = "First name";

    /// <summary>
    /// Column name for the student's last name.
    /// </summary>
    public const string LastName = "Last name";

    /// <summary>
    /// Column name for the student's email address.
    /// </summary>
    public const string Email = "Email address";
}

public static class MetadataColNames
{
    /// <summary>
    /// Column name indicating the timestamp of the last data download.
    /// </summary>
    public const string LastDownloaded = "Last downloaded from this course";
}

public static class ColsUtils
{
    /// <summary>
    /// Determines if a given column name represents student information.
    /// </summary>
    /// <param name="colName">The column name to check.</param>
    /// <returns>True if the column represents student information, otherwise false.</returns>
    private static bool IsStudentInfoCol(string colName) =>
        colName is StudentInfoColNames.FirstName or
        StudentInfoColNames.LastName or
        StudentInfoColNames.Email;

    /// <summary>
    /// Determines if a given column name represents metadata.
    /// </summary>
    /// <param name="colName">The column name to check.</param>
    /// <returns>True if the column represents metadata, otherwise false.</returns>
    private static bool IsMetadataCol(string colName) =>
        colName == MetadataColNames.LastDownloaded;

    /// <summary>
    /// Filters a CSV header and returns column names representing grades.
    /// </summary>
    /// <param name="headerRecord">The full header record from the CSV file.</param>
    /// <returns>An IEnumerable of strings containing the grade column names.</returns>
    public static IEnumerable<string> GetGradesColNames(IEnumerable<string> headerRecord) =>
        headerRecord.Where(colName => !IsStudentInfoCol(colName) && !IsMetadataCol(colName));

    /// <summary>
    /// Attempts to parse a decimal grade value from a CSV row.
    /// </summary>
    /// <param name="csvReader">The CsvReader object positioned at the correct row.</param>
    /// <param name="colName">The column name or index of the grade.</param>
    /// <returns>The parsed decimal value if successful, otherwise null.</returns>
    public static decimal? GetMarkValue(IReaderRow csvReader, string colName)
    {
        var isSuccess = csvReader.TryGetField<decimal>(colName, out var grade);
        return isSuccess ? grade : null;
    }

    public static string ShortenColNameWithSpans(this string colName, int colLength)
    {
        if (colName.Length <= colLength)
        {
            return colName;
        }

        var middleCharsToKeep = Math.Max(0, colLength - 3);
        var startLength = middleCharsToKeep / 2;
        var endLength = middleCharsToKeep - startLength;

        var shortenedName = new char[colLength];
        colName.AsSpan()[..startLength].CopyTo(shortenedName.AsSpan());
        "...".AsSpan().CopyTo(shortenedName.AsSpan(startLength));
        colName.AsSpan(colName.Length - endLength).CopyTo(shortenedName.AsSpan(startLength + 3));

        return new string(shortenedName);
    }
}
