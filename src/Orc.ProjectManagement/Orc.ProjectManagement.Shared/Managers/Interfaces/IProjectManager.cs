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
        [ObsoleteEx(ReplacementTypeOrMember = "ActiveProject", RemoveInVersion = "1.1.0", TreatAsErrorFromVersion = "1.0.0")]
        IProject Project { get; }

        IEnumerable<IProject> Projects { get; }

        [ObsoleteEx(ReplacementTypeOrMember = "ActiveProject.Location", RemoveInVersion = "1.1.0", TreatAsErrorFromVersion = "1.0.0")]
        string Location { get; }

        IProject ActiveProject { get; }
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

        [ObsoleteEx(Message = "Use ProjectActivated and ProjectRefreshed instead of it.", RemoveInVersion = "1.1.0", TreatAsErrorFromVersion = "1.0.0")]
        event EventHandler<ProjectUpdatedEventArgs> ProjectUpdated;

        event AsyncEventHandler<ProjectCancelEventArgs> ProjectRefreshing;
        event AsyncEventHandler<ProjectEventArgs> ProjectRefreshed;
        event AsyncEventHandler<ProjectEventArgs> ProjectRefreshingCanceled;
        event AsyncEventHandler<ProjectErrorEventArgs> ProjectRefreshingFailed;

        event EventHandler<ProjectEventArgs> ProjectRefreshRequired;

        event AsyncEventHandler<ProjectCancelEventArgs> ProjectClosing;
        event AsyncEventHandler<ProjectEventArgs> ProjectClosingCanceled;
        event AsyncEventHandler<ProjectEventArgs> ProjectClosed;

        event AsyncEventHandler<ProjectUpdatingCancelEventArgs> ProjectActivation;
        event AsyncEventHandler<ProjectUpdatedEventArgs> ProjectActivated;
        event AsyncEventHandler<ProjectEventArgs> ProjectActivationCanceled;
        event AsyncEventHandler<ProjectErrorEventArgs> ProjectActivationFailed;
        Task Initialize();
        Task<bool> Refresh();
        Task<bool> Refresh(IProject project);
        Task<bool> Load(string location);
        Task<bool> LoadInactive(string location);
        Task<bool> Save(string location = null);
        Task<bool> Save(IProject project, string location = null);
        Task<bool> Close();
        Task<bool> Close(IProject project);
        Task<bool> SetActiveProject(IProject project);
        IEnumerable<string> GetActivationHistory();
        #endregion
    }
}