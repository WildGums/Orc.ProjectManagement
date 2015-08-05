// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivationHistoryProjectWatcher.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System.Threading.Tasks;
    using Catel;

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

            await base.OnActivatedAsync(oldProject, newProject);
        }

        protected override async Task OnClosedAsync(IProject project)
        {
            if (project == null)
            {
                return;
            }

            _projectActivationHistoryService.Forget(project);

            var lastActiveProject = _projectActivationHistoryService.GetLastActiveProject();

            await ProjectManager.SetActiveProjectAsync(lastActiveProject);

            await base.OnClosedAsync(project);
        }
    }
}