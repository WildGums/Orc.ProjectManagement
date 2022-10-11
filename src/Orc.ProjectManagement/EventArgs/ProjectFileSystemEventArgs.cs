namespace Orc.ProjectManagement
{
    public class ProjectFileSystemEventArgs : ProjectEventArgs
    {
        public ProjectFileSystemEventArgs(string location, params string[] fileNames)
            : base(location)
        {
            FileNames = fileNames;
        }

        public ProjectFileSystemEventArgs(IProject project, params string[] fileNames)
            : base(project)
        {
            FileNames = fileNames;
        }

        public string[] FileNames { get; }
    }
}
