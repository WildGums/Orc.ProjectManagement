namespace Orc.ProjectManagement.Example.Models;

public class PersonProject : ProjectBase
{
    public PersonProject(string title) 
        : base(title)
    {
        FirstName = string.Empty;
        MiddleName = string.Empty;
        LastName = string.Empty;
    }

    public string FirstName { get; set; }

    public string MiddleName { get; set; }

    public string LastName { get; set; }
}
