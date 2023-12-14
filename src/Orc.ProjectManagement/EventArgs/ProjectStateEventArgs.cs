namespace Orc.ProjectManagement;

using System;

public class ProjectStateEventArgs : EventArgs
{
    public ProjectStateEventArgs(ProjectState projectState)
    {
        ArgumentNullException.ThrowIfNull(projectState);

        ProjectState = projectState;
    }

    public ProjectState ProjectState { get; }
}
