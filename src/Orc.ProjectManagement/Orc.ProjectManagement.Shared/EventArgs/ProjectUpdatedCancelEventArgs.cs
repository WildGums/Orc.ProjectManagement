// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectUpdatedCancelEventArgs.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System.ComponentModel;

    public class ProjectUpdatedCancelEventArgs : CancelEventArgs
    {
        public ProjectUpdatedCancelEventArgs(IProject oldProject, IProject newProject, bool cancel = false)
            : base(cancel)
        {
            OldProject = oldProject;
            NewProject = newProject;
        }

        public IProject OldProject { get; private set; }

        public IProject NewProject { get; private set; }
    }
}