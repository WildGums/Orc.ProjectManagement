// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RefreshProjectWatcher.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement.Example.ProjectManagement
{
    using Catel;
    using Catel.Logging;
    using Catel.Services;

    public class RefreshProjectWatcher : ProjectWatcherBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IMessageService _messageService;
        private bool _isAwaitingFeedback;

        public RefreshProjectWatcher(IProjectManager projectManager, IMessageService messageService) 
            : base(projectManager)
        {
            Argument.IsNotNull(() => messageService);

            _messageService = messageService;
        }

        protected override async void OnRefreshRequired(IProject project)
        {
            if (_isAwaitingFeedback)
            {
                Log.Debug("Project requires refresh, but still awaiting feedback from a previous update, ignoring event");
                return;
            }

            _isAwaitingFeedback = true;

            base.OnRefreshRequired(project);

            if (await _messageService.Show("Detected a project change, do you want to refresh the project now?", "Refresh project?", MessageButton.YesNo) == MessageResult.Yes)
            {
                await ProjectManager.Refresh();
            }

            _isAwaitingFeedback = false;
        }
    }
}