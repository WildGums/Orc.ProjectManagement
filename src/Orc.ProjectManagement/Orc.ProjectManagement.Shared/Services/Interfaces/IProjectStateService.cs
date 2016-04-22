// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectStateService.cs" company="WildGums">
//   Copyright (c) 2008 - 2016 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System;

    public interface IProjectStateService
    {
        #region Properties
        bool IsRefreshingActiveProject { get; }
        #endregion

        #region Events
        event EventHandler<EventArgs> IsRefreshingActiveProjectUpdated;
        event EventHandler<ProjectStateEventArgs> ProjectStateUpdated;
        #endregion

        #region Methods
        ProjectState GetProjectState(IProject project);
        #endregion
    }
}