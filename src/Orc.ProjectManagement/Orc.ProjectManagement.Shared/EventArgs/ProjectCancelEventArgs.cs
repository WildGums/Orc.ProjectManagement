// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectCancelEventArgs.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System.ComponentModel;

    public class ProjectCancelEventArgs : CancelEventArgs
    {
        #region Constructors
        public ProjectCancelEventArgs(IProject project, bool cancel = false)
            : base(cancel)
        {
            Project = project;
        }
        #endregion

        #region Properties
        [ObsoleteEx(Message = "In order to use project location without project in CancelEventArgs, use ProjectLocationCancelEventArgs",
            RemoveInVersion = "1.1.0", TreatAsErrorFromVersion = "1.0.0")]
        public string Location
        {
            get { return Project == null ? string.Empty : Project.Location; }
        }

        public IProject Project { get; private set; }
        #endregion
    }
}