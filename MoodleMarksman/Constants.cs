namespace MoodleMarksman;

public static class StudentInfoColNames
{
    public const string FirstName = "First name";
    public const string LastName = "Last name";
    public const string Email = "Email address";
}

public static class MetadataColNames
{
    public const string LastDownloaded = "Last downloaded from this course";
}

public static class ColsUtis
{
    public static bool IsStudentInfoCol(string colName) =>
        colName is StudentInfoColNames.FirstName or
        StudentInfoColNames.LastName or
        StudentInfoColNames.Email;

    public static bool IsMetadataCol(string colName) =>
        colName == MetadataColNames.LastDownloaded;

    // get all marks col names
    public static IEnumerable<string> GetMarksColNames(IEnumerable<string> headerRecord) =>
        headerRecord.Where(colName => !IsStudentInfoCol(colName) && !IsMetadataCol(colName));
}
