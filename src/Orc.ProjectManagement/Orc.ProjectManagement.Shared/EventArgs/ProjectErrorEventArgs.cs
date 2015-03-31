// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectEventArgs.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System;
    using Catel.Data;

    public class ProjectErrorEventArgs : ProjectEventArgs
    {
        public ProjectErrorEventArgs(string location, Exception exception = null, IValidationContext validationContext = null)
            : base(location)
        {
            Exception = exception;
            ValidationContext = validationContext ?? new ValidationContext();
        }

        public ProjectErrorEventArgs(IProject project, Exception exception = null, IValidationContext validationContext = null)
            : base(project)
        {
            Exception = exception;
            ValidationContext = validationContext ?? new ValidationContext();
        }

        public Exception Exception { get; private set; }

        public IValidationContext ValidationContext { get; private set; }
    }
}