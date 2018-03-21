// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectCancelEventArgs.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System.ComponentModel;

    public class ProjectCancelEventArgs : CancelEventArgs
    {
        #region Constructors
        public ProjectCancelEventArgs(string location, bool cancel = false)
            : base(cancel)
        {
            Location = location;
        }

        public ProjectCancelEventArgs(IProject project, bool cancel = false)
            : base(cancel)
        {
            Project = project;

            if (project != null)
            {
                Location = project.Location;
            }
        }
        #endregion

        #region Properties
        public string Location { get; private set; }
        public IProject Project { get; private set; }
        #endregion
    }
}