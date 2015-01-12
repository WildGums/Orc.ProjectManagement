// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectManagerFacts.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement.Test.Managers
{
    using System.Threading.Tasks;
    using Catel.IoC;
    using Mocks;
    using NUnit.Framework;
    using ProjectManagement.Serialization;

    public class ProjectManagerFacts
    {
        private static ProjectManager GetProjectManager()
        {
            var projectManager = new ProjectManager(new EmptyProjectValidator(), new EmptyProjectInitializer(), 
                new DefaultProjectRefresherSelector(ServiceLocator.Default, TypeFactory.Default), 
                new FixedProjectSerializerSelector<MemoryProjectReader, MemoryProjectWriter>());

            return projectManager;
        }

        [TestFixture]
        public class TheLoadMethod
        {
            [TestCase("myLocation")]
            public async Task UpdatesLocationAfterLoadingProject(string newLocation)
            {
                var projectManager = GetProjectManager();

                Assert.AreEqual(null, projectManager.Location);

                await projectManager.Load(newLocation);

                Assert.AreEqual(newLocation, projectManager.Location);
            }

            [TestCase]
            public async Task RaisesProjectLoadingEvent()
            {
                var projectManager = GetProjectManager();

                var eventRaised = false;
                projectManager.ProjectLoading += (sender, e) => eventRaised = true;

                await projectManager.Load("dummyLocation");

                Assert.IsTrue(eventRaised);
            }

            [TestCase]
            public async Task RaisesProjectLoadingFailedEvent()
            {
                var projectManager = GetProjectManager();

                var eventRaised = false;
                projectManager.ProjectLoadingFailed += (sender, e) => eventRaised = true;

                await projectManager.Load("cannotload");

                Assert.IsTrue(eventRaised);
            }

            [TestCase]
            public async Task RaisesProjectLoadedEvent()
            {
                var projectManager = GetProjectManager();

                var eventRaised = false;
                projectManager.ProjectLoaded += (sender, e) => eventRaised = true;

                await projectManager.Load("dummyLocation");

                Assert.IsTrue(eventRaised);
            }
        }

        [TestFixture]
        public class TheRefreshMethod
        {
            [TestCase]
            public async Task DoesNothingWithoutProject()
            {
                var projectManager = GetProjectManager();

                Assert.IsNull(projectManager.Project);

                await projectManager.Refresh();

                Assert.IsNull(projectManager.Project);
            }

            [TestCase]
            public async Task RaisesProjectUpdatedEvent()
            {
                var projectManager = GetProjectManager();

                await projectManager.Load("dummyLocation");

                var eventRaised = false;
                projectManager.ProjectUpdated += (sender, e) => eventRaised = true;

                await projectManager.Refresh();

                Assert.IsTrue(eventRaised);
            }
        }

        [TestFixture]
        public class TheSaveMethod
        {
            [TestCase("myLocation")]
            public async Task UpdatesLocationAfterSavingProject(string newLocation)
            {
                var projectManager = GetProjectManager();

                await projectManager.Load("dummyLocation");

                Assert.AreEqual("dummyLocation", projectManager.Location);

                await projectManager.Save(newLocation);

                Assert.AreEqual(newLocation, projectManager.Location);
            }

            [TestCase]
            public async Task RaisesProjectSavingEvent()
            {
                var projectManager = GetProjectManager();

                await projectManager.Load("dummyLocation");

                var eventRaised = false;
                projectManager.ProjectSaving += (sender, e) => eventRaised = true;

                await projectManager.Save();

                Assert.IsTrue(eventRaised);
            }

            [TestCase]
            public async Task RaisesProjectSavedEvent()
            {
                var projectManager = GetProjectManager();

                await projectManager.Load("dummyLocation");

                var eventRaised = false;
                projectManager.ProjectSaved += (sender, e) => eventRaised = true;

                await projectManager.Save();

                Assert.IsTrue(eventRaised);
            }
        }

        [TestFixture]
        public class TheCloseMethod
        {
            [TestCase]
            public async Task UpdatesProjectAfterClosingProject()
            {
                var projectManager = GetProjectManager();

                await projectManager.Load("dummyLocation");

                Assert.IsNotNull(projectManager.Project);

                projectManager.Close();

                Assert.IsNull(projectManager.Project);
            }

            [TestCase]
            public async Task UpdatesLocationAfterClosingProject()
            {
                var projectManager = GetProjectManager();

                await projectManager.Load("dummyLocation");

                Assert.AreEqual("dummyLocation", projectManager.Location);

                projectManager.Close();

                Assert.AreEqual(null, projectManager.Location);
            }

            [TestCase]
            public async Task RaisesProjectClosingEvent()
            {
                var projectManager = GetProjectManager();

                await projectManager.Load("dummyLocation");

                var eventRaised = false;
                projectManager.ProjectClosing += (sender, e) => eventRaised = true;

                projectManager.Close();

                Assert.IsTrue(eventRaised);
            }

            [TestCase]
            public async Task RaisesProjectClosedEvent()
            {
                var projectManager = GetProjectManager();

                await projectManager.Load("dummyLocation");

                var eventRaised = false;
                projectManager.ProjectClosed += (sender, e) => eventRaised = true;

                projectManager.Close();

                Assert.IsTrue(eventRaised);
            }
        }
    }
}