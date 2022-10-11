namespace Orc.ProjectManagement
{
    using System;
    using Catel.Data;

    public class ProjectErrorEventArgs : ProjectEventArgs
    {
        public ProjectErrorEventArgs(IProject project, Exception? exception = null, IValidationContext? validationContext = null)
            : base(project)
        {
            Exception = exception;
            ValidationContext = validationContext ?? new ValidationContext();
        }

        public Exception? Exception { get; private set; }
        public IValidationContext ValidationContext { get; private set; }
    }
}
