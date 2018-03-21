// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SdiProjectManagementException.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System;

    public class SdiProjectManagementException : Exception
    {
        public SdiProjectManagementException()
            : this("Cannot load project, only one loaded project is allowed at the same time. Close the project or use a different IProjectConfigurationService.")
        {
        }

        public SdiProjectManagementException(string messageFormat, params object[] args)
            : this(string.Format(messageFormat, args))
        {
        }

        public SdiProjectManagementException(string message)
            : base(message)
        {
        }
    }
}