// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RefreshProjectWatcher.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement.Example.ProjectManagement
{
    using System.Threading.Tasks;
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

        protected override async Task OnRefreshRequiredAsync(IProject project)
        {
            if (_isAwaitingFeedback)
            {
                Log.Debug("Project requires refresh, but still awaiting feedback from a previous update, ignoring event");
                return;
            }

            _isAwaitingFeedback = true;

            await base.OnRefreshRequiredAsync(project).ConfigureAwait(false);

            if (await _messageService.ShowAsync("Detected a project change, do you want to refresh the project now?", "Refresh project?", MessageButton.YesNo).ConfigureAwait(false) == MessageResult.Yes)
            {
                await ProjectManager.RefreshAsync().ConfigureAwait(false);
            }

            _isAwaitingFeedback = false;
        }
    }
}