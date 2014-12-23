// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RefreshProjectWatcher.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement.Example.ProjectManagement
{
    using Catel;
    using Catel.Services;

    public class RefreshProjectWatcher : ProjectWatcherBase
    {
        private readonly IMessageService _messageService;

        public RefreshProjectWatcher(IProjectManager projectManager, IMessageService messageService) 
            : base(projectManager)
        {
            Argument.IsNotNull(() => messageService);

            _messageService = messageService;
        }

        protected override async void OnProjectRefreshRequired()
        {
            base.OnProjectRefreshRequired();

            if (await _messageService.Show("Detected a project change, do you want to refresh the project now?", "Refresh project?", MessageButton.YesNo) == MessageResult.Yes)
            {
                await ProjectManager.Refresh();
            }
        }
    }
}