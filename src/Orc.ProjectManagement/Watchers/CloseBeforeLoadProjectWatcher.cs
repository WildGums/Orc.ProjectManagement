namespace Orc.ProjectManagement
{
    using System;
    using System.Threading.Tasks;
    using Catel.IoC;

    public class CloseBeforeLoadProjectWatcher : ProjectWatcherBase
    {
        private readonly CloseBeforeLoadProjectWorkflowItem _closeBeforeLoadProjectWorkflowItem;

        public CloseBeforeLoadProjectWatcher(IProjectManager projectManager, ITypeFactory typeFactory)
            : base(projectManager)
        {
            ArgumentNullException.ThrowIfNull(typeFactory);

            _closeBeforeLoadProjectWorkflowItem = typeFactory.CreateRequiredInstanceWithParametersAndAutoCompletion<CloseBeforeLoadProjectWorkflowItem>(projectManager);
        }
        
        protected override async Task OnLoadingAsync(ProjectCancelEventArgs e)
        {
            if (e.Cancel)
            {
                return;
            }

            e.Cancel = !await _closeBeforeLoadProjectWorkflowItem.LoadingAsync(e.Location);
        }
    }
}
