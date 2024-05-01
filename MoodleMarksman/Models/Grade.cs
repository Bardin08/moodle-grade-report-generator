namespace MoodleMarksman.Models;

public record Grade(
    string ColName,
    decimal? Value)
{
    public override string ToString()
    {
        return ColName + ": " + Value;
    }
}
