using MoodleMarksman.Clients;
using MoodleMarksman.Data;
using MoodleMarksman.Validation;
using MoodleMarksman.Visualization;

const string spreadsheetId = "spreadsheetId";
const string sheetGid = "sheetGid";

const string exportedGradeBookPath = "from-moodle.csv";
var moodleGradeBookStream = File.OpenRead(exportedGradeBookPath);
var gradeBookProcessor = new MoodleGradeBookReportImporter(new CsvGradeBookValidator());
var moodleGradeBook = gradeBookProcessor.Import(moodleGradeBookStream);


var googleSpreadsheetDownloader = new GoogleSpreadsheetDownloader();
var fromSpreadsheetStream = await googleSpreadsheetDownloader.DownloadSheetAsCsv(spreadsheetId, sheetGid);
var actualGradeBook = gradeBookProcessor.Import(fromSpreadsheetStream);


actualGradeBook.GradesColsAsImmutable(actualGradeBook.GradesColNames
    .Where(x => x.StartsWith("Quiz", StringComparison.OrdinalIgnoreCase)));

moodleGradeBook.Merge(actualGradeBook);


var moodleCsvGradeBookExporter = new MoodleCsvGradeBookExporter();
moodleCsvGradeBookExporter.Export(exportedGradeBookPath, moodleGradeBook);

GradeBookVisualizer.DisplayGradeBook(moodleGradeBook);



Console.WriteLine("Done!");
