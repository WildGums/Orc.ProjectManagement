namespace Orc.ProjectManagement
{
    public class ProjectActivationEventArgs : ProjectLocationEventArgs
    {
        public ProjectActivationEventArgs(IProject? project)
            : base(project?.Location)
        {
            Project = project;
        }

        public IProject? Project { get; private set; }
    }
}
