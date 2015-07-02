// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CloseBeforeLoadProjectWatcher.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
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

        protected override async Task OnLoading(ProjectCancelEventArgs e)
        {
            if (e.Cancel || _projectManager.ActiveProject == null)
            {
                await base.OnLoading(e);
                return;
            }

            e.Cancel = !await _projectManager.Close();

            await base.OnLoading(e);
        }
    }
}