// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectValidator.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
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