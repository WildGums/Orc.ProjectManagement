namespace Orc.ProjectManagement;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Catel;
using Catel.Data;
using Catel.IoC;
using Catel.Logging;
using Catel.Reflection;
using Catel.Threading;

public class ProjectManager : IProjectManager, INeedCustomInitialization
{
    private const int DefaultTimeout = 3000;

    private static readonly ILog Log = LogManager.GetCurrentClassLogger();

    private readonly IProjectInitializer _projectInitializer;
    private readonly IProjectManagementInitializationService _projectManagementInitializationService;
    private readonly IProjectStateSetter _projectStateSetter;
    private readonly IProjectStateService _projectStateService;
    private readonly IProjectRefresherSelector _projectRefresherSelector;
    private readonly IProjectSerializerSelector _projectSerializerSelector;
    private readonly IProjectValidator _projectValidator;
    private readonly IProjectUpgrader _projectUpgrader;

    private readonly ConcurrentDictionary<string, AsyncLock> _projectOperationLockers = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, int> _projectOperationRefCounts = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, IProjectRefresher> _projectRefreshers;
    private readonly ConcurrentDictionary<string, IProject> _projects;
    private readonly ConcurrentDictionary<string, object> _loadingProjects = new();
    private readonly ConcurrentDictionary<string, object> _savingProjects = new();
    private readonly ConcurrentDictionary<string, Task> _activeRefreshTasks = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, ProjectResources> _projectResources = new(StringComparer.OrdinalIgnoreCase);

    private readonly AsyncLock _activeProjectLock = new();
    private readonly AsyncLock _commonLock = new();
    private readonly AsyncLock _refreshLock = new ();
    private readonly AsyncLock _eventLock = new();
    private readonly AsyncLock _refresherManagementLock = new();

    public ProjectManager(IProjectValidator projectValidator, IProjectUpgrader projectUpgrader, IProjectRefresherSelector projectRefresherSelector,
        IProjectSerializerSelector projectSerializerSelector, IProjectInitializer projectInitializer, IProjectManagementConfigurationService projectManagementConfigurationService,
        IProjectManagementInitializationService projectManagementInitializationService, IProjectStateService projectStateService)
    {
        ArgumentNullException.ThrowIfNull(projectValidator);
        ArgumentNullException.ThrowIfNull(projectUpgrader);
        ArgumentNullException.ThrowIfNull(projectRefresherSelector);
        ArgumentNullException.ThrowIfNull(projectSerializerSelector);
        ArgumentNullException.ThrowIfNull(projectInitializer);
        ArgumentNullException.ThrowIfNull(projectManagementConfigurationService);
        ArgumentNullException.ThrowIfNull(projectManagementInitializationService);
        ArgumentNullException.ThrowIfNull(projectStateService);

        _projectValidator = projectValidator;
        _projectUpgrader = projectUpgrader;
        _projectRefresherSelector = projectRefresherSelector;
        _projectSerializerSelector = projectSerializerSelector;
        _projectInitializer = projectInitializer;
        _projectManagementInitializationService = projectManagementInitializationService;
        _projectStateSetter = (IProjectStateSetter)projectStateService;
        _projectStateService = projectStateService;

        _projects = new ConcurrentDictionary<string, IProject>();
        _projectRefreshers = new ConcurrentDictionary<string, IProjectRefresher>(StringComparer.OrdinalIgnoreCase);

        ProjectManagementType = projectManagementConfigurationService.GetProjectManagementType();
    }

    public ProjectManagementType ProjectManagementType { get; }

    public virtual IProject[] Projects
    {
        get { return _projects.Select(x => x.Value).ToArray(); }
    }

    public virtual IProject? ActiveProject { get; set; }

    public bool IsLoading => _loadingProjects.Any();

    void INeedCustomInitialization.Initialize()
    {
        _projectManagementInitializationService.Initialize(this);
    }

    public event AsyncEventHandler<ProjectCancelEventArgs>? ProjectLoadingAsync;
    public event AsyncEventHandler<ProjectErrorEventArgs>? ProjectLoadingFailedAsync;
    public event AsyncEventHandler<ProjectLocationEventArgs>? ProjectLoadingCanceledAsync;
    public event AsyncEventHandler<ProjectEventArgs>? ProjectLoadedAsync;

    public event AsyncEventHandler<ProjectCancelEventArgs>? ProjectSavingAsync;
    public event AsyncEventHandler<ProjectErrorEventArgs>? ProjectSavingFailedAsync;
    public event AsyncEventHandler<ProjectEventArgs>? ProjectSavingCanceledAsync;
    public event AsyncEventHandler<ProjectEventArgs>? ProjectSavedAsync;

    public event AsyncEventHandler<ProjectEventArgs>? ProjectRefreshRequiredAsync;
    public event AsyncEventHandler<ProjectCancelEventArgs>? ProjectRefreshingAsync;
    public event AsyncEventHandler<ProjectEventArgs>? ProjectRefreshedAsync;
    public event AsyncEventHandler<ProjectRefreshErrorEventArgs>? ProjectRefreshingCanceledAsync;
    public event AsyncEventHandler<ProjectErrorEventArgs>? ProjectRefreshingFailedAsync;

    public event AsyncEventHandler<ProjectCancelEventArgs>? ProjectClosingAsync;
    public event AsyncEventHandler<ProjectEventArgs>? ProjectClosingCanceledAsync;
    public event AsyncEventHandler<ProjectEventArgs>? ProjectClosedAsync;

    public event AsyncEventHandler<ProjectUpdatingCancelEventArgs>? ProjectActivationAsync;
    public event AsyncEventHandler<ProjectUpdatedEventArgs>? ProjectActivatedAsync;
    public event AsyncEventHandler<ProjectActivationEventArgs>? ProjectActivationCanceledAsync;
    public event AsyncEventHandler<ProjectErrorEventArgs>? ProjectActivationFailedAsync;

