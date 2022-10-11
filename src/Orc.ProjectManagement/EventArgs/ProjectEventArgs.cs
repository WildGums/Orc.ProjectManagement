namespace Orc.ProjectManagement
{
    public class ProjectEventArgs : System.EventArgs
    {
        public ProjectEventArgs(IProject project)
        {
            Project = project;
            Location = project.Location ?? string.Empty;
        }

        public string Location { get; private set; }
        public IProject Project { get; private set; }
    }
}
