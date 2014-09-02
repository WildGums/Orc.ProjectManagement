// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectEventArgs.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System;

    public class ProjectEventArgs : EventArgs
    {
        public ProjectEventArgs(string location)
        {
            Location = location;
        }

        public ProjectEventArgs(IProject project)
        {
            Project = project;

            if (project != null)
            {
                Location = project.Location;
            }
        }

        public string Location { get; private set; }

        public IProject Project { get; private set; }
    }
}