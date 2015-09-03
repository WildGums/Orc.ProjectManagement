// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectEventArgs.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    public class ProjectEventArgs : System.EventArgs
    {
        #region Constructors
        public ProjectEventArgs(IProject project)
        {
            Project = project;

            Project = project;

            if (project != null)
            {
                Location = project.Location;
            }
        }

        public ProjectEventArgs(string location)
        {
            Location = location;
        }
        #endregion

        #region Properties
        public string Location { get; private set; }
        public IProject Project { get; private set; }
        #endregion
    }
}