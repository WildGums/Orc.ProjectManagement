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
        public virtual bool ValidateLocationOnRefresh
        {
            get
            {
                return true;
            }
        }

        public virtual bool ValidateProjectOnRefresh
        {
            get
            {
                return true;
            }
        }

        public virtual Task<bool> CanStartLoadingProjectAsync(string location)
        {
            return TaskHelper<bool>.FromResult(true);
        }

        public virtual Task<IValidationContext> ValidateProjectBeforeLoadingAsync(string location)
        {
            var validationContext = new ValidationContext();
            return TaskHelper<IValidationContext>.FromResult(validationContext);
        }

        public virtual Task<IValidationContext> ValidateProjectAsync(IProject project)
        {
            var validationContext = new ValidationContext();
            return TaskHelper<IValidationContext>.FromResult(validationContext);
        }
    }
}
