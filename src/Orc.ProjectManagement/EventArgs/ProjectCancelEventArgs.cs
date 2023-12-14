namespace Orc.ProjectManagement;

using System.ComponentModel;

public class ProjectCancelEventArgs : CancelEventArgs
{
    public ProjectCancelEventArgs(string location, bool cancel = false)
        : base(cancel)
    {
        Location = location;
    }

    public ProjectCancelEventArgs(IProject project, bool cancel = false)
        : base(cancel)
    {
        Project = project;
        Location = project?.Location;
    }

    public string? Location { get; }
    public IProject? Project { get; }
}
