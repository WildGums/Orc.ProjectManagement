// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionSequences.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement.Tests
{
    using System;
    using Moq;

    internal class ActionSequences
    {
        public const string ProjectManagerLoad = "IProjectManager.Load";
        public const string ProjectManagerSetActiveProject = "IProjectManager.SetActiveProject";
        public const string ProjectManagerClose = "IProjectManager.Close";
        public const string ProjectManagerRefresh = "IProjectManager.Refresh";
        public const string ProjectManagerProjectLoading = "IProjectManager.ProjectLoading";
        public const string ProjectManagerProjectLoaded = "IProjectManager.ProjectLoaded";
        public const string ProjectManagerProjectActivated = "IProjectManager.ProjectActivated";
        public const string ProjectManagerProjectClosing = "IProjectManager.ProjectClosing";
        public const string ProjectManagerProjectClosed = "IProjectManager.ProjectClosed";
        public const string ProjectManagerProjectUpdated = "IProjectManager.ProjectUpdated";

        public static void SetupProjectManagerSequences(Mock<ProjectManager> mockOfProjectManager, Action<string, object[]> callbackAction)
        {
            mockOfProjectManager.Setup(pm => pm.Load(It.IsAny<string>())).CallBase().
                Callback<string>(location => callbackAction(ProjectManagerLoad, new object[] {location}));

            mockOfProjectManager.Setup(pm => pm.Load(It.IsAny<string>(), It.IsAny<bool>())).CallBase().
                Callback<string, bool>((location, updateActive) => callbackAction(ProjectManagerLoad, new object[] {location, updateActive}));

            mockOfProjectManager.Setup(pm => pm.Load(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).CallBase().
                Callback<string, bool, bool>((location, updateActive, activateLoaded) => callbackAction(ProjectManagerLoad, new object[] {location, updateActive, activateLoaded}));

            mockOfProjectManager.Setup(pm => pm.SetActiveProject(It.IsAny<IProject>())).CallBase().
                Callback<IProject>(project => callbackAction(ProjectManagerSetActiveProject, new object[] {project}));

            mockOfProjectManager.Setup(pm => pm.Close(It.IsAny<IProject>())).CallBase().
                Callback<IProject>(project => callbackAction(ProjectManagerClose, new object[] { project }));

            mockOfProjectManager.Setup(pm => pm.Refresh(It.IsAny<IProject>())).CallBase().
                Callback<IProject>(project => callbackAction(ProjectManagerRefresh, new object[] { project }));

            var projectManager = mockOfProjectManager.Object;

            projectManager.ProjectLoading += async (sender, args) => callbackAction(ProjectManagerProjectLoading, new[] { sender, args });

            projectManager.ProjectLoaded += async (sender, args) => callbackAction(ProjectManagerProjectLoaded, new[] { sender, args });

            projectManager.ProjectActivated += async (sender, args) => callbackAction(ProjectManagerProjectActivated, new[] { sender, args });

            projectManager.ProjectClosing += async (sender, args) => callbackAction(ProjectManagerProjectClosing, new[] { sender, args });

            projectManager.ProjectClosed += async (sender, args) => callbackAction(ProjectManagerProjectClosed, new[] { sender, args });

            projectManager.ProjectUpdated += async (sender, args) => callbackAction(ProjectManagerProjectUpdated, new[] { sender, args });
        }
    }
}