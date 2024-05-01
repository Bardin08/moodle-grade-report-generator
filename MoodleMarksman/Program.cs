using MoodleMarksman.Data;
using MoodleMarksman.Visualization;

const string updatedGradeBookPath = "updated.csv";
const string exportedGradeBookPath = "exported.csv";

var gradeBookProcessor = new MoodleGradeBookReportImporter();
var gradeBook = gradeBookProcessor.Import(updatedGradeBookPath);

var exportedGradeBook = gradeBookProcessor.Import(exportedGradeBookPath);
exportedGradeBook.GradesColsAsImmutable(exportedGradeBook.GradesColNames
    .Where(x => x.StartsWith("Quiz", StringComparison.OrdinalIgnoreCase)));

gradeBook.Merge(exportedGradeBook);

var moodleCsvGradeBookExporter = new MoodleCsvGradeBookExporter();
moodleCsvGradeBookExporter.Export(exportedGradeBookPath, exportedGradeBook);

GradeBookVisualizer.DisplayGradeBook(exportedGradeBook);



Console.WriteLine("Done!");
