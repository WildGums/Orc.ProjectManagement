namespace Orc.ProjectManagement
{
    using System;
    using Catel;

    public class ProjectUpdatedEventArgs : EventArgs
    {
        public ProjectUpdatedEventArgs(IProject? oldProject, IProject? newProject)
        {
            OldProject = oldProject;
            NewProject = newProject;
        }

        public IProject? OldProject { get; private set; }

        public IProject? NewProject { get; private set; }

        public bool IsRefresh
        {
            get
            {
                if (OldProject is null || NewProject is null)
                {
                    return false;
                }

                return ObjectHelper.AreEqual(OldProject.Location, NewProject.Location);
            }
        }
    }
}
