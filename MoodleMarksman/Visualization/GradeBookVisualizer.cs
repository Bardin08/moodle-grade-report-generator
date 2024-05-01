using MoodleMarksman.Models;

namespace MoodleMarksman.Visualization;

public static class GradeBookVisualizer
{
    private static IEnumerable<(Student, List<Grade>)> _studentRecords; // Holds student data
    private static int _currentStudentIndex = 0;

    public static void DisplayGradeBook(GradeBook gradeBook)
    {
        _studentRecords = gradeBook.Gradebook.Select(kvp => (kvp.Key, kvp.Value));
        DisplayCurrentStudent(); // Display the first student initially
        HandleNavigation();
    }

    private static void HandleNavigation()
    {
        while (true)
        {
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.LeftArrow)
            {
                DisplayPreviousStudent();
            }
            else if (key.Key == ConsoleKey.RightArrow)
            {
                DisplayNextStudent();
            }
            else if (key.Key == ConsoleKey.Escape)
            {
                break; // Exit on 'Escape'
            }
        }
    }

    private static void DisplayPreviousStudent()
    {
        _currentStudentIndex = Math.Max(0, _currentStudentIndex - 1); // Prevent going below index 0
        DisplayCurrentStudent();
    }

    private static void DisplayNextStudent()
    {
        _currentStudentIndex =
            Math.Min(_studentRecords.Count() - 1, _currentStudentIndex + 1); // Prevent going beyond data
        DisplayCurrentStudent();
    }

    private static void DisplayCurrentStudent()
    {
        const int colWidth = 35;
        Console.Clear();
        var (student, grades) = _studentRecords.ElementAt(_currentStudentIndex);

        Console.WriteLine(new string('#', colWidth * 2));
        Console.WriteLine($" {"",-colWidth}|  {student,-colWidth}");
        Console.WriteLine(new string('#', colWidth * 2));

        foreach (var grade in grades.Where(x => x.Value.HasValue))
        {
            Console.WriteLine($"{grade.ColName.ShortenColNameWithSpans(35),-colWidth} | {grade.Value,-colWidth}");
        }
    }
}
