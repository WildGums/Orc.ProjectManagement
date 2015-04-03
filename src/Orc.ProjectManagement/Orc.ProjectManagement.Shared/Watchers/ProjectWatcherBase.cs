// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectWatcherBase.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System;
    using Catel;
    using Catel.Data;

    public abstract class ProjectWatcherBase
    {
        #region Fields
        private readonly IProjectManager _projectManager;
        #endregion

        #region Constructors
        protected ProjectWatcherBase(IProjectManager projectManager)
        {
            Argument.IsNotNull(() => projectManager);

            _projectManager = projectManager;

            projectManager.ProjectLoading += OnProjectLoading;
            projectManager.ProjectLoadingFailed += OnProjectLoadingFailed;
            projectManager.ProjectLoadingCanceled += OnProjectLoadingCanceled;
            projectManager.ProjectLoaded += OnProjectLoaded;

            projectManager.ProjectSaving += OnProjectSaving;
            projectManager.ProjectSavingFailed += OnProjectSavingFailed;
            projectManager.ProjectSaved += OnProjectSaved;

            projectManager.ProjectClosing += OnProjectClosing;
            projectManager.ProjectClosed += OnProjectClosed;

            projectManager.ProjectUpdated += OnProjectUpdated;

            projectManager.ProjectRefreshRequired += OnProjectRefreshRequired;
        }
        #endregion

        #region Properties
        protected IProjectManager ProjectManager
        {
            get { return _projectManager; }
        }
        #endregion

        #region Methods
        protected virtual void OnLoading(string location, ref bool cancel)
        {
        }

        protected virtual void OnLoadingFailed(string location, Exception exception, IValidationContext validationContext)
        {
        }

        protected virtual void OnLoadingCanceled(string location)
        {
        }

        protected virtual void OnLoaded(IProject project)
        {
        }

        protected virtual void OnSaving(IProject project, ref bool cancel)
        {
        }

        protected virtual void OnSavingFailed(IProject project, Exception exception)
        {
        }

        protected virtual void OnSaved(IProject project)
        {
        }

        protected virtual void OnClosing(IProject project, ref bool cancel)
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

        private void OnProjectLoading(object sender, ProjectCancelEventArgs e)
        {
            var cancel = e.Cancel;
            OnLoading(e.Location, ref cancel);
            e.Cancel = cancel;
        }

        private void OnProjectLoadingFailed(object sender, ProjectErrorEventArgs e)
        {
            OnLoadingFailed(e.Location, e.Exception, e.ValidationContext);
        }

        protected void OnProjectLoadingCanceled(object sender, ProjectEventArgs e)
        {
            OnLoadingCanceled(e.Location);
        }

        private void OnProjectLoaded(object sender, ProjectEventArgs e)
        {
            OnLoaded(e.Project);
        }

        private void OnProjectSaving(object sender, ProjectCancelEventArgs e)
        {
            var cancel = e.Cancel;
            OnSaving(e.Project, ref cancel);
            e.Cancel = cancel;
        }

        private void OnProjectSavingFailed(object sender, ProjectErrorEventArgs e)
        {
            OnSavingFailed(e.Project, e.Exception);
        }

        private void OnProjectSaved(object sender, ProjectEventArgs e)
        {
            OnSaved(e.Project);
        }

        private void OnProjectClosing(object sender, ProjectCancelEventArgs e)
        {
            var cancel = e.Cancel;
            OnClosing(e.Project, ref cancel);
            e.Cancel = cancel;
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