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
        [ObsoleteEx(ReplacementTypeOrMember = "SelectedProject", RemoveInVersion = "1.1.0", TreatAsErrorFromVersion = "1.0.0")]
        IProject Project { get; }

        IEnumerable<IProject> Projects { get; }

        [ObsoleteEx(ReplacementTypeOrMember = "SelectedProject.Location", RemoveInVersion = "1.1.0", TreatAsErrorFromVersion = "1.0.0")]
        string Location { get; }

        IProject SelectedProject { get; }
        #endregion

        #region Methods
        event AsyncEventHandler<ProjectLocationCancelEventArgs> ProjectLoading;
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
        event AsyncEventHandler<ProjectCancelEventArgs> ProjectSelecting;
        event AsyncEventHandler<ProjectEventArgs> ProjectSelected;
        event AsyncEventHandler<ProjectEventArgs> ProjectSelectionCanceled;
        event AsyncEventHandler<ProjectErrorEventArgs> ProjectSelectionFailed;
        Task Initialize();
        Task Refresh();
        Task Refresh(IProject project);
        Task<bool> Load(string location, bool selectLoaded = true);
        Task<bool> Save(string location = null);
        Task<bool> Save(IProject project, string location);
        Task<bool> Close();
        Task<bool> Close(IProject project);
        Task<bool> SelectProject(IProject project, bool rememberPrevious = true);
        #endregion
    }
}