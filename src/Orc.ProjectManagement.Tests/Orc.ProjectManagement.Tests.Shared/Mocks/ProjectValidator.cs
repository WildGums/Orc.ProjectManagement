// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectValidator.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
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
        public override async Task<bool> CanStartLoadingProjectAsync(string location)
        {
            return string.Equals(location, "cannotload", StringComparison.InvariantCultureIgnoreCase);
        }

        public override async Task<IValidationContext> ValidateProjectAsync(IProject project)
        {
            var validationContext = base.ValidateProject(project);

            if (string.Equals(project.Location, "cannotload"))
            {
                validationContext.AddBusinessRuleValidationResult(BusinessRuleValidationResult.CreateError("this is a dummy error"));
            }

            return validationContext;
        }
        #endregion
    }
}