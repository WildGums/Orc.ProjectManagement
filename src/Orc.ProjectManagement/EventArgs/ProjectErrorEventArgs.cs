// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectEventArgs.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System;

    public class ProjectErrorEventArgs : ProjectEventArgs
    {
        public ProjectErrorEventArgs(string location, Exception exception = null)
            : base(location)
        {
            Exception = exception;
        }

        public ProjectErrorEventArgs(IProject project, Exception exception = null)
            : base(project)
        {
            Exception = exception;
        }

        public Exception Exception { get; private set; }
    }
}