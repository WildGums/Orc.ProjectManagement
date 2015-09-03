// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectValidator.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using Catel.Data;

    public interface IProjectValidator
    {
        bool CanStartLoadingProject(string location);

        IValidationContext ValidateProject(IProject project);
    }
}