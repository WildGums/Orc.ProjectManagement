namespace Orc.ProjectManagement;

using System.Collections.Generic;
using System.Threading.Tasks;
using Catel;

public interface IProjectManager
{
    IProject[] Projects { get; }

    IProject? ActiveProject { get; }
    ProjectManagementType ProjectManagementType { get; }

    bool IsLoading { get; }

    event AsyncEventHandler<ProjectCancelEventArgs>? ProjectLoadingAsync;
    event AsyncEventHandler<ProjectErrorEventArgs>? ProjectLoadingFailedAsync;
    event AsyncEventHandler<ProjectLocationEventArgs>? ProjectLoadingCanceledAsync;
    event AsyncEventHandler<ProjectEventArgs>? ProjectLoadedAsync;

    event AsyncEventHandler<ProjectCancelEventArgs>? ProjectSavingAsync;
    event AsyncEventHandler<ProjectErrorEventArgs>? ProjectSavingFailedAsync;
    event AsyncEventHandler<ProjectEventArgs>? ProjectSavingCanceledAsync;
    event AsyncEventHandler<ProjectEventArgs>? ProjectSavedAsync;

    event AsyncEventHandler<ProjectEventArgs>? ProjectRefreshRequiredAsync;
    event AsyncEventHandler<ProjectCancelEventArgs>? ProjectRefreshingAsync;
    event AsyncEventHandler<ProjectEventArgs>? ProjectRefreshedAsync;
    event AsyncEventHandler<ProjectRefreshErrorEventArgs>? ProjectRefreshingCanceledAsync;
    event AsyncEventHandler<ProjectErrorEventArgs>? ProjectRefreshingFailedAsync;

    event AsyncEventHandler<ProjectCancelEventArgs>? ProjectClosingAsync;
    event AsyncEventHandler<ProjectEventArgs>? ProjectClosingCanceledAsync;
    event AsyncEventHandler<ProjectEventArgs>? ProjectClosedAsync;

    event AsyncEventHandler<ProjectUpdatingCancelEventArgs>? ProjectActivationAsync;
    event AsyncEventHandler<ProjectUpdatedEventArgs>? ProjectActivatedAsync;
    event AsyncEventHandler<ProjectActivationEventArgs>? ProjectActivationCanceledAsync;
    event AsyncEventHandler<ProjectErrorEventArgs>? ProjectActivationFailedAsync;

    Task InitializeAsync();
    [ObsoleteEx(Message = "Use extension method", ReplacementTypeOrMember = "RefreshActiveProjectAsync", RemoveInVersion = "6.0.0")]
    Task<bool> RefreshAsync();
    Task<bool> RefreshAsync(IProject project);
    Task<bool> LoadAsync(string location);
    Task<bool> LoadInactiveAsync(string location);
    [ObsoleteEx(Message = "Use extension method", ReplacementTypeOrMember = "SaveActiveProjectAsync", RemoveInVersion = "6.0.0")]
    Task<bool> SaveAsync(string? location = null);
    Task<bool> SaveAsync(IProject project, string? location = null);
    [ObsoleteEx(Message = "Use extension method", ReplacementTypeOrMember = "CloseActiveProjectAsync", RemoveInVersion = "6.0.0")]
    Task<bool> CloseAsync();
    Task<bool> CloseAsync(IProject project);
    Task<bool> SetActiveProjectAsync(IProject? project);
}
