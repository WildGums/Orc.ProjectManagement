﻿namespace Orc.ProjectManagement;

using System;
using System.Collections.Generic;

public class ProjectStateService : IProjectStateService, IProjectStateSetter
{
    private static readonly ProjectState DefaultProjectState = new();

    private readonly Dictionary<string, ProjectState> _projectStates = new();

    private bool _isRefreshingActiveProject;

    public bool IsRefreshingActiveProject
    {
        get { return _isRefreshingActiveProject; }
        private set
        {
            if (_isRefreshingActiveProject != value)
            {
                _isRefreshingActiveProject = value;
                IsRefreshingActiveProjectUpdated?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public event EventHandler<EventArgs>? IsRefreshingActiveProjectUpdated;

    public event EventHandler<ProjectStateEventArgs>? ProjectStateUpdated;

    public ProjectState GetProjectState(IProject project)
    {
        ArgumentNullException.ThrowIfNull(project);

        return GetProjectState(project.Location);
    }

    public void SetProjectLoading(string location, bool value)
    {
        if (string.IsNullOrEmpty(location))
        {
            return;
        }

        UpdateProjectState(location, state => state.IsLoading = value);
    }

    public void SetProjectSaving(string location, bool value)
    {
        if (string.IsNullOrEmpty(location))
        {
            return;
        }

        UpdateProjectState(location, state => state.IsSaving = value);
    }

    public void SetProjectClosing(string location, bool value)
    {
        if (string.IsNullOrEmpty(location))
        {
            return;
        }

        UpdateProjectState(location, state => state.IsClosing = value);
    }

    public void SetProjectActivating(string? location, bool value)
    {
        if (string.IsNullOrEmpty(location))
        {
            return;
        }

        if (GetProjectState(location).IsRefreshing)
        {
            IsRefreshingActiveProject = value;
        }

        UpdateProjectState(location, state => state.IsActivating = value);
    }

    public void SetProjectDeactivating(string? location, bool value)
    {
        if (string.IsNullOrEmpty(location))
        {
            return;
        }

        UpdateProjectState(location, state => state.IsDeactivating = value);
    }

    public void SetProjectRefreshing(string location, bool value, bool isActiveProject = true)
    {
        if (string.IsNullOrEmpty(location))
        {
            return;
        }

        if (isActiveProject)
        {
            IsRefreshingActiveProject = value;
        }

        UpdateProjectState(location, state => state.IsRefreshing = value);
    }

    private ProjectState GetProjectState(string location)
    {
        lock (_projectStates)
        {
            if (!_projectStates.TryGetValue(location, out var projectState))
            {
                projectState = DefaultProjectState;
                // Do not add into the dictionary here
            }

            return projectState;
        }
    }

    private void UpdateProjectState(string location, Action<ProjectState> updateAction)
    {
        if (string.IsNullOrEmpty(location))
        {
            return;
        }

        ProjectState? projectState;

        lock (_projectStates)
        {
            if (!_projectStates.TryGetValue(location, out projectState))
            {
                projectState = new ProjectState
                {
                    Location = location
                };

                _projectStates[location] = projectState;
            }

            updateAction(projectState);
        }

        ProjectStateUpdated?.Invoke(this, new ProjectStateEventArgs(new ProjectState(projectState)));
    }
}
