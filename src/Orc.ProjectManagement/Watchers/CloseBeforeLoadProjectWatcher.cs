namespace Orc.ProjectManagement;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class CloseBeforeLoadProjectWatcher : ProjectWatcherBase
{
    private readonly CloseBeforeLoadProjectWorkflowItem _closeBeforeLoadProjectWorkflowItem;

    public CloseBeforeLoadProjectWatcher(IProjectManager projectManager, IServiceProvider serviceProvider)
        : base(projectManager)
    {
        _closeBeforeLoadProjectWorkflowItem = ActivatorUtilities.CreateInstance<CloseBeforeLoadProjectWorkflowItem>(serviceProvider, projectManager);
    }

    protected override async Task OnLoadingAsync(ProjectCancelEventArgs e)
    {
        if (e.Cancel)
        {
            return;
        }

        if (e.Location is null)
        {
            return;
        }

        e.Cancel = !await _closeBeforeLoadProjectWorkflowItem.LoadingAsync(e.Location);
    }
}
