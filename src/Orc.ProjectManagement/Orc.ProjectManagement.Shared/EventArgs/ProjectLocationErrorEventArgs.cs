// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectLocationErrorEventArgs.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System;
    using Catel.Data;

    public class ProjectLocationErrorEventArgs : ProjectLocationEventArgs
    {
        #region Constructors
        public ProjectLocationErrorEventArgs(string location, Exception exception = null, IValidationContext validationContext = null)
            : base(location)
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