// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvalidProjectException.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System;
    using Catel;

    public class InvalidProjectException : ProjectException
    {
        public InvalidProjectException(IProject project)
            : base(project, string.Format("Project '{0}' is invalid at this stage", ObjectToStringHelper.ToString(project)))
        {
        }
    }
}