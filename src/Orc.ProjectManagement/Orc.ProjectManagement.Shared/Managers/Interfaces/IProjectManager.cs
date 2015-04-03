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
        event EventHandler<ProjectErrorEventArgs> ProjectLoadingFailed;
        event EventHandler<ProjectEventArgs> ProjectLoadingCanceled;
        event EventHandler<ProjectEventArgs> ProjectLoaded;
        event EventHandler<ProjectCancelEventArgs> ProjectSaving;
        event EventHandler<ProjectErrorEventArgs> ProjectSavingFailed;
        event EventHandler<ProjectEventArgs> ProjectSavingCanceled;
        event EventHandler<ProjectEventArgs> ProjectSaved;
        event EventHandler<ProjectUpdatedEventArgs> ProjectUpdated;
        event EventHandler<EventArgs> ProjectRefreshRequired;
        event EventHandler<ProjectCancelEventArgs> ProjectClosing;
        event EventHandler<ProjectEventArgs> ProjectClosingCanceled;
        event EventHandler<ProjectEventArgs> ProjectClosed;
        Task Initialize();
        Task Refresh();
        Task Load(string location);
        Task Save(string location = null);
        void Close();
        #endregion
    }
}