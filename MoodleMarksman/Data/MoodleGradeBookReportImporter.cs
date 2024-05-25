using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using MoodleMarksman.Models;
using MoodleMarksman.Validation;

namespace MoodleMarksman.Data;

/// <summary>
/// Imports a gradebook from a Moodle gradebook report.
/// </summary>
public class MoodleGradeBookReportImporter(IGradeBookValidator gradeBookValidator) : IGradeBookImporter
{
    /// <inheritdoc />
    public GradeBook Import(Stream gradeBookStream, IReadOnlyList<string>? colsNames = null)
    {
        var validationResult = gradeBookValidator.Validate(gradeBookStream);
        if (validationResult.IsValid)
        {
            return ImportInternal(gradeBookStream, colsNames);
        }

        var errors = string.Join(":\n", validationResult.Errors!);
        var errorMessage = $"The specified gradebook file is invalid. The following columns are missing: {errors}";
        throw new InvalidDataException(errorMessage);
    }

    private GradeBook ImportInternal(Stream gradeBookStream, IReadOnlyList<string>? colsNames = null)
    {
        gradeBookStream.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(gradeBookStream);
        var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ","
        });

        return ParseGradeBookInternal(csv, colsNames);
    }

    private GradeBook ParseGradeBookInternal(CsvReader csv, IReadOnlyList<string>? colsNames = null)
    {
        csv.Read();
        csv.ReadHeader();

        var gradeBook = new Dictionary<Student, List<Grade>>();
        var marksColNames = ColsUtils
            .GetGradesColNames(csv.Context.Reader.HeaderRecord);

        if (colsNames is not null)
        {
            marksColNames = marksColNames.Where(colsNames.Contains);
        }

        marksColNames = marksColNames.ToList();

        var gradeBookModel = new GradeBook(gradeBook);

        while (csv.Read())
        {
            var (student, marks) = GetGradeBookRow(csv, marksColNames.ToList());
            if (student.Email == string.Empty)
            {
                continue;
            }

            gradeBook.Add(student, marks);
        }

        return gradeBookModel;
    }

    private static (Student student, List<Grade> marks) GetGradeBookRow(
        CsvReader csv, ICollection<string> marksColNames)
    {
        var student = new Student(
            csv.GetField<string>(StudentInfoColNames.FirstName),
            csv.GetField<string>(StudentInfoColNames.LastName),
            csv.GetField<string>(StudentInfoColNames.Email)
        );

        var marks = csv.Context.Reader.HeaderRecord
            .Where(marksColNames.Contains)
            .Select(header => new Grade(header, ColsUtils.GetMarkValue(csv, header)))
            .ToList();

        return (student, marks);
    }
}
