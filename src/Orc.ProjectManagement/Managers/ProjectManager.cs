﻿namespace Orc.ProjectManagement;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catel;
using Catel.Collections;
using Catel.Data;
using Catel.IoC;
using Catel.Logging;
using Catel.Reflection;
using Catel.Threading;
using MethodTimer;

public class ProjectManager : IProjectManager, INeedCustomInitialization
{
    private const int DefaultTimeout = 3000;

    private static readonly ILog Log = LogManager.GetCurrentClassLogger();

    private readonly IProjectInitializer _projectInitializer;
    private readonly IProjectManagementInitializationService _projectManagementInitializationService;
    private readonly IProjectStateSetter _projectStateSetter;
    private readonly IProjectStateService _projectStateService;
    private readonly IDictionary<string, IProjectRefresher> _projectRefreshers;
    private readonly IProjectRefresherSelector _projectRefresherSelector;
    private readonly ListDictionary<string, IProject> _projects;
    private readonly IProjectSerializerSelector _projectSerializerSelector;
    private readonly IProjectValidator _projectValidator;
    private readonly IProjectUpgrader _projectUpgrader;

    private readonly Dictionary<string, AsyncLock> _projectOperationLockers = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, int> _projectOperationRefCounts = new(StringComparer.OrdinalIgnoreCase);
    private readonly AsyncLock _commonAsyncLock = new();

    private readonly HashSet<string> _loadingProjects = new();
    private readonly HashSet<string> _savingProjects = new();

    private readonly AsyncLock _synchronizedCommonAsyncLock = new();

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

        _projects = new ListDictionary<string, IProject>();
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

