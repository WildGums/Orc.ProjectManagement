// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Listener.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement.Tests
{
    using System;
    using Moq;

    internal class Listener
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

        public static void ListenToProjectManager(Factory factory, Action<string, object[]> callbackAction)
        {
            var mockOfProjectManager = factory.ServiceLocator.ResolveMocked<IProjectManager>();

            mockOfProjectManager.Setup(pm => pm.Load(It.IsAny<string>())).CallBase().
                Callback<string>(location => callbackAction(ProjectManagerLoad, new object[] {location}));

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
        }
    }
}