// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectFileSystemEventArgs.cs" company="WildGums">
//   Copyright (c) 2008 - 2016 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    public class ProjectFileSystemEventArgs : ProjectEventArgs
    {
        #region Constructors
        public ProjectFileSystemEventArgs(IProject project, params string[] fileNames)
            : base(project)
        {
            FileNames = fileNames;
        }

        public ProjectFileSystemEventArgs(string location, params string[] fileNames)
            : base(location)
        {
            FileNames = fileNames;
        }
        #endregion

        #region Properties
        public string[] FileNames { get; }
        #endregion
    }
}