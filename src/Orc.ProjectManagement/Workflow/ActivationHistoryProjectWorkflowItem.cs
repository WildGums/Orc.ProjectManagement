// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivationHistoryProjectWorkflowItem.cs" company="WildGums">
//   Copyright (c) 2008 - 2018 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Data;

    public class ActivationHistoryProjectWorkflowItem : ProjectManagerWorkflowItemBase
    {
        #region Fields
        private readonly IProjectManager _projectManager;
        private readonly IProjectActivationHistoryService _projectActivationHistoryService;
        #endregion

        #region Constructors
        public ActivationHistoryProjectWorkflowItem(IProjectManager projectManager, IProjectActivationHistoryService projectActivationHistoryService)
        {
            Argument.IsNotNull(() => projectManager);
            Argument.IsNotNull(() => projectActivationHistoryService);

            _projectManager = projectManager;
            _projectActivationHistoryService = projectActivationHistoryService;

            _projectActivationHistoryService.SetProjectsSource(projectManager.Projects);
        }
        #endregion

        public override async Task ActivatedAsync(IProject oldProject, IProject newProject)
        {
            await base.ActivatedAsync(oldProject, newProject);

            if (newProject is null)
            {
                return;
            }

            _projectActivationHistoryService.Remember(newProject);
        }

        public override async Task ClosedAsync(IProject project)
        {
            await base.ClosedAsync(project);

            if (project is null)
            {
                return;
            }

            _projectActivationHistoryService.Forget(project);

            var lastActiveProject = _projectActivationHistoryService.GetLastActiveProject();

            await _projectManager.SetActiveProjectAsync(lastActiveProject).ConfigureAwait(false);
        }

        public override async Task LoadingFailedAsync(string location, Exception exception, IValidationContext validationContext)
        {
            await base.LoadingFailedAsync(location, exception, validationContext);

            if (_projectManager.ActiveProject is null)
            {
                return;
            }

            var lastActiveProject = _projectActivationHistoryService.GetLastActiveProject();
            await _projectManager.SetActiveProjectAsync(lastActiveProject);
        }
    }
}
