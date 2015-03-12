// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectValidatorExtensions.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System.Threading.Tasks;
    using Catel.Data;

    public static class IProjectValidatorExtensions
    {
        public static async Task<bool> CanStartLoadingProjectAsync(this IProjectValidator projectValidator, string location)
        {
            return await Task.Factory.StartNew(() => projectValidator.CanStartLoadingProject(location));
        }

        public static async Task<IValidationContext> ValidateProjectAsync(this IProjectValidator projectValidator, IProject project)
        {
            return await Task.Factory.StartNew(() => projectValidator.ValidateProject(project));
        }
    }
}