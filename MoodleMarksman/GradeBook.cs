using System.Text;
using MoodleMarksman.Models;

namespace MoodleMarksman;

public class GradeBook(
    Dictionary<Student, List<Grade>> gradebook)
{
    private readonly Dictionary<Student, List<Grade>> _gradebook = gradebook;
    private readonly List<string> _immutableGradesColNames = [];

    public IReadOnlyDictionary<Student, List<Grade>> Gradebook => _gradebook.AsReadOnly();

    /// <summary>
    /// Gets the available column names representing grades (grades).
    /// </summary>
    public IEnumerable<string> GradesColNames => Gradebook.First().Value.Select(x => x.ColName);

    /// <summary>
    /// Retrieves the grades associated with a student's email.
    /// </summary>
    /// <param name="email">The student's email address.</param>
    /// <returns>A list of Grade objects representing the student's grades.</returns>
    /// <exception cref="ArgumentException">Thrown if the email is null or whitespace.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the student or their grades cannot be found.</exception>
    public List<Grade> GetGrades(string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        return GetStudentGrades(email);
    }

    public GradeBook Merge(GradeBook other)
    {
        var newGradeBook = new GradeBook(_gradebook);

        foreach (var (student, grades) in other.Gradebook)
        {
            try
            {
                var oldGrades = GetStudentGrades(student.Email);
                if (oldGrades is null && grades is null)
                {
                    continue;
                }

                // todo: what's the purpose of this check?
                if (oldGrades?.SequenceEqual(grades) ?? false)
                {
                    continue;
                }
            }
            catch
            {
                // ignored
            }

            foreach (var grade in grades)
            {
                newGradeBook.UpsertStudentAndGrade(student, grade);
            }
        }

        return newGradeBook;
    }

    public void GradesColsAsImmutable(IEnumerable<string> colsNames)
    {
        _immutableGradesColNames.AddRange(colsNames);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var (student, grades) in _gradebook)
        {
            sb.AppendLine(student.ToString());
            foreach (var grade in grades)
            {
                sb.AppendLine(grade.ToString());
            }
        }

        return sb.ToString();
    }

    #region Private Methods

    private List<Grade> GetStudentGrades(string email)
    {
        var (student, grades) = _gradebook.FirstOrDefault(
            s => s.Key.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

        EnsureStudentAndGradesExists(email, student, grades);

        return grades;
    }

    private static void EnsureStudentAndGradesExists(string email, Student student, List<Grade> grades)
    {
        if (student is null)
        {
            throw new InvalidOperationException($"Student with email {email} not found");
        }

        if (grades is null)
        {
            throw new InvalidOperationException($"Student with email {email} has no grades");
        }
    }

    private void UpsertStudentAndGrade(Student student, Grade grade)
    {
        if (!_gradebook.ContainsKey(student))
        {
            InsertStudentAndGrade(student, grade);
            return;
        }

        UpdateGrade(student, grade);
    }

    private void InsertStudentAndGrade(Student student, Grade grade)
    {
        var isGradeColImmutable = _immutableGradesColNames.Contains(grade.ColName);
        List<Grade> studentGrades = isGradeColImmutable ? [] : [grade];

        _gradebook.Add(student, studentGrades);
    }

    private void UpdateGrade(Student student, Grade grade)
    {
        var grades = GetStudentGrades(student.Email);
        var existingGrade = grades.GetGradeByColName(grade.ColName);

        if (existingGrade is not null)
        {
            grades.Remove(existingGrade);
        }

        grades.Add(grade);
    }

    #endregion
}

internal static class GradeBookInternalExtensions
{
    public static Grade? GetGradeByColName(this IEnumerable<Grade> grades, string gradeName)
        => grades.FirstOrDefault(m => m.ColName.Equals(gradeName, StringComparison.OrdinalIgnoreCase));
}
