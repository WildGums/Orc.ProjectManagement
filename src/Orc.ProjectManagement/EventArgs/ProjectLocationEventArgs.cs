namespace Orc.ProjectManagement
{
    public class ProjectLocationEventArgs : System.EventArgs
    {
        public ProjectLocationEventArgs(string? location)
        {
            Location = location;
        }

        public string? Location { get; private set; }
    }
}
