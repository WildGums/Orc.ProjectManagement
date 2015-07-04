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
        }
        #endregion

        protected override async Task OnActivated(IProject oldProject, IProject newProject)
        {
            if (newProject == null)
            {
                return;
            }

            _projectActivationHistoryService.Remember(newProject);

            await base.OnActivated(oldProject, newProject);
        }

        protected override async Task OnClosed(IProject project)
        {
            if (project == null)
            {
                return;
            }

            _projectActivationHistoryService.Forget(project);

            var lastActiveProject = _projectActivationHistoryService.GetLastActiveProject();
            if (lastActiveProject == null)
            {
                return;
            }

            await ProjectManager.SetActiveProject(lastActiveProject);

            await base.OnClosed(project);
        }
    }
}