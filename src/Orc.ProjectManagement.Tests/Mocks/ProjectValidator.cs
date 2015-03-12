// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectValidator.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement.Test.Mocks
{
    using System;
    using Catel.Data;

    public class ProjectValidator : ProjectValidatorBase
    {
        #region IProjectValidator Members
        public override bool CanStartLoadingProject(string location)
        {
            return string.Equals(location, "cannotload", StringComparison.InvariantCultureIgnoreCase);
        }

        public override IValidationContext ValidateProject(IProject project)
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