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
            var validationContext = await base.ValidateProjectAsync(project);

            if (string.Equals(project.Location, "cannotload"))
            {
                validationContext.Add(BusinessRuleValidationResult.CreateError("this is a dummy error"));
            }

            return validationContext;
        }
        #endregion
    }
}