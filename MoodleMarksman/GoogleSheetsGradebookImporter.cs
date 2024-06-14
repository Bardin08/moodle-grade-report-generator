using MoodleMarksman.Clients;
using MoodleMarksman.Data;
using MoodleMarksman.Models;
using MoodleMarksman.Validation;

namespace MoodleMarksman;

public class GoogleSheetsGradebookImporter
{
    public async Task<GradeBook> ImportGradebook(GetGradebookRequest request, IReadOnlyList<string> colsToImport)
    {
        var gradeBookProcessor = new MoodleGradeBookReportImporter(new CsvGradeBookValidator());
        var googleSpreadsheetDownloader = new GoogleSpreadsheetDownloader();

        var gradeBook = new GradeBook(new Dictionary<Student, List<Grade>>());
        foreach (var gid in request.SheetGIds)
        {
            var fromSpreadsheetStream = await googleSpreadsheetDownloader
                .DownloadSheetAsCsv(request.SpreadsheetId, gid);

            var actualGradeBook = gradeBookProcessor.Import(
                fromSpreadsheetStream, colsToImport, useStrictValidation: false);
            gradeBook = gradeBook.Merge(actualGradeBook, useStrictMode: false);
        }

        return gradeBook;
    }
}
