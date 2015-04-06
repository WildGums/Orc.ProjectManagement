// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectManager.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System;
    using System.Threading.Tasks;

    public interface IProjectManager
    {
        #region Properties
        IProject Project { get; }
        string Location { get; }
        #endregion

        #region Methods
        event AsyncEventHandler<ProjectCancelEventArgs> ProjectLoading;
        event AsyncEventHandler<ProjectErrorEventArgs> ProjectLoadingFailed;
        event AsyncEventHandler<ProjectEventArgs> ProjectLoadingCanceled;
        event AsyncEventHandler<ProjectEventArgs> ProjectLoaded;
        event AsyncEventHandler<ProjectCancelEventArgs> ProjectSaving;
        event AsyncEventHandler<ProjectErrorEventArgs> ProjectSavingFailed;
        event AsyncEventHandler<ProjectEventArgs> ProjectSavingCanceled;
        event AsyncEventHandler<ProjectEventArgs> ProjectSaved;
        event EventHandler<ProjectUpdatedEventArgs> ProjectUpdated;
        event EventHandler<EventArgs> ProjectRefreshRequired;
        event AsyncEventHandler<ProjectCancelEventArgs> ProjectClosing;
        event AsyncEventHandler<ProjectEventArgs> ProjectClosingCanceled;
        event AsyncEventHandler<ProjectEventArgs> ProjectClosed;
        Task Initialize();
        Task Refresh();
        Task Load(string location);
        Task Save(string location = null);
        Task Close();
        #endregion
    }
}