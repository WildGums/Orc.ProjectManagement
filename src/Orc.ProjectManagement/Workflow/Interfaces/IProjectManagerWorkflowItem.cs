namespace Orc.ProjectManagement
{
    using System;
    using System.Threading.Tasks;
    using Catel.Data;

    /// <summary>
    /// This is the replacement of all the already implemented ProjectWatcher instances
    /// The interface and its implementations will be deprecated in next stage of getting rid of ProjectWatcher conception
    /// </summary>
    public interface IProjectManagerWorkflowItem
    {
        Task<bool> LoadingAsync(string location);
        Task LoadingCanceledAsync(string location);
        Task LoadingFailedAsync(string location, Exception? exception, IValidationContext validationContext);
        Task LoadedAsync(IProject project);
        Task<bool> ActivationAsync(IProject? oldProject, IProject? newProject, bool isRefresh);
        Task ActivationCanceledAsync(IProject project);
        Task ActivationFailedAsync(IProject project, Exception? exception, IValidationContext validationContext);
        Task ActivatedAsync(IProject? oldProject, IProject? newProject);
        Task<bool> ClosingAsync(IProject project);
        Task ClosingCanceledAsync(IProject project);
        Task ClosedAsync(IProject project);
        Task<bool> RefreshingAsync(IProject project);
        Task RefreshingCanceledAsync(IProject project);
        Task RefreshingFailedAsync(IProject project, Exception? exception, IValidationContext validationContext);
        Task RefreshedAsync(IProject project);
        Task<bool> SavingAsync(IProject project);
        Task SavingCanceledAsync(IProject project);
        Task SavingFailedAsync(IProject project, Exception? exception, IValidationContext validationContext);
        Task SavedAsync(IProject project);
    }
}
