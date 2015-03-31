// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectEventArgs.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System;
    using Catel;

    public class ProjectUpdatedEventArgs : EventArgs
    {
        public ProjectUpdatedEventArgs(IProject oldProject, IProject newProject)
        {
            OldProject = oldProject;
            NewProject = newProject;
        }

        public IProject OldProject { get; private set; }

        public IProject NewProject { get; private set; }

        public bool IsRefresh
        {
            get
            {
                if (OldProject == null || NewProject == null)
                {
                    return false;
                }

                return ObjectHelper.AreEqual(OldProject.Location, NewProject.Location);
            }
        }
    }
}