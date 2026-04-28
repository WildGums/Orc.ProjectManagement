namespace Orc.ProjectManagement;

using System;
using System.Threading.Tasks;
using Catel.Data;
using Microsoft.Extensions.DependencyInjection;

internal class ActivationHistoryProjectWatcher : ProjectWatcherBase
{
    private readonly ActivationHistoryProjectWorkflowItem _activationHistoryProjectWorkflowItem;

    public ActivationHistoryProjectWatcher(IProjectManager projectManager, IServiceProvider serviceProvider)
        : base(projectManager)
    {
        _activationHistoryProjectWorkflowItem = ActivatorUtilities.CreateInstance<ActivationHistoryProjectWorkflowItem>(serviceProvider, projectManager);
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
