// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectErrorEventArgs.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System;
    using Catel.Data;

    public class ProjectErrorEventArgs : ProjectEventArgs
    {
        #region Constructors
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
        #endregion

        #region Properties
        public Exception Exception { get; private set; }
        public IValidationContext ValidationContext { get; private set; }
        #endregion
    }
}