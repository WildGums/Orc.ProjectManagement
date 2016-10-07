// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectValidator.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System.Threading.Tasks;
    using Catel.Data;

    public interface IProjectValidator
    {
        [ObsoleteEx(ReplacementTypeOrMember = "CanStartLoadingProjectAsync", TreatAsErrorFromVersion = "1.2", RemoveInVersion = "2.0")]
        bool CanStartLoadingProject(string location);

        /// <summary>
        /// Determines whether the location exists and is ready to load a project from.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> CanStartLoadingProjectAsync(string location);

        /// <summary>
        /// Called when <see cref="CanStartLoadingProjectAsync"/> returns <c>true</c>, but before actually loading the project. This
        /// can be used to do validation of data files before actually reading the data into memory.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns>Task&lt;IValidationContext&gt;.</returns>
        Task<IValidationContext> ValidateProjectBeforeLoadingAsync(string location);

        [ObsoleteEx(ReplacementTypeOrMember = "ValidateProjectAsync", TreatAsErrorFromVersion = "1.2", RemoveInVersion = "2.0")]
        IValidationContext ValidateProject(IProject project);

        Task<IValidationContext> ValidateProjectAsync(IProject project);
    }
}