// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectManager.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Catel;

    public interface IProjectManager
    {
        #region Properties
        IEnumerable<IProject> Projects { get; }

        IProject ActiveProject { get; }
        ProjectManagementType ProjectManagementType { get; }
        #endregion

        #region Methods
        event AsyncEventHandler<ProjectCancelEventArgs> ProjectLoadingAsync;
        event AsyncEventHandler<ProjectErrorEventArgs> ProjectLoadingFailedAsync;
        event AsyncEventHandler<ProjectEventArgs> ProjectLoadingCanceledAsync;
        event AsyncEventHandler<ProjectEventArgs> ProjectLoadedAsync;

        event AsyncEventHandler<ProjectCancelEventArgs> ProjectSavingAsync;
        event AsyncEventHandler<ProjectErrorEventArgs> ProjectSavingFailedAsync;
        event AsyncEventHandler<ProjectEventArgs> ProjectSavingCanceledAsync;
        event AsyncEventHandler<ProjectEventArgs> ProjectSavedAsync;

        event AsyncEventHandler<ProjectCancelEventArgs> ProjectRefreshingAsync;
        event AsyncEventHandler<ProjectEventArgs> ProjectRefreshedAsync;
        event AsyncEventHandler<ProjectEventArgs> ProjectRefreshingCanceledAsync;
        event AsyncEventHandler<ProjectErrorEventArgs> ProjectRefreshingFailedAsync;

        event EventHandler<ProjectEventArgs> ProjectRefreshRequiredAsync;

        event AsyncEventHandler<ProjectCancelEventArgs> ProjectClosingAsync;
        event AsyncEventHandler<ProjectEventArgs> ProjectClosingCanceledAsync;
        event AsyncEventHandler<ProjectEventArgs> ProjectClosedAsync;

        event AsyncEventHandler<ProjectUpdatingCancelEventArgs> ProjectActivationAsync;
        event AsyncEventHandler<ProjectUpdatedEventArgs> ProjectActivatedAsync;
        event AsyncEventHandler<ProjectEventArgs> ProjectActivationCanceledAsync;
        event AsyncEventHandler<ProjectErrorEventArgs> ProjectActivationFailedAsync;

        Task InitializeAsync();
        Task<bool> RefreshAsync();
        Task<bool> RefreshAsync(IProject project);
        Task<bool> LoadAsync(string location);
        Task<bool> LoadInactiveAsync(string location);
        Task<bool> SaveAsync(string location = null);
        Task<bool> SaveAsync(IProject project, string location = null);
        Task<bool> CloseAsync();
        Task<bool> CloseAsync(IProject project);
        Task<bool> SetActiveProjectAsync(IProject project);
        #endregion
    }
}