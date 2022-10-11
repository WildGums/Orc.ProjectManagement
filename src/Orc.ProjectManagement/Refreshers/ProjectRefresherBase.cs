namespace Orc.ProjectManagement
{
    using System;
    using System.Threading.Tasks;
    using Catel;
    using Catel.IoC;
    using Catel.Logging;

    public abstract class ProjectRefresherBase : IProjectRefresher
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        // Note: dirty solution, but IProjectRefresherSelector is injected into ICommandManager, 
        // so we cannot accept ICommandManager in there (circular reference)
        private readonly Lazy<IProjectManager> _projectManager = new Lazy<IProjectManager>(() => ServiceLocator.Default.ResolveRequiredType<IProjectManager>());
        private bool _isSuspended;

        protected ProjectRefresherBase(string projectLocation)
            : this(projectLocation, projectLocation)
        {
        }

        protected ProjectRefresherBase(string projectLocation, string locationToWatch)
        {
            Argument.IsNotNullOrWhitespace(() => projectLocation);
            Argument.IsNotNullOrWhitespace(() => locationToWatch);

            ProjectLocation = projectLocation;
            Location = locationToWatch;
        }

        protected IProjectManager ProjectManager => _projectManager.Value;

        public string ProjectLocation { get; }

        public string Location { get; }

        public bool IsSubscribed { get; private set; }
        public bool IsEnabled { get; set; }

        public bool IsSuspended => !IsEnabled || _isSuspended;

        public event EventHandler<ProjectEventArgs>? Updated;

        public void Subscribe()
        {
            var location = Location;

            Log.Debug("Subscribing to '{0}' for automatic refresh functionality", location);

            if (IsSubscribed)
            {
                Log.Warning("Already subscribed to '{0}', will not subscribe again", location);
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

            Log.Debug("Unsubscribing from '{0}' for automatic refresh functionality", location);

            if (!IsSubscribed)
            {
                Log.Warning("Already unsubscribed from '{0}', will not unsubscribe again", location);
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
}
