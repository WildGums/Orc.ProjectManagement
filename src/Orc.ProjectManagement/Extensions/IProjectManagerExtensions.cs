namespace Orc.ProjectManagement;

using System;
using System.Threading.Tasks;

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

    public static Task<bool> CloseActiveProjectAsync(this IProjectManager projectManager)
    {
        var project = projectManager.ActiveProject;

        return project is null
            ? Task.FromResult(false)
            : projectManager.CloseAsync(project);
    }

    public static Task<bool> RefreshActiveProjectAsync(this IProjectManager projectManager)
    {
        var project = projectManager.ActiveProject;

        return project is null
            ? Task.FromResult(false)
            : projectManager.RefreshAsync(project);
    }

    public static Task<bool> SaveActiveProjectAsync(this IProjectManager projectManager, string? location = null)
    {
        var project = projectManager.ActiveProject;
        if (project is null)
        {
            return Task.FromResult(false);
        }

        return projectManager.SaveAsync(project, location);
    }
}
