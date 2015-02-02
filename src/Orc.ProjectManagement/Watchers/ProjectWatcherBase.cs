// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceWatcherBase.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System;
    using Catel;
    using Catel.Data;

    public abstract class ProjectWatcherBase
    {
        private readonly IProjectManager _projectManager;

        #region Constructors
        protected ProjectWatcherBase(IProjectManager projectManager)
        {
            Argument.IsNotNull(() => projectManager);

            _projectManager = projectManager;

            projectManager.ProjectLoading += OnProjectLoading;
            projectManager.ProjectLoadingFailed += OnProjectLoadingFailed;
            projectManager.ProjectLoaded += OnProjectLoaded;

            projectManager.ProjectSaving += OnProjectSaving;
            projectManager.ProjectSavingFailed += OnProjectSavingFailed;
            projectManager.ProjectSaved += OnProjectSaved;

            projectManager.ProjectClosing += OnProjectClosing;
            projectManager.ProjectClosed += OnProjectClosed;

            projectManager.ProjectCanceled += OnProjectCanceled;

            projectManager.ProjectUpdated += OnProjectUpdated;

            projectManager.ProjectRefreshRequired += OnProjectRefreshRequired;
        }

        /// <summary>
        /// Called when project loading process was canceled during <see cref="IProjectManager.ProjectCanceled"/> event
        /// </summary>
        protected virtual void OnProjectCanceled(object sender, ProjectEventArgs e)
        {
            
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

        protected virtual void OnLoadingFailed(string location, Exception exception, IValidationContext validationContext)
        {
        }

        protected virtual void OnLoaded(IProject project)
        {
        }

        protected virtual void OnSaving(IProject project)
        {
        }

        protected virtual void OnSavingFailed(IProject project, Exception exception)
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

        protected virtual void OnProjectRefreshRequired()
        {
        }

        private void OnProjectLoading(object sender, ProjectEventArgs e)
        {
            OnLoading(e.Location);
        }

        private void OnProjectLoadingFailed(object sender, ProjectErrorEventArgs e)
        {
            OnLoadingFailed(e.Location, e.Exception, e.ValidationContext);
        }

        private void OnProjectLoaded(object sender, ProjectEventArgs e)
        {
            OnLoaded(e.Project);
        }

        private void OnProjectSaving(object sender, ProjectEventArgs e)
        {
            OnSaving(e.Project);
        }

        private void OnProjectSavingFailed(object sender, ProjectErrorEventArgs e)
        {
            OnSavingFailed(e.Project, e.Exception);
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

        private void OnProjectRefreshRequired(object sender, EventArgs e)
        {
            OnProjectRefreshRequired();
        }
        #endregion
    }
}