    protected virtual async Task RaiseEventAsync(IProjectEventType projectEventType)
    {
        switch (projectEventType)
        {
            case ProjectLoadEvent projectLoadEvent:
                switch (projectLoadEvent.Stage)
                {
                    case ProjectEventTypeStage.Before:
                        await SafeInvokeWithTimeoutAsync(ProjectLoadingAsync, nameof(ProjectLoadingAsync), this, (ProjectCancelEventArgs)projectLoadEvent.EventArgs, DefaultTimeout)
                            .ConfigureAwait(false);
                        break;

                    case ProjectEventTypeStage.After:
                        await SafeInvokeWithTimeoutAsync(ProjectLoadedAsync, nameof(ProjectLoadedAsync), this, (ProjectEventArgs)projectLoadEvent.EventArgs, DefaultTimeout)
                            .ConfigureAwait(false);
                        break;

                    case ProjectEventTypeStage.Cancelled:
                        await SafeInvokeWithTimeoutAsync(ProjectLoadingCanceledAsync, nameof(ProjectLoadingCanceledAsync), this, (ProjectLocationEventArgs)projectLoadEvent.EventArgs, DefaultTimeout)
                            .ConfigureAwait(false);
                        break;

                    case ProjectEventTypeStage.Failed:
                        await SafeInvokeWithTimeoutAsync(ProjectLoadingFailedAsync, nameof(ProjectLoadingFailedAsync), this, (ProjectErrorEventArgs)projectLoadEvent.EventArgs, DefaultTimeout)
                            .ConfigureAwait(false);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
                break;
            case ProjectSaveEvent projectSaveEvent:
                switch (projectSaveEvent.Stage)
                {
                    case ProjectEventTypeStage.Before:
                        await SafeInvokeWithTimeoutAsync(ProjectSavingAsync, nameof(ProjectSavingAsync), this, (ProjectCancelEventArgs)projectSaveEvent.EventArgs, DefaultTimeout)
                            .ConfigureAwait(false);
                        break;

                    case ProjectEventTypeStage.After:
                        await SafeInvokeWithTimeoutAsync(ProjectSavedAsync, nameof(ProjectSavedAsync), this, (ProjectEventArgs)projectSaveEvent.EventArgs, DefaultTimeout)
                            .ConfigureAwait(false);
                        break;

                    case ProjectEventTypeStage.Cancelled:
                        await SafeInvokeWithTimeoutAsync(ProjectSavingCanceledAsync, nameof(ProjectSavingCanceledAsync), this, (ProjectEventArgs)projectSaveEvent.EventArgs, DefaultTimeout)
                            .ConfigureAwait(false);
                        break;

                    case ProjectEventTypeStage.Failed:
                        await SafeInvokeWithTimeoutAsync(ProjectSavingFailedAsync, nameof(ProjectSavingFailedAsync), this, (ProjectErrorEventArgs)projectSaveEvent.EventArgs, DefaultTimeout)
                            .ConfigureAwait(false);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
                break;
            case ProjectRefreshEvent projectRefreshEvent:
                switch (projectRefreshEvent.Stage)
                {
                    case ProjectEventTypeStage.Required:
                        await SafeInvokeWithTimeoutAsync(ProjectRefreshRequiredAsync, nameof(ProjectRefreshRequiredAsync), this, (ProjectEventArgs)projectRefreshEvent.EventArgs, DefaultTimeout)
                            .ConfigureAwait(false);
                        break;

                    case ProjectEventTypeStage.Before:
                        await SafeInvokeWithTimeoutAsync(ProjectRefreshingAsync, nameof(ProjectRefreshingAsync), this, (ProjectCancelEventArgs)projectRefreshEvent.EventArgs, DefaultTimeout)
                            .ConfigureAwait(false);
                        break;

                    case ProjectEventTypeStage.After:
                        await SafeInvokeWithTimeoutAsync(ProjectRefreshedAsync, nameof(ProjectRefreshedAsync), this, (ProjectEventArgs)projectRefreshEvent.EventArgs, DefaultTimeout)
                            .ConfigureAwait(false);
                        break;

                    case ProjectEventTypeStage.Cancelled:
                        await SafeInvokeWithTimeoutAsync(ProjectRefreshingCanceledAsync, nameof(ProjectRefreshingCanceledAsync), this, (ProjectRefreshErrorEventArgs)projectRefreshEvent.EventArgs, DefaultTimeout)
                            .ConfigureAwait(false);
                        break;

                    case ProjectEventTypeStage.Failed:
                        await SafeInvokeWithTimeoutAsync(ProjectRefreshingFailedAsync, nameof(ProjectRefreshingFailedAsync), this, (ProjectErrorEventArgs)projectRefreshEvent.EventArgs, DefaultTimeout)
                            .ConfigureAwait(false);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
                break;
            case ProjectCloseEvent projectCloseEvent:
                switch (projectCloseEvent.Stage)
                {
                    case ProjectEventTypeStage.Before:
                        await SafeInvokeWithTimeoutAsync(ProjectClosingAsync, nameof(ProjectClosingAsync), this, (ProjectCancelEventArgs)projectCloseEvent.EventArgs, DefaultTimeout)
                            .ConfigureAwait(false);
                        break;

                    case ProjectEventTypeStage.After:
                        await SafeInvokeWithTimeoutAsync(ProjectClosedAsync, nameof(ProjectClosedAsync), this, (ProjectEventArgs)projectCloseEvent.EventArgs, DefaultTimeout)
                            .ConfigureAwait(false);
                        break;

                    case ProjectEventTypeStage.Cancelled:
                        await SafeInvokeWithTimeoutAsync(ProjectClosingCanceledAsync, nameof(ProjectClosingCanceledAsync), this, (ProjectEventArgs)projectCloseEvent.EventArgs, DefaultTimeout)
                            .ConfigureAwait(false);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
                break;
            case ProjectActivationEvent projectActivationEvent:
                switch (projectActivationEvent.Stage)
                {
                    case ProjectEventTypeStage.Before:
                        await SafeInvokeWithTimeoutAsync(ProjectActivationAsync, nameof(ProjectActivationAsync), this, (ProjectUpdatingCancelEventArgs)projectActivationEvent.EventArgs, DefaultTimeout)
                            .ConfigureAwait(false);
                        break;

                    case ProjectEventTypeStage.After:
                        await SafeInvokeWithTimeoutAsync(ProjectActivatedAsync, nameof(ProjectActivatedAsync), this, (ProjectUpdatedEventArgs)projectActivationEvent.EventArgs, DefaultTimeout)
                            .ConfigureAwait(false);
                        break;

                    case ProjectEventTypeStage.Cancelled:
                        await SafeInvokeWithTimeoutAsync(ProjectActivationCanceledAsync,nameof(ProjectActivationCanceledAsync), this, (ProjectActivationEventArgs)projectActivationEvent.EventArgs, DefaultTimeout)
                            .ConfigureAwait(false);
                        break;

                    case ProjectEventTypeStage.Failed:
                        await SafeInvokeWithTimeoutAsync(ProjectActivationFailedAsync, nameof(ProjectActivationFailedAsync), this, (ProjectErrorEventArgs)projectActivationEvent.EventArgs, DefaultTimeout)
                            .ConfigureAwait(false);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> SafeInvokeWithTimeoutAsync<TEventArgs>(
        AsyncEventHandler<TEventArgs>? handler,
        string eventName,
        object sender,
        TEventArgs e,
        int timeout) where TEventArgs : EventArgs
    {
        if (handler is null)
        {
            return false;
        }

        try
        {
            using var cts = new CancellationTokenSource(timeout);

            Log.DebugIfAttached($"Starting to handle project management event '{eventName}'");

            var task = SafeInvokeAsync(eventName, handler, sender, e);

            try
            {
                await task.WaitAsync(cts.Token).ConfigureAwait(false);
                Log.DebugIfAttached($"Successfully handled project management event '{eventName}'");
                return true;
            }
            catch (OperationCanceledException)
            {
                Log.Warning($"Project management event '{eventName}' timed out after {timeout}ms. " +
                            "This might indicate a deadlock or performance issue with event handlers.");
                return false;
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Failed to handle project management event '{eventName}'");
            throw;
        }
    }

    private async Task<bool> SafeInvokeAsync<TEventArgs>(string eventName, AsyncEventHandler<TEventArgs>? handler, object sender, TEventArgs e)
        where TEventArgs : EventArgs
    {
        if (handler is null)
        {
            return false;
        }

        // Get all handlers upfront to avoid collection modification issues
        var eventListeners = handler.GetInvocationList().Cast<AsyncEventHandler<TEventArgs>>().ToArray();

        // Lock for the entire event handling sequence to ensure sequential execution
        using (await _eventLock.LockAsync())
        {
            for (var i = 0; i < eventListeners.Length; i++)
            {
                var eventListener = eventListeners[i];
                try
                {
                    Log.DebugIfAttached($"Executing event handler [{i + 1}/{eventListeners.Length}]: " +
                                        $"target '{eventListener.Target}', method '{eventListener.Method.Name}'");

                    // Execute handlers sequentially within the lock
                    await InvokeEventListenerAsync(eventName, sender, e, eventListener)
                        .ConfigureAwait(false);

                    Log.DebugIfAttached($"Event handler [{i + 1}/{eventListeners.Length}] successfully executed: " +
                                        $"target '{eventListener.Target}', method '{eventListener.Method.Name}'");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Failed to invoke event handler [{i + 1}/{eventListeners.Length}]: " +
                                  $"target '{eventListener.Target}', method '{eventListener.Method.Name}'");
                    throw;
                }
            }

            return true;
        }
    }

    protected virtual async Task InvokeEventListenerAsync<TEventArgs>(
        string eventName,
        object sender,
        TEventArgs args,
        AsyncEventHandler<TEventArgs> eventListener)
        where TEventArgs : EventArgs
    {
        try
        {
            await eventListener(sender, args).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            Log.Warning($"Event handler cancelled for '{eventName}', this may affect project state consistency");
            throw;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Event handler failed for '{eventName}', this may affect project state consistency");
            throw;
        }
    }

    public virtual async Task InitializeAsync()
    {
        var locations = (from location in await _projectInitializer.GetInitialLocationsAsync()
            where !string.IsNullOrWhiteSpace(location)
            select location);

        foreach (var location in locations)
        {
            Log.Debug("Loading initial project from location '{0}'", location);
            await LoadAsync(location).ConfigureAwait(false);
        }
    }

    [ObsoleteEx(Message = "Use extension method", ReplacementTypeOrMember = "RefreshActiveProjectAsync", RemoveInVersion = "6.0.0")]
    public Task<bool> RefreshAsync()
    {
        var project = ActiveProject;

        return project is null
            ? Task.FromResult(false)
            : RefreshAsync(project);
    }

    public virtual Task<bool> RefreshAsync(IProject project)
    {
        ArgumentNullException.ThrowIfNull(project);

        return SynchronizeProjectOperationAsync(project.Location, () => SyncedRefreshAsync(project));
    }

    public virtual Task<bool> LoadAsync(string location)
    {
        Argument.IsNotNullOrWhitespace("location", location);

        return SynchronizeProjectOperationAsync(location, async () =>
        {
            var project = await SyncedLoadProjectAsync(location);

            if (project is not null)
            {
                await SetActiveProjectAsync(project);
            }

            return project is not null;
        });
    }

    public virtual Task<bool> LoadInactiveAsync(string location)
    {
        Argument.IsNotNullOrWhitespace("location", location);

        return SynchronizeProjectOperationAsync(location, async () =>
        {
            var project = await SyncedLoadProjectAsync(location);

            return project is not null;
        });
    }

    [ObsoleteEx(Message = "Use extension method", ReplacementTypeOrMember = "SaveActiveProjectAsync", RemoveInVersion = "6.0.0")]
    public Task<bool> SaveAsync(string? location = null)
    {
        var project = ActiveProject;
        if (project is null)
        {
            Log.Warning("Cannot save empty project");
            return Task.FromResult(false);
        }

        return SaveAsync(project, location);
    }

    public virtual async Task<bool> SaveAsync(IProject project, string? location = null)
    {
        if (string.IsNullOrWhiteSpace(location))
        {
            location = project.Location;
        }

        var state = _projectStateService.GetProjectState(project);
        if (state.IsClosing || state.IsRefreshing)
        {
            return await SyncedSaveAsync(project, location);
        }

        return await SynchronizeProjectOperationAsync(location, () => SyncedSaveAsync(project, location));
    }

    [ObsoleteEx(Message = "Use extension method", ReplacementTypeOrMember = "CloseActiveProjectAsync", RemoveInVersion = "6.0.0")]
    public Task<bool> CloseAsync()
    {
        var project = ActiveProject;

        return project is null
            ? Task.FromResult(false)
            : CloseAsync(project);
    }

    public virtual Task<bool> CloseAsync(IProject project)
    {
        ArgumentNullException.ThrowIfNull(project);

        var location = project.Location;

        return SynchronizeProjectOperationAsync(location, () => SyncedCloseAsync(project));
    }

    public virtual async Task<bool> SetActiveProjectAsync(IProject? project)
    {
        // Acquire locks in correct order
        using (await AcquireLocksAsync(_commonLock, _activeProjectLock))
        {
            var activeProject = ActiveProject;
            if (project is not null && !Projects.Contains(project))
            {
                return false;
            }

            var activeProjectLocation = activeProject?.Location;
            var newProjectLocation = project?.Location;

            if (string.Equals(activeProjectLocation, newProjectLocation))
            {
                return false;
            }

            Log.Info(project is not null
                ? $"Activating project '{project.Location}'"
                : "Deactivating currently active project");

            using var deactivatingState = activeProject is not null
                ? new ProjectStateScope(_projectStateSetter, activeProjectLocation, ProjectStateType.Deactivating)
                : null;
            using var activatingState = project is not null
                ? new ProjectStateScope(_projectStateSetter, newProjectLocation, ProjectStateType.Activating)
                : null;

            try
            {
                var eventArgs = new ProjectUpdatingCancelEventArgs(activeProject, project);
                await RaiseEventAsync(new ProjectActivationEvent(ProjectEventTypeStage.Before, eventArgs))
                    .ConfigureAwait(false);

                if (eventArgs.Cancel)
                {
                    return false;
                }

                ActiveProject = project;

                deactivatingState?.Commit();
                activatingState?.Commit();

                await RaiseEventAsync(new ProjectActivationEvent(ProjectEventTypeStage.After,
                    new ProjectUpdatedEventArgs(activeProject, project))).ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to change active project");
                await RaiseEventAsync(new ProjectActivationEvent(ProjectEventTypeStage.Failed,
                    new ProjectErrorEventArgs(project, ex))).ConfigureAwait(false);
                return false;
            }
        }
    }

    protected virtual async Task<IProject?> ReadProjectAsync(string location)
    {
        var projectReader = _projectSerializerSelector.GetReader(location);
        if (projectReader is null)
        {
            throw Log.ErrorAndCreateException<InvalidOperationException>($"No project reader is found for location '{location}'");
        }

        Log.Debug("Using project reader '{0}'", projectReader.GetType().Name);

        var project = await projectReader.ReadAsync(location).ConfigureAwait(false);

        return project;
    }

    protected virtual Task<bool> WriteProjectAsync(IProject project, string location)
    {
        var projectWriter = _projectSerializerSelector.GetWriter(location);
        if (projectWriter is null)
        {
            throw Log.ErrorAndCreateException<NotSupportedException>($"No project writer is found for location '{location}'");
        }

        Log.Debug("Using project writer '{0}'", projectWriter.GetType().Name);

        return projectWriter.WriteAsync(project, location);
    }

    private async Task<T> SynchronizeProjectOperationAsync<T>(string projectLocation, Func<Task<T>> operation)
    {
        Argument.IsNotNullOrEmpty(() => projectLocation);

        using (await _commonLock.LockAsync())
        {
            AsyncLock? projectLock;
            int refCount;

            if (!_projectOperationLockers.TryGetValue(projectLocation, out projectLock))
            {
                projectLock = new AsyncLock();
                _projectOperationLockers.TryAdd(projectLocation, projectLock);
            }

            refCount = _projectOperationRefCounts.GetValueOrDefault(projectLocation, 0);
            refCount++;
            _projectOperationRefCounts[projectLocation] = refCount;

            try
            {
                using (await projectLock.LockAsync())
                {
                    Log.Debug($"Start synchronized operation for '{projectLocation}' refCount = {refCount}]");
                    return await operation().ConfigureAwait(false);
                }
            }
            finally
            {
                await ReleaseSynchronizationContextAsync(projectLocation);
            }
        }
    }
    private async Task ReleaseSynchronizationContextAsync(string projectLocation)
    {
        Log.Debug($"Releasing operation synchronization context for '{projectLocation}'");

        using (await _commonLock.LockAsync())
        {
            var refCount = _projectOperationRefCounts.GetValueOrDefault(projectLocation, 0);
            refCount--;

            if (refCount >= 1)
            {
                _projectOperationRefCounts[projectLocation] = refCount;
            }
            else
            {
                _projectOperationRefCounts.TryRemove(projectLocation, out _);
                _projectOperationLockers.TryRemove(projectLocation, out _);
            }
        }

        Log.Debug($"Released operation synchronization context for '{projectLocation}'");
    }

    private async Task<bool> SyncedRefreshAsync(IProject project)
    {
        var projectLocation = project.Location;
        var activeProjectLocation = this.GetActiveProjectLocation();
        if (string.IsNullOrEmpty(activeProjectLocation))
        {
            return false;
        }

        Log.Debug("Refreshing project from '{0}'", projectLocation);

        var isRefreshingActiveProject = activeProjectLocation.EndsWithIgnoreCase(projectLocation);

        _projectStateSetter.SetProjectRefreshing(projectLocation, true, isRefreshingActiveProject);

        var cancelEventArgs = new ProjectCancelEventArgs(project);
        await RaiseEventAsync(new ProjectRefreshEvent(ProjectEventTypeStage.Before, cancelEventArgs)).ConfigureAwait(false);

        Exception? error = null;
        IValidationContext? validationContext = null;

        try
        {
            if (cancelEventArgs.Cancel)
            {
                _projectStateSetter.SetProjectRefreshing(projectLocation, false);

                await RaiseEventAsync(new ProjectRefreshEvent(ProjectEventTypeStage.Cancelled, new ProjectLocationEventArgs(projectLocation))).ConfigureAwait(false);
                return false;
            }

            UnregisterProject(project);

            if (isRefreshingActiveProject)
            {
                await SetActiveProjectAsync(null).ConfigureAwait(false);
            }

            if (_projectValidator.ValidateLocationOnRefresh)
            {
                validationContext = await _projectValidator.ValidateProjectBeforeLoadingAsync(projectLocation);
                if (validationContext.HasErrors)
                {
                    throw Log.ErrorAndCreateException<InvalidOperationException>($"Project could not be loaded from '{projectLocation}', the validator returned errors");
                }
            }

            var loadedProject = await QuietlyLoadProjectAsync(projectLocation, false).ConfigureAwait(false);
            if (loadedProject is null) 
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>($"Failed to load project from '{projectLocation}'");
            }

            if (_projectValidator.ValidateProjectOnRefresh)
            {
                validationContext = await _projectValidator.ValidateProjectAsync(loadedProject);
                if (validationContext.HasErrors)
                {
                    throw Log.ErrorAndCreateException<InvalidOperationException>($"Project data was loaded from '{projectLocation}', but the validator returned errors");
                }
            }

            RegisterProject(loadedProject);

            // Note: we disable IsRefreshingActiveProject at Activated event, that is why isActiveProject == false
            _projectStateSetter.SetProjectRefreshing(projectLocation, true, false);

            await RaiseEventAsync(new ProjectRefreshEvent(ProjectEventTypeStage.After, new ProjectEventArgs(loadedProject))).ConfigureAwait(false); ;

            if (isRefreshingActiveProject)
            {
                await SetActiveProjectAsync(loadedProject).ConfigureAwait(false);
            }

            Log.Info("Refreshed project from '{0}'", projectLocation);
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Failed to load project from '{0}'", projectLocation);

            error = ex;
        }

        if (error is null)
        {
            return true;
        }

        _projectStateSetter.SetProjectRefreshing(projectLocation, false);

        var eventArgs = new ProjectErrorEventArgs(project,
            new ProjectException(project, $"Failed to load project from location '{projectLocation}' while refreshing.", error),
            validationContext);

        await RaiseEventAsync(new ProjectRefreshEvent(ProjectEventTypeStage.Failed, eventArgs)).ConfigureAwait(false);

        return false;
    }

    private async Task<IProject?> SyncedLoadProjectAsync(string location)
    {
        Argument.IsNotNullOrWhitespace("location", location);

        var project = Projects.FirstOrDefault(x => location.EqualsIgnoreCase(x.Location));
        if (project is not null)
        {
            return project;
        }

        var projectLocation = location;
        using (new DisposableToken(location,
            _ => _loadingProjects.TryAdd(projectLocation, new object()),
            _ => _loadingProjects.TryRemove(projectLocation, out var _)))
        {
            using var loadingState = new ProjectStateScope(_projectStateSetter, location, ProjectStateType.Loading);

            try
            {
                Log.Debug($"Going to load project from '{location}', checking if an upgrade is required");

                if (await _projectUpgrader.RequiresUpgradeAsync(location))
                {
                    Log.Debug($"Upgrade is required for '{location}', upgrading...");
                    location = await _projectUpgrader.UpgradeAsync(location);
                    Log.Debug($"Upgraded project, final location is '{location}'");
                }

                var cancelEventArgs = new ProjectCancelEventArgs(location);
                await RaiseEventAsync(new ProjectLoadEvent(ProjectEventTypeStage.Before, cancelEventArgs))
                    .ConfigureAwait(false);

                if (cancelEventArgs.Cancel)
                {
                    Log.Debug("Canceled loading of project from '{0}'", location);
                    await RaiseEventAsync(new ProjectLoadEvent(ProjectEventTypeStage.Cancelled,
                        new ProjectLocationEventArgs(location))).ConfigureAwait(false);
                    return null;
                }

                // Validate and load the project
                await ValidateProjectManagementTypeAsync(location);
                var validationContext = await ValidateAndPrepareProjectLoadingAsync(location);
                project = await LoadAndValidateProjectAsync(location, validationContext);

                RegisterProject(project);

                // Only commit the state if everything succeeded
                loadingState.Commit();

                await RaiseEventAsync(new ProjectLoadEvent(ProjectEventTypeStage.After,
                    new ProjectEventArgs(project))).ConfigureAwait(false);

                Log.Info("Loaded project from '{0}'", location);
                return project;
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to load project from '{0}'", location);

                await RaiseEventAsync(new ProjectLoadEvent(ProjectEventTypeStage.Failed,
                    new ProjectErrorEventArgs(location, ex))).ConfigureAwait(false);

                return null;
            }
        }
    }

    private async Task ValidateProjectManagementTypeAsync(string location)
    {
        if (_projects.Count > 0 && ProjectManagementType == ProjectManagementType.SingleDocument)
        {
            throw Log.ErrorAndCreateException(
                message => new SdiProjectManagementException(message, location),
                "Cannot load project '{0}', currently in SDI mode");
        }
    }

    private async Task<IValidationContext> ValidateAndPrepareProjectLoadingAsync(string location)
    {
        IValidationContext validationContext;

        if (!await _projectValidator.CanStartLoadingProjectAsync(location))
        {
            validationContext = new ValidationContext();
            validationContext.Add(BusinessRuleValidationResult.CreateError(
                "Project validator informed that project could not be loaded"));

            throw Log.ErrorAndCreateException(
                message => new ProjectException(location, message),
                $"Cannot load project from '{location}'");
        }

        validationContext = await _projectValidator.ValidateProjectBeforeLoadingAsync(location);
        if (validationContext.HasErrors)
        {
            throw Log.ErrorAndCreateException<InvalidOperationException>(
                $"Project could not be loaded from '{location}', validator returned errors");
        }

        return validationContext;
    }

    private async Task<IProject> LoadAndValidateProjectAsync(string location, IValidationContext previousValidationContext)
    {
        var project = await QuietlyLoadProjectAsync(location, true);
        if (project is null)
        {
            throw Log.ErrorAndCreateException<InvalidOperationException>(
                $"Project could not be loaded from '{location}'");
        }

        var validationContext = await _projectValidator.ValidateProjectAsync(project);
        if (validationContext.HasErrors)
        {
            throw Log.ErrorAndCreateException<InvalidOperationException>(
                $"Project data was loaded from '{location}', but the validator returned errors");
        }

        return project;
    }

    private async Task<bool> SyncedSaveAsync(IProject project, string location)
    {
        ArgumentNullException.ThrowIfNull(project);

        if (string.IsNullOrWhiteSpace(location))
        {
            location = project.Location;
        }

        if (!_savingProjects.TryAdd(location, new object()))
        {
            Log.Warning($"Save operation already in progress for '{location}'");
            return false;
        }

        try
        {
            Log.Debug("Saving project '{0}' to '{1}'", project, location);

            _projectStateSetter.SetProjectSaving(location, true);

            var cancelEventArgs = new ProjectCancelEventArgs(project);
            await RaiseEventAsync(new ProjectSaveEvent(ProjectEventTypeStage.Before, cancelEventArgs))
                .ConfigureAwait(false);

            if (cancelEventArgs.Cancel)
            {
                _projectStateSetter.SetProjectSaving(location, false);
                Log.Debug("Canceled saving of project to '{0}'", location);
                await RaiseEventAsync(new ProjectSaveEvent(ProjectEventTypeStage.Cancelled,
                    new ProjectEventArgs(project))).ConfigureAwait(false);
                return false;
            }

            var success = false;
            Exception? error = null;

            try
            {
                success = await WriteProjectAsync(project, location);
            }
            catch (Exception ex)
            {
                error = ex;
            }

            if (error is not null)
            {
                _projectStateSetter.SetProjectSaving(location, false);
                Log.Error(error, "Failed to save project '{0}' to '{1}'", project, location);
                await RaiseEventAsync(new ProjectSaveEvent(ProjectEventTypeStage.Failed,
                    new ProjectErrorEventArgs(project, error))).ConfigureAwait(false);
                return false;
            }

            if (!success)
            {
                _projectStateSetter.SetProjectSaving(location, false);
                Log.Warning("Not saved project '{0}' to '{1}'", project, location);
                await RaiseEventAsync(new ProjectSaveEvent(ProjectEventTypeStage.Failed,
                    new ProjectErrorEventArgs(project))).ConfigureAwait(false);
                return false;
            }

            _projectStateSetter.SetProjectSaving(location, false);
            await RaiseEventAsync(new ProjectSaveEvent(ProjectEventTypeStage.After,
                new ProjectEventArgs(project))).ConfigureAwait(false);

            Log.Info("Saved project '{0}' to '{1}'", project.ToString(), location);
            return true;
        }
        finally
        {
            // Ensure saving token is removed
            _savingProjects.TryRemove(location, out _);
        }
    }

    private async Task<bool> SyncedCloseAsync(IProject project)
    {
        ArgumentNullException.ThrowIfNull(project);

        Log.Debug("Closing project '{0}'", project);

        _projectStateSetter.SetProjectClosing(project.Location, true);

        var cancelEventArgs = new ProjectCancelEventArgs(project);
        await RaiseEventAsync(new ProjectCloseEvent(ProjectEventTypeStage.Before, cancelEventArgs)).ConfigureAwait(false);

        if (cancelEventArgs.Cancel)
        {
            _projectStateSetter.SetProjectClosing(project.Location, false);

            Log.Debug("Canceled closing project '{0}'", project);
            await RaiseEventAsync(new ProjectCloseEvent(ProjectEventTypeStage.Cancelled, new ProjectEventArgs(project))).ConfigureAwait(false);

            return false;
        }

        if (Equals(ActiveProject, project))
        {
            await SetActiveProjectAsync(null).ConfigureAwait(false);
        }

        UnregisterProject(project);

        _projectStateSetter.SetProjectClosing(project.Location, false);
        await RaiseEventAsync(new ProjectCloseEvent(ProjectEventTypeStage.After, new ProjectEventArgs(project))).ConfigureAwait(false);

        Log.Info("Closed project '{0}'", project);

        return true;
    }

    private void RegisterProject(IProject project)
    {
        RegisterProjectAsync(project).GetAwaiter().GetResult();
    }

    private async Task<IProjectRefresher?> InitializeProjectRefresherAsync(string projectLocation)
    {
        using (await _refresherManagementLock.LockAsync())
        {
            // Check if refresher already exists
            if (_projectRefreshers.TryGetValue(projectLocation, out var existingRefresher))
            {
                return existingRefresher;
            }

            IProjectRefresher? refresher = null;
            try
            {
                refresher = _projectRefresherSelector.GetProjectRefresher(projectLocation);
                if (refresher is null)
                {
                    return null;
                }

                // Initialize before adding to dictionary to handle initialization failures
                Log.Debug("Subscribing to project refresher '{0}'", refresher.GetType().GetSafeFullName());
                refresher.Updated += OnProjectRefresherUpdated;

                try
                {
                    refresher.Subscribe();
                }
                catch (Exception ex)
                {
                    // Clean up event handler if subscription fails
                    refresher.Updated -= OnProjectRefresherUpdated;
                    throw Log.ErrorAndCreateException<InvalidOperationException>($"Failed to subscribe refresher for project '{projectLocation}'", ex);
                }

                // Only add to dictionary if initialization succeeds
                if (_projectRefreshers.TryAdd(projectLocation, refresher))
                {
                    return refresher;
                }
                else
                {
                    // Another thread added a refresher, clean up this one
                    await CleanupRefresherAsync(refresher, projectLocation).ConfigureAwait(false);
                    return _projectRefreshers[projectLocation];
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to initialize project refresher for '{projectLocation}'");
                if (refresher is not null)
                {
                    await CleanupRefresherAsync(refresher, projectLocation).ConfigureAwait(false);
                }
                throw;
            }
        }
    }

    private async Task ReleaseProjectRefresherAsync(IProject project)
    {
        var location = project.Location;

        using (await _refresherManagementLock.LockAsync())
        {
            if (!_projectRefreshers.TryRemove(location, out var projectRefresher))
            {
                return;
            }

            await CleanupRefresherAsync(projectRefresher, location).ConfigureAwait(false);
        }
    }

    private async Task RegisterProjectAsync(IProject project)
    {
        var projectLocation = project.Location;

        _projects.AddOrUpdate(projectLocation,
            addValue: project,
            updateValueFactory: (_, existingProject) =>
            {
                Log.Warning($"Project at location '{projectLocation}' already exists, updating registration");
                return project;
            });

        try
        {
            var refresher = await InitializeProjectRefresherAsync(projectLocation).ConfigureAwait(false);

            var resources = new ProjectResources(this, projectLocation, refresher, Log);
            _projectResources.AddOrUpdate(projectLocation, resources,
                (_, existing) =>
                {
                    existing.Dispose();
                    return resources;
                });
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Failed to initialize refresher for project '{projectLocation}'");
            // Continue without refresher - project can still be used
        }
    }

    private async Task UnregisterProjectAsync(IProject project)
    {
        var location = project.Location;

        if (_projects.TryRemove(location, out _))
        {
            await ReleaseProjectRefresherAsync(project).ConfigureAwait(false);

            if (_projectResources.TryRemove(location, out var resources))
            {
                try
                {
                    resources.Dispose();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Failed to cleanup resources for project '{location}'");
                }
            }
        }
    }

    private async Task CleanupRefresherAsync(IProjectRefresher refresher, string projectLocation)
    {
        try
        {
            // First remove event handler to prevent new refreshes during cleanup
            refresher.Updated -= OnProjectRefresherUpdated;

            // Then unsubscribe
            refresher.Unsubscribe();
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Failed to cleanup refresher for project '{projectLocation}'");
        }
    }

    protected virtual async Task<IProject?> QuietlyLoadProjectAsync(string location, bool skipCanLoadValidation)
    {
        if (skipCanLoadValidation)
        {
            Log.Debug("Validating to see if we can load the project from '{0}'", location);

            if (!await _projectValidator.CanStartLoadingProjectAsync(location))
            {
                throw Log.ErrorAndCreateException(message => new ProjectException(location, message),
                    $"Cannot load project from '{location}'");
            }
        }

        var project = await ReadProjectAsync(location);
        if (project is null)
        {
            throw Log.ErrorAndCreateException<InvalidOperationException>(
                $"Project could not be loaded from '{location}'");
        }

        // Use a proper cleanup token
        if (!_loadingProjects.TryAdd(location, new object()))
        {
            throw Log.ErrorAndCreateException<InvalidOperationException>(
                $"Project is already being loaded from '{location}'");
        }

        try
        {
            return project;
        }
        finally
        {
            _loadingProjects.TryRemove(location, out _);
        }
    }

    private void UnregisterProject(IProject project)
    {
        UnregisterProjectAsync(project).GetAwaiter().GetResult();
    }

    private async void OnProjectRefresherUpdated(object? sender, ProjectLocationEventArgs e)
    {
        var projectLocation = e.Location;
        if (string.IsNullOrEmpty(projectLocation))
        {
            return;
        }

        try
        {
            // Verify refresher is still valid before proceeding
            using (await _refresherManagementLock.LockAsync())
            {
                if (!_projectRefreshers.ContainsKey(projectLocation))
                {
                    // Refresher was removed, ignore the update
                    return;
                }
            }

            // Create new task or get existing one
            var refreshTask = _activeRefreshTasks.GetOrAdd(
                projectLocation,
                async loc =>
                {
                    try
                    {
                        await HandleProjectRefreshAsync(loc).ConfigureAwait(false);
                    }
                    finally
                    {
                        _activeRefreshTasks.TryRemove(loc, out _);
                    }
                });

            await refreshTask.ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error processing refresh for project '{0}'", projectLocation);
        }
    }

    private async Task HandleProjectRefreshAsync(string projectLocation)
    {
        try
        {
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));

            // Use the established lock hierarchy
            using (await AcquireLocksAsync(_commonLock, _refreshLock))
            {
                // Recheck state after acquiring locks
                if (!_projectRefreshers.ContainsKey(projectLocation))
                {
                    Log.Debug($"Refresher for project '{projectLocation}' was removed while waiting for locks");
                    return;
                }

                if (_loadingProjects.ContainsKey(projectLocation) ||
                    _savingProjects.ContainsKey(projectLocation))
                {
                    Log.Debug("Project '{0}' is busy with load/save operation, skipping refresh", projectLocation);
                    return;
                }

                if (!_projects.TryGetValue(projectLocation, out var project))
                {
                    Log.Warning("Project refresh required, but project '{0}' not found in list of open projects",
                        projectLocation);
                    return;
                }

                await RaiseEventAsync(
                    new ProjectRefreshEvent(
                        ProjectEventTypeStage.Required,
                        new ProjectEventArgs(project)
                    )
                ).ConfigureAwait(false);
            }
        }
        catch (TimeoutException)
        {
            Log.Warning($"Refresh operation timed out for project '{projectLocation}'");
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Failed to process refresh for project '{projectLocation}'");
            throw;
        }
    }

    private async Task<IDisposable> AcquireLocksAsync(params AsyncLock[] locks)
    {
        var timeout = TimeSpan.FromSeconds(30);
        var disposables = new List<IDisposable>();

        try
        {
            using var cts = new CancellationTokenSource(timeout);

            foreach (var lockObj in locks)
            {
                var lockAcquisitionTask = Task.Run(async () =>
                {
                    try
                    {
                        // Pass cancellation token to ensure task can be cancelled
                        if (cts.Token.IsCancellationRequested)
                        {
                            throw new OperationCanceledException();
                        }

                        var lockHandle = await lockObj.LockAsync();

                        // Check again after acquisition in case we were cancelled
                        if (cts.Token.IsCancellationRequested)
                        {
                            lockHandle.Dispose();
                            throw new OperationCanceledException();
                        }

                        return lockHandle;
                    }
                    catch (Exception)
                    {
                        // Ensure we don't hold the lock if cancelled
                        throw;
                    }
                });

                var timeoutTask = Task.Delay(timeout, cts.Token);

                Task completedTask;
                try
                {
                    completedTask = await Task.WhenAny(lockAcquisitionTask, timeoutTask)
                        .ConfigureAwait(false);
                }
                catch (Exception)
                {
                    // Cancel pending lock acquisition
#pragma warning disable CL0001
                    cts.Cancel();
#pragma warning restore CL0001
                    throw;
                }

                if (completedTask == timeoutTask)
                {
                    // Cancel any pending lock acquisition
#pragma warning disable CL0001
                    cts.Cancel();
#pragma warning restore CL0001

                    // Wait for the lock task to complete to ensure proper cleanup
                    try
                    {
                        // Short timeout for cleanup
                        using var cleanupCts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                        await lockAcquisitionTask.WaitAsync(cleanupCts.Token).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Log.Warning(ex, "Failed to wait for lock cleanup after timeout");
                    }

                    throw Log.ErrorAndCreateException<TimeoutException>($"Failed to acquire lock within {timeout.TotalSeconds} seconds");
                }

                try
                {
                    var lockHandle = await lockAcquisitionTask.ConfigureAwait(false);
                    disposables.Add(lockHandle);
                }
                catch (OperationCanceledException)
                {
                    throw Log.ErrorAndCreateException<TimeoutException>($"Lock acquisition was cancelled after {timeout.TotalSeconds} seconds");
                }
            }

            return new CompositeDisposable(disposables);
        }
        catch
        {
            // Clean up any successfully acquired locks
            foreach (var disposable in disposables)
            {
                try
                {
                    disposable.Dispose();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error disposing lock during cleanup");
                }
            }
            throw;
        }
    }

    private sealed class ProjectStateScope : IDisposable
    {
        private readonly IProjectStateSetter _stateSetter;
        private readonly string? _location;
        private readonly ProjectStateType _stateType;
        private readonly bool? _isActiveProject;
        private bool _isCommitted;

        public ProjectStateScope(IProjectStateSetter stateSetter, string? location, ProjectStateType stateType, bool? isActiveProject = null)
        {
            _stateSetter = stateSetter;
            _location = location;
            _stateType = stateType;
            _isActiveProject = isActiveProject;
            SetState(true);
        }

        public void Commit() => _isCommitted = true;

        public void Dispose()
        {
            if (!_isCommitted && !string.IsNullOrEmpty(_location))
            {
                SetState(false);
            }
        }

        private void SetState(bool value)
        {
            if (string.IsNullOrEmpty(_location))
            {
                return;
            }

            switch (_stateType)
            {
                case ProjectStateType.Loading:
                    _stateSetter.SetProjectLoading(_location, value);
                    break;
                case ProjectStateType.Saving:
                    _stateSetter.SetProjectSaving(_location, value);
                    break;
                case ProjectStateType.Refreshing:
                    _stateSetter.SetProjectRefreshing(_location, value, _isActiveProject ?? false);
                    break;
                case ProjectStateType.Closing:
                    _stateSetter.SetProjectClosing(_location, value);
                    break;
                case ProjectStateType.Activating:
                    _stateSetter.SetProjectActivating(_location, value);
                    break;
                case ProjectStateType.Deactivating:
                    _stateSetter.SetProjectDeactivating(_location, value);
                    break;
            }
        }
    }

    private enum ProjectStateType
    {
        Loading,
        Saving,
        Refreshing,
        Closing,
        Activating,
        Deactivating
    }

    private class LockHierarchy
    {
        // Lock acquisition order (from highest to lowest priority):
        // 1. _commonLock (for operation synchronization)
        // 2. _projectOperationLockers (individual project locks)
        // 3. _activeProjectLock (for active project changes)
        // 4. _refreshLock (for refresh operations)
    }

    private sealed class CompositeDisposable : IDisposable
    {
        private readonly IList<IDisposable> _disposables;
        private bool _isDisposed;

        public CompositeDisposable(IEnumerable<IDisposable> disposables)
        {
            _disposables = disposables.ToList();
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;

            // Dispose in reverse order
            for (var i = _disposables.Count - 1; i >= 0; i--)
            {
                try
                {
                    _disposables[i].Dispose();
                }
                catch (Exception ex)
                {
                    // Log error but continue disposing other resources
                    Log.Error(ex, "Error during composite disposal");
                }
            }

            _disposables.Clear();
        }
    }

    private sealed class ProjectResources : IDisposable
    {
        private readonly ILog _log;
        private readonly ProjectManager _projectManager;
        private readonly string _projectLocation;
        private readonly IProjectRefresher? _refresher;
        private readonly List<IDisposable> _resources;
        private bool _isDisposed;

        public ProjectResources(ProjectManager projectManager, string projectLocation, IProjectRefresher? refresher, ILog log)
        {
            _projectManager = projectManager;
            _projectLocation = projectLocation;
            _refresher = refresher;
            _log = log;
            _resources = new List<IDisposable>();
        }

        public void AddResource(IDisposable resource)
        {
            if (_isDisposed)
            {
#pragma warning disable IDISP007
                resource.Dispose();
#pragma warning restore IDISP007
                throw _log.ErrorAndCreateException<ObjectDisposedException>(nameof(ProjectResources));
            }
            _resources.Add(resource);
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;

            if (_refresher is not null)
            {
                try
                {
                    _refresher.Unsubscribe();
                    _refresher.Updated -= _projectManager.OnProjectRefresherUpdated;
                }
                catch (Exception ex)
                {
                    _log.Error(ex, $"Failed to cleanup refresher for project '{_projectLocation}'");
                }
            }

            // Dispose all tracked resources in reverse order
            for (var i = _resources.Count - 1; i >= 0; i--)
            {
                try
                {
                    _resources[i].Dispose();
                }
                catch (Exception ex)
                {
                    _log.Error(ex, $"Failed to dispose resource for project '{_projectLocation}'");
                }
            }

            _resources.Clear();
        }
    }
}
