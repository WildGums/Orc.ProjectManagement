namespace Orc.ProjectManagement;

using System;

public static class IProjectManagerExtensions
{
    public static TProject? GetActiveProject<TProject>(this IProjectManager projectManager)
        where TProject : IProject
    {
        ArgumentNullException.ThrowIfNull(projectManager);

        return (TProject?)projectManager.ActiveProject;
    }

    public static string? GetActiveProjectLocation(this IProjectManager projectManager)
    {
        ArgumentNullException.ThrowIfNull(projectManager);

        var activeProject = projectManager.ActiveProject;
        return activeProject?.Location;
    }
}
