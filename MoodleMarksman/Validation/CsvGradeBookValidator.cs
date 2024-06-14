using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace MoodleMarksman.Validation;

public class CsvGradeBookValidator : IGradeBookValidator
{
    private static readonly CsvConfiguration DefaultCsvConfiguration = new(CultureInfo.InvariantCulture)
    {
        HasHeaderRecord = true
    };

    public ValidationResult Validate(Stream gradeBookStream, CsvConfiguration? csvConfiguration = null)
    {
        csvConfiguration ??= DefaultCsvConfiguration;
        // Rules:

        // Must contain cols that'd matched to the StudentInfoColNames (FirstName, LastName, Email)
        // Can (optionally) contain cols that'd matched to the MetadataColNames (LastDownloaded)
        // Must contain at least one grade col

        var streamReader = new StreamReader(gradeBookStream);
        var csvReader = new CsvReader(streamReader, csvConfiguration);
        csvReader.Read();
        csvReader.ReadHeader();

        return CheckCols(csvReader);
    }

    private static ValidationResult CheckCols(CsvReader csvReader)
    {
        var headerRecord = csvReader.Context.Reader.HeaderRecord;

        var gradeColNames = ColsUtils.GetGradesColNames(headerRecord).ToList();

        var missingCols = CheckMissingCols(headerRecord, gradeColNames);
        return missingCols.Count is 0
            ? new ValidationResult(true)
            : new ValidationResult(false, missingCols);
    }

    private static HashSet<string> CheckMissingCols(
        string[] headerRecord, List<string> gradeColNames, bool useStrict = true)
    {
        var missingCols = new HashSet<string>();
        var studentInfoCols = new HashSet<string>
        {
            StudentInfoColNames.FirstName,
            StudentInfoColNames.LastName,
            StudentInfoColNames.Email
        };

        if (useStrict)
        {
            if (!headerRecord.Any(studentInfoCols.Contains))
            {
                missingCols.Add("Student information columns");
            }
        }
        else
        {
            var atLEastOneStudentInfoCol = headerRecord.Any(studentInfoCols.Contains);
            if (!atLEastOneStudentInfoCol)
            {
                missingCols.Add("Student information columns");
            }
        }

        if (!headerRecord.Any(gradeColNames.Contains))
        {
            missingCols.Add("Grade columns");
        }

        return missingCols;
    }
}
