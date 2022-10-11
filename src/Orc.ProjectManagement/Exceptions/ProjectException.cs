namespace Orc.ProjectManagement
{
    using System;

    public class ProjectException : Exception
    {
        public ProjectException(string location, string message)
            : base(message)
        {
            Location = location;
        }

        public ProjectException(IProject project, string message)
            : base(message)
        {
            Project = project;
        }

        public ProjectException(IProject project, string message, Exception innerException)
            : base(message, innerException)
        {
            Project = project;
        }

        public string? Location { get; private set; }
        public IProject? Project { get; private set; }
    }
}
