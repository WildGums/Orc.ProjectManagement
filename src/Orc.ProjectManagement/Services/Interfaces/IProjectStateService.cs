namespace Orc.ProjectManagement;

using System;

public interface IProjectStateService
{
    bool IsRefreshingActiveProject { get; }

    event EventHandler<EventArgs>? IsRefreshingActiveProjectUpdated;
    event EventHandler<ProjectStateEventArgs>? ProjectStateUpdated;

    ProjectState GetProjectState(IProject project);
}