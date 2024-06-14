using MoodleMarksman.Data;
using MoodleMarksman.Validation;
using MoodleMarksman.Visualization;

namespace MoodleMarksman;

public class UtilRunner
{
    public const bool IsHomework = true;

    public static async Task RunAsync()
    {
        const string exportedGradeBookPath = "current-marks.csv";
        var moodleGradeBookStream = File.OpenRead(exportedGradeBookPath);
        var gradeBookProcessor = new MoodleGradeBookReportImporter(new CsvGradeBookValidator());
        var moodleGradeBook = gradeBookProcessor.Import(moodleGradeBookStream);

        var googleSpreadsheetDescriptor = GoogleSpreadsheetDataProvider.Descriptors["hw2"];
        var colsToImport = moodleGradeBook.GradesColNames.ToList();

        var gradeBookRequest = new GetGradebookRequest(
            googleSpreadsheetDescriptor.SpreadsheetId,
            googleSpreadsheetDescriptor.SheetGIds);

        var googleSheetsGradebookImporter = new GoogleSheetsGradebookImporter();
        var actualGradeBook = await googleSheetsGradebookImporter
            .ImportGradebook(gradeBookRequest, colsToImport);

        var updatedGradebook = moodleGradeBook.Merge(actualGradeBook, !IsHomework);

        var moodleCsvGradeBookExporter = new MoodleCsvGradeBookExporter();
        moodleCsvGradeBookExporter.Export(exportedGradeBookPath, updatedGradebook);

        GradeBookVisualizer.DisplayGradeBook(moodleGradeBook);

        Console.WriteLine("Done!");
    }
}

public static class GoogleSpreadsheetDataProvider
{
    public static readonly Dictionary<string, SpreadsheetDescriptor> Descriptors = new()
    {
    };
}

public record SpreadsheetDescriptor(string SpreadsheetId, string[] SheetGIds);
