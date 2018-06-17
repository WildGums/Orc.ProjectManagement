// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CloseBeforeLoadProjectWorkflowItem.cs" company="WildGums">
//   Copyright (c) 2008 - 2018 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;

    public class CloseBeforeLoadProjectWorkflowItem : ProjectManagerWorkflowItemBase
    {
        #region Fields
        private readonly IProjectManager _projectManager;
        #endregion

        #region Constructors
        public CloseBeforeLoadProjectWorkflowItem(IProjectManager projectManager)
        {
            Argument.IsNotNull(() => projectManager);

            _projectManager = projectManager;
        }
        #endregion

        public override async Task<bool> LoadingAsync(string location)
        {
            if (!await base.LoadingAsync(location))
            {
                return false;
            }

            foreach (var project in _projectManager.Projects.ToList())
            {
                return !await _projectManager.CloseAsync(project);
            }

            return true;
        }
    }
}
