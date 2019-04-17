// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectRefresherBase.cs" company="WildGums">
//   Copyright (c) 2008 - 2018 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System;
    using System.Threading.Tasks;
    using Catel;
    using Catel.IoC;
    using Catel.Logging;
    using Catel.Threading;

    public abstract class ProjectRefresherBase : IProjectRefresher
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        // Note: dirty solution, but IProjectRefresherSelector is injected into ICommandManager, 
        // so we cannot accept ICommandManager in there (circular reference)
        private readonly Lazy<IProjectManager> _projectManager = new Lazy<IProjectManager>(() => ServiceLocator.Default.ResolveType<IProjectManager>());
        private bool _isSuspended;
        #endregion

        #region Constructors
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
        #endregion

        #region Properties
        protected IProjectManager ProjectManager => _projectManager.Value;

        public string ProjectLocation { get; }

        public string Location { get; }

        public bool IsSubscribed { get; private set; }
        public bool IsEnabled { get; set; }

        public bool IsSuspended => !IsEnabled || _isSuspended;
        #endregion

        #region Methods
        #region Events
        public event EventHandler<ProjectEventArgs> Updated;
        #endregion

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

            return TaskHelper.Completed;
        }

        private Task OnProjectManagerSavedAsync(object sender, ProjectEventArgs e)
        {
            _isSuspended = false;

            return TaskHelper.Completed;
        }

        private Task OnProjectManagerSavingCanceledAsync(object sender, ProjectEventArgs e)
        {
            _isSuspended = false;

            return TaskHelper.Completed;
        }

        private Task OnProjectManagerSavingFailedAsync(object sender, ProjectErrorEventArgs e)
        {
            _isSuspended = false;

            return TaskHelper.Completed;
        }
        #endregion
    }
}
