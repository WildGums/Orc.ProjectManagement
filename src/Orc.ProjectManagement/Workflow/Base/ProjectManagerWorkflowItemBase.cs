namespace Orc.ProjectManagement
{
    using System;
    using System.Threading.Tasks;
    using Catel.Data;

    public abstract class ProjectManagerWorkflowItemBase : IProjectManagerWorkflowItem
    {
        public virtual Task ActivatedAsync(IProject? oldProject, IProject? newProject)
        {
            return Task.CompletedTask;
        }

        public virtual Task<bool> ActivationAsync(IProject? oldProject, IProject? newProject, bool isRefresh)
        {
            return Task.FromResult(true);
        }

        public virtual Task ActivationCanceledAsync(IProject project)
        {
            return Task.CompletedTask;
        }

        public virtual Task ActivationFailedAsync(IProject project, Exception? exception, IValidationContext validationContext)
        {
            return Task.CompletedTask;
        }

        public virtual Task ClosedAsync(IProject project)
        {
            return Task.CompletedTask;
        }

        public virtual Task<bool> ClosingAsync(IProject project)
        {
            return Task.FromResult(true);
        }

        public virtual Task ClosingCanceledAsync(IProject project)
        {
            return Task.CompletedTask;
        }

        public virtual Task LoadedAsync(IProject project)
        {
            return Task.CompletedTask;
        }

        public virtual Task<bool> LoadingAsync(string location)
        {
            return Task.FromResult(true);
        }

        public virtual Task LoadingCanceledAsync(string location)
        {
            return Task.CompletedTask;
        }

        public virtual Task LoadingFailedAsync(string location, Exception? exception, IValidationContext validationContext)
        {
            return Task.CompletedTask;
        }

        public virtual Task RefreshedAsync(IProject project)
        {
            return Task.CompletedTask;
        }

        public virtual Task<bool> SavingAsync(IProject project)
        {
            return Task.FromResult(true);
        }

        public virtual Task SavingCanceledAsync(IProject project)
        {
            return Task.CompletedTask;
        }

        public virtual Task SavingFailedAsync(IProject project, Exception? exception, IValidationContext validationContext)
        {
            return Task.CompletedTask;
        }

        public virtual Task SavedAsync(IProject project)
        {
            return Task.CompletedTask;
        }

        public virtual Task<bool> RefreshingAsync(IProject project)
        {
            return Task.FromResult(true);
        }

        public virtual Task RefreshingCanceledAsync(IProject project)
        {
            return Task.CompletedTask;
        }

        public virtual Task RefreshingFailedAsync(IProject project, Exception? exception, IValidationContext validationContext)
        {
            return Task.CompletedTask;
        }
    }
}
