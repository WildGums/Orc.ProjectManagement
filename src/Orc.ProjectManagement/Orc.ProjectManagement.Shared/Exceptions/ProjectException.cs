// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectException.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System;

    public class ProjectException : Exception
    {
        #region Constructors
        public ProjectException(string location, string message)
            : base(message)
        {
            Location = location;
        }

        public ProjectException(IProject project, string message)
            : base(message)
        {
            Project = project;
        }

        public ProjectException(IProject project, string message, Exception innerException)
            : base(message, innerException)
        {
            Project = project;
        }
        #endregion

        #region Properties
        public string Location { get; private set; }
        public IProject Project { get; private set; }
        #endregion
    }
}