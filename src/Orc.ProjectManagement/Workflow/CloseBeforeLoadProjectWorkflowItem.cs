namespace Orc.ProjectManagement
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class CloseBeforeLoadProjectWorkflowItem : ProjectManagerWorkflowItemBase
    {
        private readonly IProjectManager _projectManager;

        public CloseBeforeLoadProjectWorkflowItem(IProjectManager projectManager)
        {
            ArgumentNullException.ThrowIfNull(projectManager);

            _projectManager = projectManager;
        }

        public override async Task<bool> LoadingAsync(string location)
        {
            if (!await base.LoadingAsync(location))
            {
                return false;
            }

            foreach (var project in _projectManager.Projects.ToList())
            {
                if (!await _projectManager.CloseAsync(project))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
