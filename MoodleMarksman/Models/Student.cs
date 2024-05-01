namespace MoodleMarksman.Models;

public record Student(
    string FirstName,
    string LastName,
    string Email)
{
    public override string ToString()
    {
        return FirstName + " " + LastName + " (" + Email[..Email.IndexOf('@')] + ")";
    }
}
