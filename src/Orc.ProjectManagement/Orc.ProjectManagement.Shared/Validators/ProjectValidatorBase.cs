// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectValidatorBase.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System.Threading.Tasks;
    using Catel.Data;
    using Catel.Threading;

    public abstract class ProjectValidatorBase : IProjectValidator
    {
        [ObsoleteEx(ReplacementTypeOrMember = "CanStartLoadingProjectAsync", TreatAsErrorFromVersion = "1.2", RemoveInVersion = "2.0")]
        public virtual bool CanStartLoadingProject(string location)
        {
            return true;
        }

        public virtual Task<bool> CanStartLoadingProjectAsync(string location)
        {
            var canStart = CanStartLoadingProject(location);
            return TaskHelper<bool>.FromResult(canStart);
        }

        [ObsoleteEx(ReplacementTypeOrMember = "ValidateProjectAsync", TreatAsErrorFromVersion = "1.2", RemoveInVersion = "2.0")]
        public virtual IValidationContext ValidateProject(IProject project)
        {
            var validationContext = new ValidationContext();
            return validationContext;
        }

        public virtual Task<IValidationContext> ValidateProjectBeforeLoadingAsync(string location)
        {
            var validationContext = new ValidationContext();
            return TaskHelper<IValidationContext>.FromResult(validationContext);
        }

        public virtual Task<IValidationContext> ValidateProjectAsync(IProject project)
        {
            var validationContext = ValidateProject(project);
            return TaskHelper<IValidationContext>.FromResult(validationContext);
        }
    }
}