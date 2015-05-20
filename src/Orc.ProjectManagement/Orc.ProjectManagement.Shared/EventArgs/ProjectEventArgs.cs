// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectEventArgs.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using Catel;

    public class ProjectEventArgs : System.EventArgs
    {
        #region Constructors
        public ProjectEventArgs(IProject project)
        {
            Project = project;
        }
        #endregion

        #region Properties
        [ObsoleteEx(ReplacementTypeOrMember = "Project.Location", RemoveInVersion = "1.1.0", TreatAsErrorFromVersion = "1.0.0")]
        public string Location
        {
            get { return Project == null ? string.Empty : Project.Location; }
        }

        public IProject Project { get; private set; }
        #endregion
    }
}