// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectValidatorBase.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using Catel.Data;

    public abstract class ProjectValidatorBase : IProjectValidator
    {
        public virtual bool CanStartLoadingProject(string location)
        {
            return true;
        }

        public virtual IValidationContext ValidateProject(IProject project)
        {
            var validationContext = new ValidationContext();
            return validationContext;
        }
    }
}