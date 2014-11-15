// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceWatcherBase.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using Catel;

    public abstract class ProjectWatcherBase
    {
        private readonly IProjectManager _projectManager;

        #region Constructors
        protected ProjectWatcherBase(IProjectManager projectManager)
        {
            Argument.IsNotNull(() => projectManager);

            _projectManager = projectManager;

            projectManager.ProjectLoading += OnProjectLoading;
            projectManager.ProjectLoaded += OnProjectLoaded;

            projectManager.ProjectSaving += OnProjectSaving;
            projectManager.ProjectSaved += OnProjectSaved;

            projectManager.ProjectClosing += OnProjectClosing;
            projectManager.ProjectClosed += OnProjectClosed;

            projectManager.ProjectUpdated += OnProjectUpdated;
        }
        #endregion

        #region Properties
        protected IProjectManager ProjectManager
        {
            get { return _projectManager; }
        }
        #endregion

        #region Methods
        protected virtual void OnLoading(string location)
        {
        }

        protected virtual void OnLoaded(IProject project)
        {
        }

        protected virtual void OnSaving(IProject project)
        {
        }

        protected virtual void OnSaved(IProject project)
        {
        }

        protected virtual void OnClosing(IProject project)
        {
        }

        protected virtual void OnClosed(IProject project)
        {
        }

        protected virtual void OnUpdated(IProject oldProject, IProject newProject, bool isRefresh)
        {
        }

        private void OnProjectLoading(object sender, ProjectEventArgs e)
        {
            OnLoading(e.Location);
        }

        private void OnProjectLoaded(object sender, ProjectEventArgs e)
        {
            OnLoaded(e.Project);
        }

        private void OnProjectSaving(object sender, ProjectEventArgs e)
        {
            OnSaving(e.Project);
        }

        private void OnProjectSaved(object sender, ProjectEventArgs e)
        {
            OnSaved(e.Project);
        }

        private void OnProjectClosing(object sender, ProjectEventArgs e)
        {
            OnClosing(e.Project);
        }

        private void OnProjectClosed(object sender, ProjectEventArgs e)
        {
            OnClosed(e.Project);
        }

        private void OnProjectUpdated(object sender, ProjectUpdatedEventArgs e)
        {
            OnUpdated(e.OldProject, e.NewProject, e.IsRefresh);
        }
        #endregion
    }
}