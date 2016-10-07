// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectStateService.cs" company="WildGums">
//   Copyright (c) 2008 - 2016 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Data;
    using Catel.Logging;
    using Catel.Threading;

    public class ProjectStateService : IProjectStateService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IProjectManager _projectManager;

        private readonly Dictionary<string, ProjectState> _projectStates = new Dictionary<string, ProjectState>();
        private string _lastActiveProject = string.Empty;
        private bool _isRefreshingActiveProject;

        public ProjectStateService(IProjectManager projectManager)
        {
            Argument.IsNotNull(() => projectManager);

            _projectManager = projectManager;

            InitSubscriptions();

            _lastActiveProject = _projectManager.ActiveProject?.Location;
        }

        public bool IsRefreshingActiveProject
        {
            get { return _isRefreshingActiveProject; }
            private set
            {
                if (_isRefreshingActiveProject != value)
                {
                    _isRefreshingActiveProject = value;
                    IsRefreshingActiveProjectUpdated.SafeInvoke(this);
                }
            }
        }

        public event EventHandler<EventArgs> IsRefreshingActiveProjectUpdated;

        public event EventHandler<ProjectStateEventArgs> ProjectStateUpdated;

        public ProjectState GetProjectState(IProject project)
        {
            Argument.IsNotNull(() => project);

            var location = project.Location;
            ProjectState projectState = null;

            _projectStates.TryGetValue(location, out projectState);

            return projectState;
        }

        private void InitSubscriptions()
        {
            _projectManager.ProjectLoadingAsync += async (sender, e) => await OnLoadingAsync(e).ConfigureAwait(false);
            _projectManager.ProjectLoadingFailedAsync += async (sender, e) => await OnLoadingFailedAsync(e.Location, e.Exception, e.ValidationContext).ConfigureAwait(false);
            _projectManager.ProjectLoadingCanceledAsync += async (sender, e) => await OnLoadingCanceledAsync(e.Location).ConfigureAwait(false);
            _projectManager.ProjectLoadedAsync += async (sender, e) => await OnLoadedAsync(e.Project).ConfigureAwait(false);

            _projectManager.ProjectSavingAsync += async (sender, e) => await OnSavingAsync(e).ConfigureAwait(false);
            _projectManager.ProjectSavingCanceledAsync += async (sender, e) => await OnSavingCanceledAsync(e.Project).ConfigureAwait(false);
            _projectManager.ProjectSavingFailedAsync += async (sender, e) => await OnSavingFailedAsync(e.Project, e.Exception).ConfigureAwait(false);
            _projectManager.ProjectSavedAsync += async (sender, e) => await OnSavedAsync(e.Project).ConfigureAwait(false);

            _projectManager.ProjectClosingAsync += async (sender, e) => await OnClosingAsync(e).ConfigureAwait(false);
            _projectManager.ProjectClosingCanceledAsync += async (sender, e) => await OnClosingCanceledAsync(e.Project).ConfigureAwait(false);
            _projectManager.ProjectClosedAsync += async (sender, e) => await OnClosedAsync(e.Project).ConfigureAwait(false);

            _projectManager.ProjectActivationAsync += async (sender, e) => await OnActivationAsync(e).ConfigureAwait(false);
            _projectManager.ProjectActivationFailedAsync += async (sender, e) => await OnActivationFailedAsync(e.Project, e.Exception).ConfigureAwait(false);
            _projectManager.ProjectActivationCanceledAsync += async (sender, e) => await OnActivationCanceledAsync(e.Project).ConfigureAwait(false);
            _projectManager.ProjectActivatedAsync += async (sender, e) => await OnActivatedAsync(e.OldProject, e.NewProject).ConfigureAwait(false);

            _projectManager.ProjectRefreshingAsync += async (sender, e) => await OnRefreshingAsync(e).ConfigureAwait(false);
            _projectManager.ProjectRefreshingFailedAsync += async (sender, e) => await OnRefreshingFailedAsync(e.Project, e.Exception, e.ValidationContext).ConfigureAwait(false);
            _projectManager.ProjectRefreshingCanceledAsync += async (sender, e) => await OnRefreshingCanceledAsync(e.Project).ConfigureAwait(false);
            _projectManager.ProjectRefreshedAsync += async (sender, e) => await OnRefreshedAsync(e.Project).ConfigureAwait(false);
        }

        private Task OnLoadingAsync(ProjectCancelEventArgs e)
        {
            UpdateState(e.Location, state => state.IsLoading = true);

            return TaskHelper.Completed;
        }

        private Task OnLoadingFailedAsync(string location, Exception exception, IValidationContext validationContext)
        {
            UpdateState(location, state => state.IsLoading = false);

            return TaskHelper.Completed;
        }

        private Task OnLoadingCanceledAsync(string location)
        {
            UpdateState(location, state => state.IsLoading = false);

            return TaskHelper.Completed;
        }

        private Task OnLoadedAsync(IProject project)
        {
            UpdateState(project?.Location, state => state.IsLoading = false);

            return TaskHelper.Completed;
        }

        private Task OnSavingAsync(ProjectCancelEventArgs e)
        {
            // TODO: Support SaveAs where we store the new location, but we need to make sure that we also remove 
            // the old one (and revert on failure & cancel). For now this is sufficient (we will just get a new instance)
            UpdateState(e.Location, state => state.IsSaving = true);

            return TaskHelper.Completed;
        }

        private Task OnSavingCanceledAsync(IProject project)
        {
            UpdateState(project?.Location, state => state.IsSaving = false);

            return TaskHelper.Completed;
        }

        private Task OnSavingFailedAsync(IProject project, Exception exception)
        {
            UpdateState(project?.Location, state => state.IsSaving = false);

            return TaskHelper.Completed;
        }

        private Task OnSavedAsync(IProject project)
        {
            UpdateState(project?.Location, state => state.IsSaving = false);

            return TaskHelper.Completed;
        }

        private Task OnClosingAsync(ProjectCancelEventArgs e)
        {
            UpdateState(e.Location, state => state.IsClosing = true);

            return TaskHelper.Completed;
        }

        private Task OnClosingCanceledAsync(IProject project)
        {
            UpdateState(project?.Location, state => state.IsClosing = false);

            return TaskHelper.Completed;
        }

        private Task OnClosedAsync(IProject project)
        {
            UpdateState(project?.Location, state => state.IsClosing = false);

            return TaskHelper.Completed;
        }

        private Task OnActivationAsync(ProjectUpdatingCancelEventArgs e)
        {
            UpdateState(e.OldProjectLocation, state => state.IsDeactivating = true);
            UpdateState(e.NewProjectLocation, state => state.IsActivating = true);

            return TaskHelper.Completed;
        }

        private Task OnActivationFailedAsync(IProject project, Exception exception)
        {
            IsRefreshingActiveProject = false;

            // Note: unfortunately we cannot handle IsDeactivating = false
            UpdateState(project?.Location, state => state.IsActivating = false);

            return TaskHelper.Completed;
        }

        private Task OnActivationCanceledAsync(IProject project)
        {
            IsRefreshingActiveProject = false;

            // Note: unfortunately we cannot handle IsDeactivating = false
            UpdateState(project?.Location, state => state.IsActivating = false);

            return TaskHelper.Completed;
        }

        private Task OnActivatedAsync(IProject oldProject, IProject newProject)
        {
            var stillRefreshingActiveProject = _isRefreshingActiveProject && newProject == null;
            if (!stillRefreshingActiveProject)
            {
                _lastActiveProject = newProject?.Location;
            }

            UpdateState(oldProject?.Location, state => state.IsDeactivating = false);
            UpdateState(newProject?.Location, state => state.IsActivating = false);

            if (!stillRefreshingActiveProject)
            {
                IsRefreshingActiveProject = false;
            }

            return TaskHelper.Completed;
        }

        protected virtual Task OnRefreshingAsync(ProjectCancelEventArgs e)
        {
            if (e.Location.EqualsIgnoreCase(_lastActiveProject))
            {
                IsRefreshingActiveProject = true;
            }

            UpdateState(e.Location, state => state.IsRefreshing = true);

            return TaskHelper.Completed;
        }

        private Task OnRefreshingFailedAsync(IProject project, Exception exception, IValidationContext validationContext)
        {
            IsRefreshingActiveProject = false;

            UpdateState(project?.Location, state => state.IsRefreshing = false);

            return TaskHelper.Completed;
        }

        private Task OnRefreshingCanceledAsync(IProject project)
        {
            IsRefreshingActiveProject = false;

            UpdateState(project?.Location, state => state.IsRefreshing = false);

            return TaskHelper.Completed;
        }

        private Task OnRefreshedAsync(IProject project)
        {
            // Note: we disable IsRefreshingActiveProject at Activated event

            UpdateState(project?.Location, state => state.IsRefreshing = false);

            return TaskHelper.Completed;
        }

        private void UpdateState(string location, Action<ProjectState> updateAction)
        {
            if (string.IsNullOrEmpty(location))
            {
                return;
            }

            ProjectState projectState = null;

            if (!_projectStates.TryGetValue(location, out projectState))
            {
                projectState = new ProjectState
                {
                    Location = location
                };

                _projectStates[location] = projectState;
            }

            updateAction(projectState);

            ProjectStateUpdated.SafeInvoke(this, () => new ProjectStateEventArgs(new ProjectState(projectState)));
        }
    }
}