using System.Globalization;
using CsvHelper;
using MoodleMarksman.Models;

namespace MoodleMarksman;

/// <summary>
/// Exports the specified gradebook to a CSV file, using a previous gradebook for header
/// information and data validation.
/// </summary>
public class MoodleCsvGradeBookExporter : IGradeBookExporter
{
    /// <inheritdoc />
    public string Export(string prevGradeBookPath, GradeBook gradeBook)
    {
        using var reader = new StreamReader(prevGradeBookPath);
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

            var marks = gradeBook.GetMarks(student.Email);
            foreach (var mark in marks)
            {
                record[mark.ColName] = mark.Value?.ToString() ?? "";
            }

            foreach (var key in record.Keys)
            {
                csvWriter.WriteField(record[key]);
            }

            csvWriter.NextRecord();
        }

        return Path.GetFullPath("temp.csv");
    }
}