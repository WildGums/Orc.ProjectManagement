namespace Orc.ProjectManagement;

using System;
using System.Threading.Tasks;
using Catel;
using Catel.IoC;
using Catel.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public abstract class ProjectRefresherBase : IProjectRefresher
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(ProjectRefresherBase));

    private bool _isSuspended;

    // Note: dirty solution, but IProjectRefresherSelector is injected into ICommandManager, 
    // so we cannot accept ICommandManager in there (circular reference)
    private readonly Lazy<IProjectManager> _projectManager;
    private readonly IServiceProvider _serviceProvider;

    protected ProjectRefresherBase(string projectLocation, IServiceProvider serviceProvider)
        : this(projectLocation, projectLocation, serviceProvider)
    {
    }

    protected ProjectRefresherBase(string projectLocation, string locationToWatch, IServiceProvider serviceProvider)
    {
        Argument.IsNotNullOrWhitespace(() => projectLocation);
        Argument.IsNotNullOrWhitespace(() => locationToWatch);

        ProjectLocation = projectLocation;
        Location = locationToWatch;
        _serviceProvider = serviceProvider;

        _projectManager = new Lazy<IProjectManager>(() => _serviceProvider.GetRequiredService<IProjectManager>());
    }

    protected IProjectManager ProjectManager => _projectManager.Value;

    public string ProjectLocation { get; }

    public string Location { get; }

    public bool IsSubscribed { get; private set; }
    public bool IsEnabled { get; set; }

    public bool IsSuspended => !IsEnabled || _isSuspended;

    public event EventHandler<ProjectLocationEventArgs>? Updated;

    public void Subscribe()
    {
        var location = Location;

        Logger.LogDebug("Subscribing to '{Location}' for automatic refresh functionality", location);

        if (IsSubscribed)
        {
            Logger.LogWarning("Already subscribed to '{Location}', will not subscribe again", location);
            return;
        }

        ProjectManager.ProjectSavingAsync += OnProjectManagerSavingAsync;
        ProjectManager.ProjectSavedAsync += OnProjectManagerSavedAsync;
        ProjectManager.ProjectSavingCanceledAsync += OnProjectManagerSavingCanceledAsync;
        ProjectManager.ProjectSavingFailedAsync += OnProjectManagerSavingFailedAsync;

        SubscribeToLocation(location);

        IsSubscribed = true;
    }

    public void Unsubscribe()
    {
        var location = Location;

        Logger.LogDebug("Unsubscribing from '{Location}' for automatic refresh functionality", location);

        if (!IsSubscribed)
        {
            Logger.LogWarning("Already unsubscribed from '{Location}', will not unsubscribe again", location);
            return;
        }

        ProjectManager.ProjectSavingAsync -= OnProjectManagerSavingAsync;
        ProjectManager.ProjectSavedAsync -= OnProjectManagerSavedAsync;
        ProjectManager.ProjectSavingCanceledAsync -= OnProjectManagerSavingCanceledAsync;
        ProjectManager.ProjectSavingFailedAsync -= OnProjectManagerSavingFailedAsync;

        UnsubscribeFromLocation(location);

        IsSubscribed = false;
    }

    protected abstract void SubscribeToLocation(string location);

    protected abstract void UnsubscribeFromLocation(string location);

    protected void RaiseUpdated(string fileName)
    {
        Updated?.Invoke(this, new ProjectFileSystemEventArgs(ProjectLocation, fileName));
    }

    private Task OnProjectManagerSavingAsync(object sender, ProjectCancelEventArgs e)
    {
        _isSuspended = true;

        return Task.CompletedTask;
    }

    private Task OnProjectManagerSavedAsync(object sender, ProjectEventArgs e)
    {
        _isSuspended = false;

        return Task.CompletedTask;
    }

    private Task OnProjectManagerSavingCanceledAsync(object sender, ProjectEventArgs e)
    {
        _isSuspended = false;

        return Task.CompletedTask;
    }

    private Task OnProjectManagerSavingFailedAsync(object sender, ProjectErrorEventArgs e)
    {
        _isSuspended = false;

        return Task.CompletedTask;
    }
}
