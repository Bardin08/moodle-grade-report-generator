using System.Globalization;
using System.Text;
using CsvHelper;

namespace MoodleMarksman.Models;

public class GradeBookProcessor(
    Dictionary<Student, List<Mark>> gradebook,
    bool isSourceOfTruth = false)
{
    private readonly Dictionary<Student, List<Mark>> _gradebook = gradebook;
    private readonly bool _isSourceOfTruth = isSourceOfTruth;
    private readonly List<string> _immutableMarksColNames = [];

    public GradeBookProcessor() : this([])
    {
    }

    public decimal? GetMark(string email, string markColName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(markColName);

        var marks = GetStudentMarks(email);
        return marks.GetMarkByColName(markColName)?.Value;
    }

    public void UpdateMark(string email, string markColName, decimal value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(markColName);

        var isMarkColMutable = _immutableMarksColNames.Contains(markColName);
        if (!isMarkColMutable)
        {
            return;
        }

        var marks = GetStudentMarks(email);

        var mark = marks.GetMarkByColName(markColName);

        if (mark is not null)
        {
            marks.Remove(mark);
        }

        marks.Add(new Mark(markColName, value));
    }

    /// <summary>
    /// Override sourceGradeBook gradebook with this grade book's marks.
    /// If sourceGradeBook is a source of truth(param passed to constructor), only add marks that are not present in others.
    /// </summary>
    /// <param name="sourceGradeBookProcessor">Gradebook that will be updated</param>
    /// <returns>The gradebook</returns>
    public GradeBookProcessor Merge(GradeBookProcessor sourceGradeBookProcessor)
    {
        foreach (var (student, marks) in _gradebook)
        {
            foreach (var mark in marks.Where(mark => !sourceGradeBookProcessor._immutableMarksColNames.Contains(mark.ColName)))
            {
                if (!sourceGradeBookProcessor._isSourceOfTruth)
                {
                    sourceGradeBookProcessor.UpsertStudentAndMark(student, mark);
                }
                else
                {
                    var otherMark = sourceGradeBookProcessor.GetMark(student.Email, mark.ColName);
                    if (otherMark is null)
                    {
                        sourceGradeBookProcessor.UpsertStudentAndMark(student, mark);
                    }
                }
            }
        }

        return sourceGradeBookProcessor;
    }

    public void UpdateFile(string sourceFileSrc)
    {
        using var reader = new StreamReader(sourceFileSrc);
        using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
        using var writer = new StreamWriter("temp.csv");
        using var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);

        csvReader.Read();
        csvReader.ReadHeader();

        foreach (var header in csvReader.Context.Reader.HeaderRecord)
        {
            csvWriter.WriteField(header);
        }
        csvWriter.NextRecord();

        while (csvReader.Read())
        {
            var record = csvReader.HeaderRecord
                .ToDictionary(
                    header => header,
                    header => csvReader.GetField(header));

            var student = new Student(
                record[StudentInfoColNames.FirstName],
                record[StudentInfoColNames.LastName],
                record[StudentInfoColNames.Email]
            );

            var marks = _gradebook[student];
            foreach (var mark in marks)
            {
                record[mark.ColName] = mark.Value?.ToString() ?? "";
            }

            // Write back to CSV
            foreach (var key in record.Keys)
            {
                csvWriter.WriteField(record[key]);
            }

            csvWriter.NextRecord();
        }

        // File.Delete(filePath);
        // File.Move("temp.csv", filePath);
    }

    public void MarksColsAsImmutable(IEnumerable<string> marksColNames)
    {
        _immutableMarksColNames.AddRange(marksColNames);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var (student, marks) in _gradebook)
        {
            sb.AppendLine(student.ToString());
            foreach (var mark in marks)
            {
                sb.AppendLine(mark.ToString());
            }
        }

        return sb.ToString();
    }

    #region Private Methods

    private List<Mark> GetStudentMarks(string email)
    {
        var (student, marks) = _gradebook.FirstOrDefault(
            s => s.Key.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

        EnsureStudentAndMarksExists(email, student, marks);

        return marks;
    }

    private static void EnsureStudentAndMarksExists(string email, Student student, List<Mark> marks)
    {
        if (student is null)
        {
            throw new InvalidOperationException($"Student with email {email} not found");
        }

        if (marks is null)
        {
            throw new InvalidOperationException($"Student with email {email} has no marks");
        }
    }

    private void UpsertStudentAndMark(Student student, Mark mark)
    {
        if (!_gradebook.ContainsKey(student))
        {
            var isMarkColMutable = _immutableMarksColNames.Contains(mark.ColName);
            List<Mark> studentMarks = isMarkColMutable ? [mark] : [];

            _gradebook.Add(student, studentMarks);
        }

        var marks = GetStudentMarks(student.Email);
        var existingMark = marks.GetMarkByColName(mark.ColName);

        if (existingMark is not null)
        {
            marks.Remove(existingMark);
        }

        marks.Add(mark);
    }

    #endregion
}

internal static class GradeBookInternalExtensions
{
    public static Mark? GetMarkByColName(this IEnumerable<Mark> marks, string markName)
        => marks.FirstOrDefault(m => m.ColName.Equals(markName, StringComparison.OrdinalIgnoreCase));
}
