// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CloseBeforeLoadProjectWatcher.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System.Linq;
    using System.Threading.Tasks;

    public class CloseBeforeLoadProjectWatcher : ProjectWatcherBase
    {
        #region Fields
        private readonly IProjectManager _projectManager;
        #endregion

        #region Constructors
        public CloseBeforeLoadProjectWatcher(IProjectManager projectManager)
            : base(projectManager)
        {
            _projectManager = projectManager;
        }
        #endregion

        protected override async Task OnLoadingAsync(ProjectCancelEventArgs e)
        {
            if (e.Cancel)
            {
                await base.OnLoadingAsync(e).ConfigureAwait(false);
                return;
            }

            foreach (var project in _projectManager.Projects.ToList())
            {
                e.Cancel = e.Cancel || !await _projectManager.CloseAsync(project).ConfigureAwait(false);
            }

            await base.OnLoadingAsync(e).ConfigureAwait(false);
        }
    }
}