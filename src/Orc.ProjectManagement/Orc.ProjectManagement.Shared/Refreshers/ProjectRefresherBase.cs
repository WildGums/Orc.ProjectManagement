// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectRefresherBase.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
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
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        // Note: dirty solution, but IProjectRefresherSelector is injected into ICommandManager, 
        // so we cannot accept ICommandManager in there (circular reference)
        private readonly Lazy<IProjectManager> _projectManager = new Lazy<IProjectManager>(() => ServiceLocator.Default.ResolveType<IProjectManager>());

        protected ProjectRefresherBase(string location)
        {
            Argument.IsNotNullOrWhitespace(() => location);

            Location = location;
        }

        #region Properties
        protected IProjectManager ProjectManager { get { return _projectManager.Value; } }

        public string Location { get; private set; }

        public bool IsSubscribed { get; private set; }

        public bool IsSuspended { get; private set; }
        #endregion

        #region Events
        public event EventHandler<ProjectEventArgs> Updated;
        #endregion

        #region Methods
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

        protected abstract void SubscribeToLocation(string location);

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

        protected abstract void UnsubscribeFromLocation(string location);

        protected void RaiseUpdated(string path)
        {
            Updated.SafeInvoke(this, new ProjectEventArgs(path));
        }

        private Task OnProjectManagerSavingAsync(object sender, ProjectCancelEventArgs e)
        {
            IsSuspended = true;

            return TaskHelper.Completed;
        }

        private Task OnProjectManagerSavedAsync(object sender, ProjectEventArgs e)
        {
            IsSuspended = false;

            return TaskHelper.Completed;
        }

        private Task OnProjectManagerSavingCanceledAsync(object sender, ProjectEventArgs e)
        {
            IsSuspended = false;

            return TaskHelper.Completed;
        }

        private Task OnProjectManagerSavingFailedAsync(object sender, ProjectErrorEventArgs e)
        {
            IsSuspended = false;

            return TaskHelper.Completed;
        }
        #endregion
    }
}