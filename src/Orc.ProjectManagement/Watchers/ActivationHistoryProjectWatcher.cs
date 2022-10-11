namespace Orc.ProjectManagement
{
    using System;
    using System.Threading.Tasks;
    using Catel.Data;
    using Catel.IoC;

    internal class ActivationHistoryProjectWatcher : ProjectWatcherBase
    {
        private readonly ActivationHistoryProjectWorkflowItem _activationHistoryProjectWorkflowItem;

        public ActivationHistoryProjectWatcher(IProjectManager projectManager, 
            ITypeFactory typeFactory)
            : base(projectManager)
        {
            ArgumentNullException.ThrowIfNull(typeFactory);

            _activationHistoryProjectWorkflowItem = typeFactory.CreateRequiredInstanceWithParametersAndAutoCompletion<ActivationHistoryProjectWorkflowItem>(projectManager);
        }

        protected override Task OnActivatedAsync(IProject? oldProject, IProject? newProject)
        {
            return _activationHistoryProjectWorkflowItem.ActivatedAsync(oldProject, newProject);
        }

        protected override Task OnClosedAsync(IProject project)
        {
            return _activationHistoryProjectWorkflowItem.ClosedAsync(project);
        }

        protected override Task OnLoadingFailedAsync(string location, Exception? exception, IValidationContext validationContext)
        {
            return _activationHistoryProjectWorkflowItem.LoadingFailedAsync(location, exception, validationContext);
        }
    }
}
