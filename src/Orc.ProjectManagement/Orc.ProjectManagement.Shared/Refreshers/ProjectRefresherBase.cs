// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectRefresherBase.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


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

            ProjectManager.ProjectSaving += OnProjectManagerSaving;
            ProjectManager.ProjectSaved += OnProjectManagerSaved;
            ProjectManager.ProjectSavingCanceled += OnProjectManagerSavingCanceled;
            ProjectManager.ProjectSavingFailed += OnProjectManagerSavingFailed;

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

            ProjectManager.ProjectSaving -= OnProjectManagerSaving;
            ProjectManager.ProjectSaved -= OnProjectManagerSaved;
            ProjectManager.ProjectSavingCanceled -= OnProjectManagerSavingCanceled;
            ProjectManager.ProjectSavingFailed -= OnProjectManagerSavingFailed;

            UnsubscribeFromLocation(location);

            IsSubscribed = false;
        }

        protected abstract void UnsubscribeFromLocation(string location);

        protected void RaiseUpdated(string path)
        {
            Updated.SafeInvoke(this, new ProjectEventArgs(path));
        }

        private async Task OnProjectManagerSaving(object sender, ProjectCancelEventArgs e)
        {
            IsSuspended = true;
        }

        private async Task OnProjectManagerSaved(object sender, ProjectEventArgs e)
        {
            IsSuspended = false;
        }

        private async Task OnProjectManagerSavingCanceled(object sender, ProjectEventArgs e)
        {
            IsSuspended = false;
        }

        private async Task OnProjectManagerSavingFailed(object sender, ProjectErrorEventArgs e)
        {
            IsSuspended = false;
        }
        #endregion
    }
}