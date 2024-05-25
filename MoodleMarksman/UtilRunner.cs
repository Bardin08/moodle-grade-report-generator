using MoodleMarksman.Clients;
using MoodleMarksman.Data;
using MoodleMarksman.Validation;
using MoodleMarksman.Visualization;

namespace MoodleMarksman;

public class UtilRunner
{
    public static async Task RunAsync()
    {
        const string spreadsheetId = "sheet-id-here";

        const string exportedGradeBookPath = "current-marks.csv";
        var moodleGradeBookStream = File.OpenRead(exportedGradeBookPath);
        var gradeBookProcessor = new MoodleGradeBookReportImporter(new CsvGradeBookValidator());
        var moodleGradeBook = gradeBookProcessor.Import(moodleGradeBookStream);

        var colsToImport = moodleGradeBook.GradesColNames.ToList();
        var actualGradeBook = await new GoogleSheetsGradebookImporter()
            .ImportGradebook(new GetGradebookRequest(spreadsheetId,
            ["GIds-here"]), colsToImport);

        var updated = moodleGradeBook.Merge(actualGradeBook);

        var moodleCsvGradeBookExporter = new MoodleCsvGradeBookExporter();
        moodleCsvGradeBookExporter.Export(exportedGradeBookPath, updated);

        GradeBookVisualizer.DisplayGradeBook(moodleGradeBook);

        Console.WriteLine("Done!");
    }
}
