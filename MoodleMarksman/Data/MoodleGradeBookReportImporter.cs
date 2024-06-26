﻿using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using MoodleMarksman.Models;

namespace MoodleMarksman.Data;

/// <summary>
/// Imports a gradebook from a Moodle gradebook report.
/// </summary>
public class MoodleGradeBookReportImporter : IGradeBookImporter
{
    /// <inheritdoc />
    public GradeBook Import(string gradeBookPath)
    {
        if (!File.Exists(gradeBookPath))
        {
            throw new FileNotFoundException("The specified gradebook file could not be found.", gradeBookPath);
        }

        return ImportInternal(gradeBookPath);
    }

    private GradeBook ImportInternal(string gradeBookPath)
    {
        using var reader = new StreamReader(gradeBookPath);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ","
        });

        return ParseGradeBookInternal(csv);
    }

    private GradeBook ParseGradeBookInternal(CsvReader csv)
    {
        csv.Read();
        csv.ReadHeader();

        var gradeBook = new Dictionary<Student, List<Grade>>();
        var marksColNames = ColsUtils
            .GetGradesColNames(csv.Context.Reader.HeaderRecord)
            .ToList();

        var gradeBookModel = new GradeBook(gradeBook, marksColNames);

        while (csv.Read())
        {
            var (student, marks) = GetGradeBookRow(csv, marksColNames);
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
