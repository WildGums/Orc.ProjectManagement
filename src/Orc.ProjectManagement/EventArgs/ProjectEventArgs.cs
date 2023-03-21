namespace Orc.ProjectManagement;

public class ProjectEventArgs : ProjectLocationEventArgs
{
    public ProjectEventArgs(IProject project)
        : base(project.Location)
    {
        Project = project;
    }

    public IProject Project { get; }
}