    private async Task<bool> SafeInvokeWithTimeoutAsync<TEventArgs>(AsyncEventHandler<TEventArgs>? handler, string eventName, object sender, TEventArgs e, int timeout)
        where TEventArgs : EventArgs
    {
        if (handler is null)
        {
            return false;
        }

        try
        {
            Log.DebugIfAttached($"Handling project management event '{eventName}'");

            var task = SafeInvokeAsync(eventName, handler, sender, e);
            var completedTask = await Task.WhenAny(task, Task.Delay(timeout));

            if (completedTask != task)
            {
                Log.Warning($"Handling project management event '{eventName}' has timed out");
            }
            else
            {
                Log.DebugIfAttached($"Handled project management event '{eventName}'");
            }

            return await task.ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to handle project management event '{eventName}'");
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

        var eventListeners = handler.GetInvocationList().Cast<AsyncEventHandler<TEventArgs>>().ToArray();

        foreach (var eventListener in eventListeners)
        {
            try
            {
                Log.DebugIfAttached($"Executing event handler: target '{eventListener.Target}', method '{eventListener.Method.Name}'");

                await InvokeEventListenerAsync(eventName, sender, e, eventListener);

                Log.DebugIfAttached($"Event handler successfully executed: target '{eventListener.Target}', method '{eventListener.Method.Name}'");
            }
            catch (Exception ex)
            {

                Log.Error(ex, $"Failed to invoke event handler handler: target '{eventListener.Target}', method '{eventListener.Method.Name}'");
                throw;
            }
        }

        return true;
    }

    protected virtual async Task InvokeEventListenerAsync<TEventArgs>(string eventName, object sender, TEventArgs args, AsyncEventHandler<TEventArgs> eventListener)
        where TEventArgs : EventArgs
    {
        await eventListener(sender, args).ConfigureAwait(false);
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
        using (await _commonAsyncLock.LockAsync())
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

            var eventArgs = new ProjectUpdatingCancelEventArgs(activeProject, project);

            _projectStateSetter.SetProjectDeactivating(activeProject?.Location, true);
            _projectStateSetter.SetProjectActivating(project?.Location, true);

            await RaiseEventAsync(new ProjectActivationEvent(ProjectEventTypeStage.Before, eventArgs)).ConfigureAwait(false);

            if (eventArgs.Cancel)
            {
                Log.Info(project is not null
                    ? $"Activating project '{project.Location}' was canceled"
                    : "Deactivating currently active project");

                _projectStateSetter.SetProjectActivating(project?.Location, false);

                await RaiseEventAsync(new ProjectActivationEvent(ProjectEventTypeStage.Cancelled, new ProjectActivationEventArgs(project))).ConfigureAwait(false);

                return false;
            }

            Exception? exception = null;

            try
            {
                ActiveProject = project;
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (exception is not null)
            {
                Log.Error(exception, project is not null
                    ? $"Failed to activate project '{project.Location}'"
                    : "Failed to deactivate currently active project");

                _projectStateSetter.SetProjectActivating(project?.Location ?? string.Empty, false);
                await RaiseEventAsync(new ProjectActivationEvent(ProjectEventTypeStage.Failed, new ProjectErrorEventArgs(project, exception))).ConfigureAwait(false);

                return false;
            }

            _projectStateSetter.SetProjectDeactivating(activeProject?.Location, false);
            _projectStateSetter.SetProjectActivating(project?.Location, false);

            await RaiseEventAsync(new ProjectActivationEvent(ProjectEventTypeStage.After, new ProjectUpdatedEventArgs(activeProject, project))).ConfigureAwait(false);

            Log.Debug(project is not null
                ? $"Activating project '{project.Location}' was canceled"
                : "Deactivating currently active project");

            return true;
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

        var countedAsyncLock = await InitializeSynchronizationContextAsync(projectLocation);

        var asyncLock = countedAsyncLock.AsyncLock;
        var refCount = countedAsyncLock.RefCount;

        try
        {
            using (await asyncLock.LockAsync())
            {
                Log.Debug($"Start synchronized operation for '{projectLocation}' refCount = {refCount}]");
                return await operation();
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Failed to execute synchronized operation for '{projectLocation}' refCount = {refCount}]");
            throw;
        }
        finally
        {
            await ReleaseSynchronizationContextAsync(projectLocation);
        }
    }

    private async Task ReleaseSynchronizationContextAsync(string projectLocation)
    {
        Log.Debug($"Releasing operation synchronization context for '{projectLocation}'");

        using (await _synchronizedCommonAsyncLock.LockAsync())
        {
            if (!_projectOperationRefCounts.TryGetValue(projectLocation, out var refCount))
            {
                refCount = 0;
            }

            refCount--;

            if (refCount >= 1)
            {
                _projectOperationRefCounts[projectLocation] = refCount;
            }
            else
            {
                _projectOperationRefCounts.Remove(projectLocation);
                _projectOperationLockers.Remove(projectLocation);
            }
        }

        Log.Debug($"Released operation synchronization context for '{projectLocation}'");
    }

    private async Task<OperationSynchronizationContext> InitializeSynchronizationContextAsync(string projectLocation)
    {
        AsyncLock? asyncLock;
        int refCount;

        Log.Debug($"Initializing operation synchronization context for '{projectLocation}'");

        using (await _synchronizedCommonAsyncLock.LockAsync())
        {
            if (!_projectOperationLockers.TryGetValue(projectLocation, out asyncLock))
            {
                asyncLock = new AsyncLock();
                _projectOperationLockers.Add(projectLocation, asyncLock);
            }

            if (!_projectOperationRefCounts.TryGetValue(projectLocation, out refCount))
            {
                refCount = 0;
            }

            refCount++;

            _projectOperationRefCounts[projectLocation] = refCount;
        }

        Log.Debug($"Initialized operation synchronization context for '{projectLocation}' refCount = [{refCount}]");

        return new OperationSynchronizationContext(asyncLock, refCount);
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
        using (new DisposableToken(location, _ => _loadingProjects.Add(projectLocation), _ => _loadingProjects.Remove(projectLocation)))
        {
            Log.Debug($"Going to load project from '{location}', checking if an upgrade is required");

            if (await _projectUpgrader.RequiresUpgradeAsync(location))
            {
                Log.Debug($"Upgrade is required for '{location}', upgrading...");

                location = await _projectUpgrader.UpgradeAsync(location);

                Log.Debug($"Upgraded project, final location is '{location}'");
            }

            Log.Debug($"Loading project from '{location}'");

            _projectStateSetter.SetProjectLoading(location, true);

            var cancelEventArgs = new ProjectCancelEventArgs(location);

            await RaiseEventAsync(new ProjectLoadEvent(ProjectEventTypeStage.Before, cancelEventArgs)).ConfigureAwait(false);

            if (cancelEventArgs.Cancel)
            {
                Log.Debug("Canceled loading of project from '{0}'", location);

                _projectStateSetter.SetProjectLoading(location, false);

                await RaiseEventAsync(new ProjectLoadEvent(ProjectEventTypeStage.Cancelled, new ProjectLocationEventArgs(location))).ConfigureAwait(false);

                return null;
            }

            Exception? error = null;
            IValidationContext? validationContext = null;

            try
            {
                if (_projects.Count > 0 && ProjectManagementType == ProjectManagementType.SingleDocument)
                {
                    throw Log.ErrorAndCreateException(message => new SdiProjectManagementException(message, location), "Cannot load project '{0}', currently in SDI mode");
                }

                if (!await _projectValidator.CanStartLoadingProjectAsync(location))
                {
                    validationContext = new ValidationContext();
                    validationContext.Add(BusinessRuleValidationResult.CreateError("Project validator informed that project could not be loaded"));

                    throw Log.ErrorAndCreateException(message => new ProjectException(location, message), $"Cannot load project from '{location}'");
                }

                validationContext = await _projectValidator.ValidateProjectBeforeLoadingAsync(location);
                if (validationContext.HasErrors)
                {
                    throw Log.ErrorAndCreateException<InvalidOperationException>($"Project could not be loaded from '{location}', validator returned errors");
                }

                project = await QuietlyLoadProjectAsync(location, true).ConfigureAwait(false);

                validationContext = await _projectValidator.ValidateProjectAsync(project);
                if (validationContext.HasErrors)
                {
                    throw Log.ErrorAndCreateException<InvalidOperationException>($"Project data was loaded from '{location}', but the validator returned errors");
                }

                RegisterProject(project);
            }
            catch (Exception ex)
            {
                error = ex;
                Log.Warning(ex, "Failed to load project from '{0}'", location);
            }

            if (project is null || error is not null)
            {
                _projectStateSetter.SetProjectLoading(location, false);

                await RaiseEventAsync(new ProjectLoadEvent(ProjectEventTypeStage.Failed, new ProjectErrorEventArgs(location, error, validationContext))).ConfigureAwait(false);

                return null;
            }

            _projectStateSetter.SetProjectLoading(location, false);

            await RaiseEventAsync(new ProjectLoadEvent(ProjectEventTypeStage.After, new ProjectEventArgs(project))).ConfigureAwait(false);

            Log.Info("Loaded project from '{0}'", location);
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

        using (new DisposableToken(location, _ => _savingProjects.Add(location), _ => _savingProjects.Remove(location)))
        {
            Log.Debug("Saving project '{0}' to '{1}'", project, location);

            // We could support SaveAs where we store the new location, but we need to make sure that we also remove
            // the old one (and revert on failure & cancel). For now this is sufficient (we will just get a new instance)
            _projectStateSetter.SetProjectSaving(location, true);

            var cancelEventArgs = new ProjectCancelEventArgs(project);
            await RaiseEventAsync(new ProjectSaveEvent(ProjectEventTypeStage.Before, cancelEventArgs)).ConfigureAwait(false);

            if (cancelEventArgs.Cancel)
            {
                _projectStateSetter.SetProjectSaving(location, false);

                Log.Debug("Canceled saving of project to '{0}'", location);
                await RaiseEventAsync(new ProjectSaveEvent(ProjectEventTypeStage.Cancelled, new ProjectEventArgs(project))).ConfigureAwait(false);

                return false;
            }

            Exception? error = null;
            var success = true;
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

                await RaiseEventAsync(new ProjectSaveEvent(ProjectEventTypeStage.Failed, new ProjectErrorEventArgs(project, error))).ConfigureAwait(false);

                return false;
            }

            if (!success)
            {
                _projectStateSetter.SetProjectSaving(location, false);

                Log.Warning("Not saved project '{0}' to '{1}'", project, location);

                await RaiseEventAsync(new ProjectSaveEvent(ProjectEventTypeStage.Failed, new ProjectErrorEventArgs(project))).ConfigureAwait(false);

                return false;
            }

            _projectStateSetter.SetProjectSaving(location, false);

            await RaiseEventAsync(new ProjectSaveEvent(ProjectEventTypeStage.After, new ProjectEventArgs(project))).ConfigureAwait(false);

            var projectString = project.ToString();
            Log.Info("Saved project '{0}' to '{1}'", projectString, location);
        }

        return true;
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
        var projectLocation = project.Location;
        _projects[projectLocation] = project;

        InitializeProjectRefresher(projectLocation);
    }

    private async Task<IProject> QuietlyLoadProjectAsync(string location, bool skipCanLoadValidation)
    {
        if (skipCanLoadValidation)
        {
            Log.Debug("Validating to see if we can load the project from '{0}'", location);

            if (!await _projectValidator.CanStartLoadingProjectAsync(location))
            {
                throw Log.ErrorAndCreateException(message => new ProjectException(location, message), $"Cannot load project from '{location}'");
            }
        }

        var project = await ReadProjectAsync(location);

        if (project is null)
        {
            throw Log.ErrorAndCreateException<InvalidOperationException>($"Project could not be loaded from '{location}'");
        }

        return project;
    }

    private void UnregisterProject(IProject project)
    {
        var location = project.Location;
        if (_projects.ContainsKey(location))
        {
            _projects.Remove(location);
        }

        ReleaseProjectRefresher(project);
    }

    private void InitializeProjectRefresher(string projectLocation)
    {
        if (_projectRefreshers.TryGetValue(projectLocation, out var projectRefresher))
        {
            return;
        }

        try
        {
            projectRefresher = _projectRefresherSelector.GetProjectRefresher(projectLocation);

            if (projectRefresher is null)
            {
                return;
            }

            Log.Debug("Subscribing to project refresher '{0}'", projectRefresher.GetType().GetSafeFullName());

            projectRefresher.Updated += OnProjectRefresherUpdated;
            projectRefresher.Subscribe();

            _projectRefreshers[projectLocation] = projectRefresher;
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Failed to subscribe to project refresher");
            throw;
        }
    }

    private void ReleaseProjectRefresher(IProject project)
    {
        var location = project.Location;

        if (!_projectRefreshers.TryGetValue(location, out var projectRefresher))
        {
            return;
        }

        try
        {
            Log.Debug("Unsubscribing from project refresher '{0}'", projectRefresher.GetType().GetSafeFullName());

            projectRefresher.Unsubscribe();
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Failed to unsubscribe from project refresher");
        }

        projectRefresher.Updated -= OnProjectRefresherUpdated;

        _projectRefreshers.Remove(location);
    }

    private async void OnProjectRefresherUpdated(object? sender, ProjectLocationEventArgs e)
    {
        var projectLocation = e.Location;
        if (string.IsNullOrEmpty(projectLocation))
        {
            return;
        }

        if (_loadingProjects.Contains(projectLocation) || _savingProjects.Contains(projectLocation))
        {
            return;
        }

        if (_projects.TryGetValue(projectLocation, out var project))
        {
            // Note: not sure why we still need this
            await RaiseEventAsync(new ProjectRefreshEvent(ProjectEventTypeStage.Required, new ProjectEventArgs(project))).ConfigureAwait(false);
        }
        else
        {
            Log.Warning($"Project refresh required, but can't find project '{projectLocation}' in list of open projects");
        }
    }

    private class OperationSynchronizationContext
    {
        public OperationSynchronizationContext(AsyncLock asyncLock, int refCount)
        {
            AsyncLock = asyncLock;
            RefCount = refCount;
        }

        public AsyncLock AsyncLock { get; }
        public int RefCount { get; }
    }
}
