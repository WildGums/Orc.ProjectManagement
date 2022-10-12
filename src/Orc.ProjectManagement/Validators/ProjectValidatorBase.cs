namespace Orc.ProjectManagement
{
    using System.Threading.Tasks;
    using Catel.Data;

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
            return Task.FromResult(true);
        }

        public virtual Task<IValidationContext> ValidateProjectBeforeLoadingAsync(string location)
        {
            var validationContext = new ValidationContext();
            return Task.FromResult<IValidationContext>(validationContext);
        }

        public virtual Task<IValidationContext> ValidateProjectAsync(IProject project)
        {
            var validationContext = new ValidationContext();
            return Task.FromResult<IValidationContext>(validationContext);
        }
    }
}
