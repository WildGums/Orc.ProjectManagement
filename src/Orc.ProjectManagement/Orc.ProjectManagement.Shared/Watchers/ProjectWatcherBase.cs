// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectWatcherBase.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.ProjectManagement
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Data;
    using Catel.Threading;

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

            InitSubscriptions();
        }
        #endregion

        #region Properties
        protected IProjectManager ProjectManager
        {
            get { return _projectManager; }
        }

        protected bool IsRefreshing { get; private set; }

        protected bool IsActivating { get; private set; }

        protected bool IsLoading { get; private set; }

        protected bool IsSaving { get; private set; }

        protected bool IsClosing { get; private set; }
        #endregion

        private void InitSubscriptions()
        {
            var type = GetType();

            var baseType = typeof (ProjectWatcherBase);

            var methodInfos = from method in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                              where method.GetBaseDefinition().DeclaringType != method.DeclaringType
                              from subscriber in baseType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                              let methodName = "Subscribe" + method.Name
                              let subscriberName = subscriber.Name
                              let subscriberNameAsync = subscriberName + "Async"
                              where string.Equals(subscriberName, methodName) || string.Equals(subscriberNameAsync, methodName)
                              select subscriber;

            foreach (var methodInfo in methodInfos)
            {
                methodInfo.Invoke(this, new object[0]);
            }
        }

        private void SubscribeOnLoading()
        {
            _projectManager.ProjectLoadingAsync += async (sender, e) => await OnLoadingInternalAsync(e).ConfigureAwait(false);
        }

        private void SubscribeOnLoadingFailed()
        {
            _projectManager.ProjectLoadingFailedAsync += async (sender, e) => await OnLoadingFailedInternalAsync(e.Location, e.Exception, e.ValidationContext).ConfigureAwait(false);
        }

        private void SubscribeOnLoadingCanceled()
        {
            _projectManager.ProjectLoadingCanceledAsync += async (sender, e) => await OnLoadingCanceledInternalAsync(e.Location).ConfigureAwait(false);
        }

        private void SubscribeOnLoaded()
        {
            _projectManager.ProjectLoadedAsync += async (sender, e) => await OnLoadedInternalAsync(e.Project).ConfigureAwait(false);
        }

        private void SubscribeOnSaving()
        {
            _projectManager.ProjectSavingAsync += async (sender, e) => await OnSavingInternalAsync(e).ConfigureAwait(false);
        }

        private void SubscribeOnSavingCanceled()
        {
            _projectManager.ProjectSavingCanceledAsync += async (sender, e) => await OnSavingCanceledInternalAsync(e.Project).ConfigureAwait(false);
        }

        private void SubscribeOnSavingFailed()
        {
            _projectManager.ProjectSavingFailedAsync += async (sender, e) => await OnSavingFailedInternalAsync(e.Project, e.Exception).ConfigureAwait(false);
        }

        private void SubscribeOnSaved()
        {
            _projectManager.ProjectSavedAsync += async (sender, e) => await OnSavedInternalAsync(e.Project).ConfigureAwait(false);
        }

        private void SubscribeOnClosing()
        {
            _projectManager.ProjectClosingAsync += async (sender, e) => await OnClosingInternalAsync(e).ConfigureAwait(false);
        }

        private void SubscribeOnClosed()
        {
            _projectManager.ProjectClosedAsync += async (sender, e) => await OnClosedInternalAsync(e.Project).ConfigureAwait(false);
        }

        private void SubscribeOnClosingCanceled()
        {
            _projectManager.ProjectClosingCanceledAsync += async (sender, e) => await OnClosingCanceledInternalAsync(e.Project).ConfigureAwait(false);
        }

        private void SubscribeOnRefreshRequired()
        {
            _projectManager.ProjectRefreshRequiredAsync += async (sender, e) => await OnProjectRefreshRequiredAsync(e).ConfigureAwait(false);
        }

        private void SubscribeOnActivated()
        {
            _projectManager.ProjectActivatedAsync += async (sender, e) => await OnActivatedInternalAsync(e.OldProject, e.NewProject).ConfigureAwait(false);
        }

        private void SubscribeOnActivationFailed()
        {
            _projectManager.ProjectActivationFailedAsync += async (sender, e) => await OnActivationFailedInternalAsync(e.Project, e.Exception).ConfigureAwait(false);
        }

        private void SubscribeOnActivationCanceled()
        {
            _projectManager.ProjectActivationCanceledAsync += async (sender, e) => await OnActivationCanceledInternalAsync(e.Project).ConfigureAwait(false);
        }

        private void SubscribeOnActivation()
        {
            _projectManager.ProjectActivationAsync += async (sender, e) => await OnActivationInternalAsync(e).ConfigureAwait(false);
        }

        private void SubscribeOnRefreshed()
        {
            _projectManager.ProjectRefreshedAsync += async (sender, e) => await OnRefreshedInternalAsync(e.Project).ConfigureAwait(false);
        }

        private void SubscribeOnRefreshingFailed()
        {
            _projectManager.ProjectRefreshingFailedAsync += async (sender, e) => await OnRefreshingFailedInternalAsync(e.Project, e.Exception, e.ValidationContext).ConfigureAwait(false);
        }

        private void SubscribeOnRefreshingCanceled()
        {
            _projectManager.ProjectRefreshingCanceledAsync += async (sender, e) => await OnRefreshingCanceledInternalAsync(e.Project).ConfigureAwait(false);
        }

        private void SubscribeOnRefreshing()
        {
            _projectManager.ProjectRefreshingAsync += async (sender, e) => await OnRefreshingInternalAsync(e).ConfigureAwait(false);
        }

        private async Task OnProjectRefreshRequiredAsync(ProjectEventArgs e)
        {
            var projects = _projectManager.Projects.Where(project => string.Equals(project.Location, e.Location)).ToList();
            var projectFileSystemEventArgs = e as ProjectFileSystemEventArgs;

            foreach (var project in projects)
            {
                await OnRefreshRequiredInternalAsync(project).ConfigureAwait(false);
            }

            if (projectFileSystemEventArgs == null)
            {
                return;
            }

            var fileNames = projectFileSystemEventArgs.FileNames;
            foreach (var project in projects)
            {
                await OnRefreshRequiredInternalAsync(project, fileNames).ConfigureAwait(false);
            }
        }

        private Task OnLoadingInternalAsync(ProjectCancelEventArgs e)
        {
            IsLoading = true;

            return OnLoadingAsync(e);
        }

        protected virtual Task OnLoadingAsync(ProjectCancelEventArgs e)
        {
            return TaskHelper.Completed;
        }

        private Task OnLoadingFailedInternalAsync(string location, Exception exception, IValidationContext validationContext)
        {
            IsLoading = false;

            return OnLoadingFailedAsync(location, exception, validationContext);
        }

        protected virtual Task OnLoadingFailedAsync(string location, Exception exception, IValidationContext validationContext)
        {
            return TaskHelper.Completed;
        }

        private Task OnLoadingCanceledInternalAsync(string location)
        {
            IsLoading = false;

            return OnLoadingCanceledAsync(location);
        }

        protected virtual Task OnLoadingCanceledAsync(string location)
        {
            return TaskHelper.Completed;
        }

        private Task OnLoadedInternalAsync(IProject project)
        {
            IsLoading = false;

            return OnLoadedAsync(project);
        }

        protected virtual Task OnLoadedAsync(IProject project)
        {
            return TaskHelper.Completed;
        }

        private Task OnSavingInternalAsync(ProjectCancelEventArgs e)
        {
            IsSaving = true;

            return OnSavingAsync(e);
        }

        protected virtual Task OnSavingAsync(ProjectCancelEventArgs e)
        {
            return TaskHelper.Completed;
        }

        private Task OnSavingCanceledInternalAsync(IProject project)
        {
            IsSaving = false;

            return OnSavingCanceledAsync(project);
        }

        protected virtual Task OnSavingCanceledAsync(IProject project)
        {
            return TaskHelper.Completed;
        }

        private Task OnSavingFailedInternalAsync(IProject project, Exception exception)
        {
            IsSaving = false;

            return OnSavingFailedAsync(project, exception);
        }

        protected virtual Task OnSavingFailedAsync(IProject project, Exception exception)
        {
            return TaskHelper.Completed;
        }

        private Task OnSavedInternalAsync(IProject project)
        {
            IsSaving = false;

            return OnSavedAsync(project);
        }

        protected virtual Task OnSavedAsync(IProject project)
        {
            return TaskHelper.Completed;
        }

        private Task OnClosingInternalAsync(ProjectCancelEventArgs e)
        {
            IsClosing = true;

            return OnClosingAsync(e);
        }

        protected virtual Task OnClosingAsync(ProjectCancelEventArgs e)
        {
            return TaskHelper.Completed;
        }

        private Task OnClosedInternalAsync(IProject project)
        {
            IsClosing = false;

            return OnClosedAsync(project);
        }

        protected virtual Task OnClosedAsync(IProject project)
        {
            return TaskHelper.Completed;
        }

        private Task OnClosingCanceledInternalAsync(IProject project)
        {
            IsClosing = false;

            return OnClosingCanceledAsync(project);
        }

        protected virtual Task OnClosingCanceledAsync(IProject project)
        {
            return TaskHelper.Completed;
        }

        private Task OnRefreshRequiredInternalAsync(IProject project, params string[] fileNames)
        {
            return OnRefreshRequiredAsync(project, fileNames);
        }

        protected virtual Task OnRefreshRequiredAsync(IProject project, params string[] fileNames)
        {
            return TaskHelper.Completed;
        }

        private Task OnActivatedInternalAsync(IProject oldProject, IProject newProject)
        {
            IsActivating = false;

            return OnActivatedAsync(oldProject, newProject);
        }

        protected virtual Task OnActivatedAsync(IProject oldProject, IProject newProject)
        {
            return TaskHelper.Completed;
        }

        private Task OnActivationFailedInternalAsync(IProject project, Exception exception)
        {
            IsActivating = false;

            return OnActivationFailedAsync(project, exception);
        }

        protected virtual Task OnActivationFailedAsync(IProject project, Exception exception)
        {
            return TaskHelper.Completed;
        }

        private Task OnActivationCanceledInternalAsync(IProject project)
        {
            IsActivating = false;

            return OnActivationCanceledAsync(project);
        }

        protected virtual Task OnActivationCanceledAsync(IProject project)
        {
            return TaskHelper.Completed;
        }

        private Task OnActivationInternalAsync(ProjectUpdatingCancelEventArgs e)
        {
            IsActivating = true;

            return OnActivationAsync(e);
        }

        protected virtual Task OnActivationAsync(ProjectUpdatingCancelEventArgs e)
        {
            return TaskHelper.Completed;
        }

        private Task OnRefreshedInternalAsync(IProject project)
        {
            IsRefreshing = false;

            return OnRefreshedAsync(project);
        }

        protected virtual Task OnRefreshedAsync(IProject project)
        {
            return TaskHelper.Completed;
        }

        private Task OnRefreshingFailedInternalAsync(IProject project, Exception exception, IValidationContext validationContext)
        {
            IsRefreshing = false;

            return OnRefreshingFailedAsync(project, exception, validationContext);
        }

        protected virtual Task OnRefreshingFailedAsync(IProject project, Exception exception, IValidationContext validationContext)
        {
            return TaskHelper.Completed;
        }

        private Task OnRefreshingCanceledInternalAsync(IProject project)
        {
            IsRefreshing = false;

            return OnRefreshingCanceledAsync(project);
        }

        protected virtual Task OnRefreshingCanceledAsync(IProject project)
        {
            return TaskHelper.Completed;
        }

        protected virtual Task OnRefreshingInternalAsync(ProjectCancelEventArgs e)
        {
            IsRefreshing = true;

            return OnRefreshingAsync(e);
        }

        protected virtual Task OnRefreshingAsync(ProjectCancelEventArgs e)
        {
            return TaskHelper.Completed;
        }
    }
}