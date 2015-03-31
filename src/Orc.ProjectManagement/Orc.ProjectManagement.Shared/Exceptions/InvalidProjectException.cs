// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvalidProjectException.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System;
    using Catel;

    public class InvalidProjectException : Exception
    {
        public InvalidProjectException(IProject project)
            : base(string.Format("Project '{0}' is invalid at this stage", ObjectToStringHelper.ToString(project)))
        {
            Project = project;
        }

        public IProject Project { get; private set; }
    }
}