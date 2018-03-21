// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivationHistoryProjectWatcher.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Data;

    internal class ActivationHistoryProjectWatcher : ProjectWatcherBase
    {
        #region Fields
        private readonly IProjectActivationHistoryService _projectActivationHistoryService;
        #endregion

        #region Constructors
        public ActivationHistoryProjectWatcher(IProjectManager projectManager, IProjectActivationHistoryService projectActivationHistoryService)
            : base(projectManager)
        {
            Argument.IsNotNull(() => projectActivationHistoryService);

            _projectActivationHistoryService = projectActivationHistoryService;

            _projectActivationHistoryService.SetProjectsSource(ProjectManager.Projects);
        }
        #endregion

        protected override async Task OnActivatedAsync(IProject oldProject, IProject newProject)
        {
            if (newProject == null)
            {
                return;
            }

            _projectActivationHistoryService.Remember(newProject);

            await base.OnActivatedAsync(oldProject, newProject).ConfigureAwait(false);
        }

        protected override async Task OnClosedAsync(IProject project)
        {
            if (project == null)
            {
                return;
            }

            _projectActivationHistoryService.Forget(project);

            var lastActiveProject = _projectActivationHistoryService.GetLastActiveProject();

            await ProjectManager.SetActiveProjectAsync(lastActiveProject).ConfigureAwait(false);

            await base.OnClosedAsync(project).ConfigureAwait(false);
        }

        protected override async Task OnLoadingFailedAsync(string location, Exception exception, IValidationContext validationContext)
        {
            await base.OnLoadingFailedAsync(location, exception, validationContext);
            if (ProjectManager.ActiveProject == null)
            {
                return;
            }

            var lastActiveProject = _projectActivationHistoryService.GetLastActiveProject();
            await ProjectManager.SetActiveProjectAsync(lastActiveProject).ConfigureAwait(false);
        }
    }
}