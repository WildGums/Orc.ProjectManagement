﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectWatcherBase.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System;
    using System.Threading.Tasks;
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
        protected virtual async Task OnLoading(ProjectCancelEventArgs e)
        {
        }

        protected virtual async Task OnLoadingFailed(string location, Exception exception, IValidationContext validationContext)
        {
        }

        protected virtual async Task OnLoadingCanceled(string location)
        {
        }

        protected virtual async Task OnLoaded(IProject project)
        {
        }

        protected virtual async Task OnSaving(ProjectCancelEventArgs e)
        {
        }

        protected virtual async Task OnSavingFailed(IProject project, Exception exception)
        {
        }

        protected virtual async Task OnSaved(IProject project)
        {
        }

        protected virtual async Task OnClosing(ProjectCancelEventArgs e)
        {
        }

        protected virtual async Task OnClosed(IProject project)
        {
        }

        protected virtual void OnUpdated(IProject oldProject, IProject newProject, bool isRefresh)
        {
        }

        protected virtual void OnProjectRefreshRequired()
        {
        }

        private async Task OnProjectLoading(object sender, ProjectCancelEventArgs e)
        {
            await OnLoading(e);
        }

        private async Task OnProjectLoadingFailed(object sender, ProjectErrorEventArgs e)
        {
            await OnLoadingFailed(e.Location, e.Exception, e.ValidationContext);
        }

        protected async Task OnProjectLoadingCanceled(object sender, ProjectEventArgs e)
        {
            await OnLoadingCanceled(e.Location);
        }

        private async Task OnProjectLoaded(object sender, ProjectEventArgs e)
        {
            await OnLoaded(e.Project);
        }

        private async Task OnProjectSaving(object sender, ProjectCancelEventArgs e)
        {
            await OnSaving(e);
        }

        private async Task OnProjectSavingFailed(object sender, ProjectErrorEventArgs e)
        {
            await OnSavingFailed(e.Project, e.Exception);
        }

        private async Task OnProjectSaved(object sender, ProjectEventArgs e)
        {
            await OnSaved(e.Project);
        }

        private async Task OnProjectClosing(object sender, ProjectCancelEventArgs e)
        {
            await OnClosing(e);
        }

        private async Task OnProjectClosed(object sender, ProjectEventArgs e)
        {
            await OnClosed(e.Project);
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