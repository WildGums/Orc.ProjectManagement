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
        [ObsoleteEx(ReplacementTypeOrMember = "CurrentProject", RemoveInVersion = "1.1.0", TreatAsErrorFromVersion = "1.0.0")]
        IProject Project { get; }

        IEnumerable<IProject> Projects { get; }

        [ObsoleteEx(ReplacementTypeOrMember = "CurrentProject.Location", RemoveInVersion = "1.1.0", TreatAsErrorFromVersion = "1.0.0")]
        string Location { get; }

        IProject CurrentProject { get; }
        #endregion

        #region Methods
        [ObsoleteEx(ReplacementTypeOrMember = "ProjectLocationLoading", RemoveInVersion = "1.1.0", TreatAsErrorFromVersion = "1.0.0")]
        event AsyncEventHandler<ProjectCancelEventArgs> ProjectLoading;
        [ObsoleteEx(ReplacementTypeOrMember = "ProjectLocationLoadingFailed", RemoveInVersion = "1.1.0", TreatAsErrorFromVersion = "1.0.0")]
        event AsyncEventHandler<ProjectErrorEventArgs> ProjectLoadingFailed;
        [ObsoleteEx(ReplacementTypeOrMember = "ProjectLocationLoadingCanceled", RemoveInVersion = "1.1.0", TreatAsErrorFromVersion = "1.0.0")]
        event AsyncEventHandler<ProjectEventArgs> ProjectLoadingCanceled;

        event AsyncEventHandler<ProjectLocationCancelEventArgs> ProjectLocationLoading;
        event AsyncEventHandler<ProjectLocationErrorEventArgs> ProjectLocationLoadingFailed;
        event AsyncEventHandler<ProjectLocationEventArgs> ProjectLocationLoadingCanceled;
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
        event AsyncEventHandler<ProjectUpdatedCancelEventArgs> CurrentProjectChanging;
        event AsyncEventHandler<ProjectUpdatedEventArgs> CurrentProjectChanged;
        event AsyncEventHandler<ProjectEventArgs> ChangingCurrentProjectCanceled;
        event AsyncEventHandler<ProjectErrorEventArgs> ChangingCurrentProjectFailed;
        Task Initialize();
        Task Refresh();
        Task Refresh(IProject project);
        Task<bool> Load(string location, bool UpdateCurrent = true);
        Task<bool> Save(string location = null);
        Task<bool> Save(IProject project, string location);
        Task<bool> Close();
        Task<bool> Close(IProject project);
        Task<bool> SetCurrentProject(IProject project);
        #endregion
    }
}