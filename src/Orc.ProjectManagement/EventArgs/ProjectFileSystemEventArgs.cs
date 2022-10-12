namespace Orc.ProjectManagement
{
    public class ProjectFileSystemEventArgs : ProjectLocationEventArgs
    {
        public ProjectFileSystemEventArgs(string location, params string[] fileNames)
            : base(location)
        {
            FileNames = fileNames;
        }

        public string[] FileNames { get; }
    }
}
