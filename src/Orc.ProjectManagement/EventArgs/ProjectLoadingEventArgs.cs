namespace Orc.ProjectManagement
{
    /// <summary>
    /// Event args object specific for <see cref="IProjectManager.ProjectLoading"/> event
    /// </summary>
    public class ProjectLoadingEventArgs : ProjectEventArgs
    {
        /// <summary>
        /// Constructor with string
        /// </summary>
        /// <param name="location">Location</param>
        public ProjectLoadingEventArgs(string location)
            : base(location)
        {
        }

        /// <summary>
        /// Constructor with project
        /// </summary>
        /// <param name="project">Project</param>
        public ProjectLoadingEventArgs(IProject project)
            : base(project)
        {
        }

        /// <summary>
        /// If loading was canceled or not
        /// </summary>
        public bool IsCanceled { get; set; }
    }
}