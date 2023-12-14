namespace Orc.ProjectManagement.Tests;

using System;
using Moq;

internal class Listener
{
    public const string ProjectManagerLoad = "IProjectManager.Load";
    public const string ProjectManagerSetActiveProject = "IProjectManager.SetActiveProject";
    public const string ProjectManagerActiveProjectSet = "IProjectManager.ActiveProject.Set";
    public const string ProjectManagerClose = "IProjectManager.Close";
    public const string ProjectManagerRefresh = "IProjectManager.Refresh";
    public const string ProjectManagerProjectLoading = "IProjectManager.ProjectLoading";
    public const string ProjectManagerProjectLoaded = "IProjectManager.ProjectLoaded";
    public const string ProjectManagerProjectActivated = "IProjectManager.ProjectActivated";
    public const string ProjectManagerProjectClosing = "IProjectManager.ProjectClosing";
    public const string ProjectManagerProjectClosed = "IProjectManager.ProjectClosed";

    public static void ListenToProjectManager(Factory factory, Action<string, object[]> callbackAction)
    {
        var mockOfProjectManager = factory.ServiceLocator.ResolveMocked<ProjectManager>();

        mockOfProjectManager.Setup(pm => pm.LoadAsync(It.IsAny<string>())).CallBase().
            Callback<string>(location => callbackAction(ProjectManagerLoad, new object[] {location}));

        mockOfProjectManager.Setup(pm => pm.SetActiveProjectAsync(It.IsAny<IProject>())).CallBase().
            Callback<IProject>(project => callbackAction(ProjectManagerSetActiveProject, new object[] {project}));

        mockOfProjectManager.Setup(pm => pm.CloseAsync(It.IsAny<IProject>())).CallBase().
            Callback<IProject>(project => callbackAction(ProjectManagerClose, new object[] { project }));

        mockOfProjectManager.Setup(pm => pm.RefreshAsync(It.IsAny<IProject>())).CallBase().
            Callback<IProject>(project => callbackAction(ProjectManagerRefresh, new object[] { project }));

        mockOfProjectManager.SetupSet(pm => pm.ActiveProject = It.IsAny<IProject>()).
            Callback<IProject>(project => callbackAction(ProjectManagerActiveProjectSet, new object[] { project }));

        var projectManager = mockOfProjectManager.Object;

        projectManager.ProjectLoadingAsync += async (sender, args) => callbackAction(ProjectManagerProjectLoading, new[] { sender, args });

        projectManager.ProjectLoadedAsync += async (sender, args) => callbackAction(ProjectManagerProjectLoaded, new[] { sender, args });

        projectManager.ProjectActivatedAsync += async (sender, args) => callbackAction(ProjectManagerProjectActivated, new[] { sender, args });

        projectManager.ProjectClosingAsync += async (sender, args) => callbackAction(ProjectManagerProjectClosing, new[] { sender, args });

        projectManager.ProjectClosedAsync += async (sender, args) => callbackAction(ProjectManagerProjectClosed, new[] { sender, args });
    }
}
