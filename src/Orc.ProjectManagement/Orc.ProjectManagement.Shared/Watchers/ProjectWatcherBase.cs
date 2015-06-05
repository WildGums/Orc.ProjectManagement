// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectWatcherBase.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
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

            Init();
        }
        #endregion

        #region Properties
        protected IProjectManager ProjectManager
        {
            get { return _projectManager; }
        }
        #endregion

        private void Init()
        {
            var type = this.GetType();
            var baseType = type.BaseType;

            if (baseType == null)
            {
                return;
            }

            var methodInfos = from method in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                where method.GetBaseDefinition().DeclaringType != method.DeclaringType
                from subscriber in baseType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                let subscriberName = "Subscribe" + method.Name
                where string.Equals(subscriber.Name, subscriberName)
                select subscriber;

            foreach (var methodInfo in methodInfos)
            {
                methodInfo.Invoke(this, new object[0]);
            }
        }

        private void SubscribeOnLoading()
        {
            _projectManager.ProjectLoading += async (sender, e) => await OnLoading(e);
        }

        private void SubscribeOnLoadingFailed()
        {
            _projectManager.ProjectLoadingFailed += async (sender, e) => await OnLoadingFailed(e.Location, e.Exception, e.ValidationContext);
        }

        private void SubscribeOnLoadingCanceled()
        {
            _projectManager.ProjectLoadingCanceled += async (sender, e) => await OnLoadingCanceled(e.Location);
        }

        private void SubscribeOnLoaded()
        {
            _projectManager.ProjectLoaded += async (sender, e) => await OnLoaded(e.Project);
        }

        private void SubscribeOnSaving()
        {
            _projectManager.ProjectSaving += async (sender, e) => await OnSaving(e);
        }

        private void SubscribeOnSavingCanceled()
        {
            _projectManager.ProjectSavingCanceled += async (sender, e) => await OnSavingCanceled(e.Project);
        }

        private void SubscribeOnSavingFailed()
        {
            _projectManager.ProjectSavingFailed += async (sender, e) => await OnSavingFailed(e.Project, e.Exception);
        }

        private void SubscribeOnSaved()
        {
            _projectManager.ProjectSaved += async (sender, e) => await OnSaved(e.Project);
        }

        private void SubscribeOnClosing()
        {
            _projectManager.ProjectClosing += async (sender, e) => await OnClosing(e);
        }

        private void SubscribeOnClosed()
        {
            _projectManager.ProjectClosed += async (sender, e) => await OnClosed(e.Project);
        }

        private void SubscribeOnClosingCanceled()
        {
            _projectManager.ProjectClosingCanceled += async (sender, e) => await OnClosingCanceled(e.Project);
        }

        private void SubscribeOnRefreshRequired()
        {
            _projectManager.ProjectRefreshRequired += OnProjectRefreshRequired;
        }

        private void OnProjectRefreshRequired(object sender, ProjectEventArgs e)
        {
            foreach (var project in _projectManager.Projects.Where(project => string.Equals(project.Location, e.Location)))
            {
                OnRefreshRequired(project);
            }
        }

        private void SubscribeOnActivated()
        {
            _projectManager.ProjectActivated += async (sender, e) => await OnActivated(e.OldProject, e.NewProject);
            ;
        }

        private void SubscribeOnActivationFailed()
        {
            _projectManager.ProjectActivationFailed += async (sender, e) => await OnActivationFailed(e.Project, e.Exception);
            ;
        }

        private void SubscribeOnActivationCanceled()
        {
            _projectManager.ProjectActivationCanceled += async (sender, e) => await OnActivationCanceled(e.Project);
            ;
        }

        private void SubscribeOnActivation()
        {
            _projectManager.ProjectActivation += async (sender, e) => await OnActivation(e);
            ;
        }

        private void SubscribeOnRefreshed()
        {
            _projectManager.ProjectRefreshed += async (sender, e) => await OnRefreshed(e.Project);
        }

        private void SubscribeOnRefreshingFailed()
        {
            _projectManager.ProjectRefreshingFailed += async (sender, e) => await OnRefreshingFailed(e.Project, e.Exception, e.ValidationContext);
            ;
        }

        private void SubscribeOnRefreshingCanceled()
        {
            _projectManager.ProjectRefreshingCanceled += async (sender, e) => await OnRefreshingCanceled(e.Project);
            ;
        }

        private void SubscribeOnRefreshing()
        {
            _projectManager.ProjectRefreshing += async (sender, e) => await OnRefreshing(e);
        }

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

        protected virtual async Task OnSavingCanceled(IProject project)
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

        protected virtual async Task OnClosingCanceled(IProject project)
        {
        }

        [ObsoleteEx(Message = "Use OnProjectActivated and OnRefreshed instead of it.", RemoveInVersion = "1.1.0", TreatAsErrorFromVersion = "1.0.0")]
        protected virtual void OnUpdated(IProject oldProject, IProject newProject, bool isRefresh)
        {
        }

        protected virtual void OnRefreshRequired(IProject project)
        {
        }

        [ObsoleteEx(ReplacementTypeOrMember = "OnRefreshRequired", RemoveInVersion = "1.1.0", TreatAsErrorFromVersion = "1.0.0")]
        protected virtual void OnProjectRefreshRequired(IProject project)
        {
        }

        protected virtual async Task OnActivated(IProject oldProject, IProject newProject)
        {
        }

        protected virtual async Task OnActivationFailed(IProject project, Exception exception)
        {
        }

        protected virtual async Task OnActivationCanceled(IProject project)
        {
        }

        protected virtual async Task OnActivation(ProjectUpdatingCancelEventArgs e)
        {
        }

        protected virtual async Task OnRefreshed(IProject project)
        {
        }

        protected virtual async Task OnRefreshingFailed(IProject project, Exception exception, IValidationContext validationContext)
        {
        }

        protected virtual async Task OnRefreshingCanceled(IProject project)
        {
        }

        protected virtual async Task OnRefreshing(ProjectCancelEventArgs e)
        {
        }
    }
}