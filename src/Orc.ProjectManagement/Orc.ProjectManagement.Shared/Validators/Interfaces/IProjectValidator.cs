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

        Task<bool> CanStartLoadingProjectAsync(string location);

        [ObsoleteEx(ReplacementTypeOrMember = "ValidateProjectAsync", TreatAsErrorFromVersion = "1.2", RemoveInVersion = "2.0")]
        IValidationContext ValidateProject(IProject project);

        Task<IValidationContext> ValidateProjectAsync(IProject project);
    }
}