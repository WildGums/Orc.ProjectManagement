namespace Orc.ProjectManagement;

using System;

public class ProjectState
{
    public ProjectState()
    {
        Location = string.Empty;
    }

    public ProjectState(ProjectState projectState)
    {
        ArgumentNullException.ThrowIfNull(projectState);

        Location = projectState.Location;
        IsLoading = projectState.IsLoading;
        IsSaving = projectState.IsSaving;
        IsRefreshing = projectState.IsRefreshing;
        IsDeactivating = projectState.IsDeactivating;
        IsActivating = projectState.IsActivating;
        IsClosing = projectState.IsClosing;
    }

    public string Location { get; set; }

    public bool IsLoading { get; set; }

    public bool IsSaving { get; set; }

    public bool IsRefreshing { get; set; }

    public bool IsDeactivating { get; set; }

    public bool IsActivating { get; set; }

    public bool IsClosing { get; set; }
}