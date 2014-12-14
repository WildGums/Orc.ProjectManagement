// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectValidator.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement.Test.Mocks
{
    using System;
    using System.Threading.Tasks;
    using Catel.Data;

    public class ProjectValidator : ProjectValidatorBase
    {
        #region IProjectValidator Members
        public override async Task<bool> CanStartLoadingProject(string location)
        {
            return string.Equals(location, "cannotload", StringComparison.InvariantCultureIgnoreCase);
        }

        public override async Task<IValidationContext> ValidateProject(IProject project)
        {
            var validationContext = await base.ValidateProject(project);

            if (string.Equals(project.Location, "cannotload"))
            {
                validationContext.AddBusinessRuleValidationResult(BusinessRuleValidationResult.CreateError("this is a dummy error"));
            }

            return validationContext;
        }
        #endregion
    }
}