// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CloseBeforeLoadProjectWatcher.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System.Threading.Tasks;
    using Catel;
    using Catel.IoC;

    public class CloseBeforeLoadProjectWatcher : ProjectWatcherBase
    {
        #region Fields
        private readonly CloseBeforeLoadProjectWorkflowItem _closeBeforeLoadProjectWorkflowItem;
        #endregion

        #region Constructors
        public CloseBeforeLoadProjectWatcher(IProjectManager projectManager, ITypeFactory typeFactory)
            : base(projectManager)
        {
            Argument.IsNotNull(() => typeFactory);

            _closeBeforeLoadProjectWorkflowItem = typeFactory.CreateInstanceWithParametersAndAutoCompletion<CloseBeforeLoadProjectWorkflowItem>(projectManager);
        }
        #endregion

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
