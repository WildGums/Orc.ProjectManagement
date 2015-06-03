// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectValidatorExtensions.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System.Threading.Tasks;
    using Catel.Data;
    using Catel.Threading;

    public static class IProjectValidatorExtensions
    {
        public static Task<bool> CanStartLoadingProjectAsync(this IProjectValidator projectValidator, string location)
        {
            return TaskHelper.Run(() => projectValidator.CanStartLoadingProject(location));
        }

        public static Task<IValidationContext> ValidateProjectAsync(this IProjectValidator projectValidator, IProject project)
        {
            return TaskHelper.Run(() => projectValidator.ValidateProject(project));
        }
    }
}