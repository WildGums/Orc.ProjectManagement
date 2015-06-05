// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectManagerFacts.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement.Test.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Catel.IoC;
    using Mocks;
    using Moq;
    using NUnit.Framework;
    using Tests;

    public class ProjectManagerFacts
    {
        [TestFixture]
        public class TheLoadMethod
        {
            [TestCase("myLocation")]
            public async Task UpdatesLocationAfterLoadingProject(string newLocation)
            {
                var factory = Factory.Create().SetupDefault();
                var projectManager = factory.GetProjectManager();

                Assert.AreEqual(null, projectManager.ActiveProject);

                await projectManager.Load(newLocation);

                Assert.AreEqual(newLocation, projectManager.ActiveProject.Location);
            }

            [TestCase]
            public async Task RaisesProjectLoadingEvent()
            {
                var factory = Factory.Create().SetupDefault();
                var projectManager = factory.GetProjectManager();

                var eventRaised = false;
                projectManager.ProjectLoading += async (sender, e) => eventRaised = true;

                await projectManager.Load("dummyLocation");

                Assert.IsTrue(eventRaised);
            }

            [TestCase]
            public async Task RaisesProjectLocationLoadingFailedEvent()
            {
                var factory = Factory.Create().SetupDefault();
                var projectManager = factory.GetProjectManager();

                var eventRaised = false;
                projectManager.ProjectLoadingFailed += async (sender, e) => eventRaised = true;

                await projectManager.Load("cannotload");

                Assert.IsTrue(eventRaised);
            }

            [TestCase]
            public async Task RaisesProjectLocationLoadingCanceledEvent()
            {
                var factory = Factory.Create().SetupDefault();
                var projectManager = factory.GetProjectManager();

                var eventRaised = false;
                projectManager.ProjectLoading += async (sender, e) => e.Cancel = true;
                projectManager.ProjectLoadingCanceled += async (sender, e) => eventRaised = true;

                await projectManager.Load("dummyLocation");

                Assert.IsTrue(eventRaised);
            }

            [TestCase]
            public async Task RaisesProjectLoadedEvent()
            {
                var factory = Factory.Create().SetupDefault();
                var projectManager = factory.GetProjectManager();

                var eventRaised = false;
                projectManager.ProjectLoaded += async (sender, e) => eventRaised = true;

                await projectManager.Load("dummyLocation");

                Assert.IsTrue(eventRaised);
            }
        }
/*
        [TestFixture]
        public class TheLoadMethodWithParameterUpdateActive
        {
            [TestCase]
            public async Task CloseActiveProjectWhileLoadWhenUpdateActiveTrue()
            {
                var factory = Factory.Create().SetupDefault();
                var mock = factory.Mock<ProjectManager>();

                var activeProject = new Project("activeProjectLocation");
                mock.Setup(pm => pm.ActiveProject).Returns(activeProject);

                await mock.Object.Load("newProjectLocation", true);

                mock.Verify(pm => pm.Close(activeProject), Times.Once);
            }

            [TestCase]
            public async Task CloseActiveProjectBeforeLoadedWhenUpdateActiveTrue()
            {
                var factory = Factory.Create().SetupDefault();
                var mock = factory.Mock<ProjectManager>();

                var activeProject = new Project("activeProjectLocation");
                mock.Setup(pm => pm.ActiveProject).Returns(activeProject);

                var actionsSequence = new List<Tuple<string, object[]>>();
                Listener.ListenToProjectManager(mock, (s, objects) => actionsSequence.Add(new Tuple<string, object[]>(s, objects)));

                await mock.Object.Load("newProjectLocation", true);

                var indexOfClose = actionsSequence.IndexOf(actionsSequence.FirstOrDefault(x => string.Equals(x.Item1, Listener.ProjectManagerClose)));
                var indexOfLoaded = actionsSequence.IndexOf(actionsSequence.FirstOrDefault(x => string.Equals(x.Item1, Listener.ProjectManagerProjectLoaded)));
                Assert.Greater(indexOfLoaded, indexOfClose);
            }

            [TestCase]
            public async Task DontCloseAnyProjectsWhileLoadWhenUpdateActiveFalse()
            {
                var factory = Factory.Create().SetupDefault();
                var mock = factory.Mock<ProjectManager>();

                var activeProject = new Project("activeProjectLocation");

                mock.Setup(pm => pm.ActiveProject).Returns(activeProject);

                await mock.Object.Load("newProjectLocation", false);

                mock.Verify(pm => pm.Close(It.IsAny<IProject>()), Times.Never);
            }

            [TestCase]
            public async Task DontCloseAnyProjectsWhileLoadWhenUpdateActiveTrueAndActiveProjectNull()
            {
                var factory = Factory.Create().SetupDefault();
                var mock = factory.Mock<ProjectManager>();

                mock.Setup(pm => pm.ActiveProject).Returns((IProject) null);

                await mock.Object.Load("newProjectLocation", true);

                mock.Verify(pm => pm.Close(It.IsAny<IProject>()), Times.Never);
            }

            [TestCase]
            public async Task ActivateLoadedProjectWhenUpdateActiveTrue()
            {
                var factory = Factory.Create().SetupDefault();
                var mock = factory.Mock<ProjectManager>();

                var newProjectlocation = "newProjectLocation";

                await mock.Object.Load(newProjectlocation, true);

                mock.Verify(pm => pm.SetActiveProject(It.Is<IProject>(x => string.Equals(newProjectlocation, x.Location))), Times.Once);
            }

            [TestCase]
            public async Task DontActivateLoadedProjectWhenUpdateActiveTrue()
            {
                var factory = Factory.Create().SetupDefault();
                var mock = factory.Mock<ProjectManager>();

                var newProjectlocation = "newProjectLocation";

                await mock.Object.Load(newProjectlocation, true);

                mock.Verify(pm => pm.SetActiveProject(It.Is<IProject>(x => string.Equals(newProjectlocation, x.Location))), Times.Once);
            }
        }
        */
        [TestFixture]
        public class TheRefreshMethod
        {
            [TestCase]
            public async Task DoesNothingWithoutProject()
            {
                var factory = Factory.Create().SetupDefault();
                var projectManager = factory.GetProjectManager();

                Assert.IsNull(projectManager.ActiveProject);

                await projectManager.Refresh();

                Assert.IsNull(projectManager.ActiveProject);
            }

            [TestCase]
            public async Task RaisesProjectActivatedEvent()
            {
                var factory = Factory.Create().SetupDefault();
                var projectManager = factory.GetProjectManager();

                await projectManager.Load("dummyLocation");

                var eventRaised = false;
                projectManager.ProjectActivated += async (sender, e) => eventRaised = true;

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
                var factory = Factory.Create().SetupDefault();
                var projectManager = factory.GetProjectManager();

                await projectManager.Load("dummyLocation");

                Assert.AreEqual("dummyLocation", projectManager.ActiveProject.Location);

                await projectManager.Save(newLocation);

                Assert.AreEqual(newLocation, projectManager.ActiveProject.Location);
            }

            [TestCase]
            public async Task RaisesProjectSavingEvent()
            {
                var factory = Factory.Create().SetupDefault();
                var projectManager = factory.GetProjectManager();

                await projectManager.Load("dummyLocation");

                var eventRaised = false;
                projectManager.ProjectSaving += async (sender, e) => eventRaised = true;

                await projectManager.Save();

                Assert.IsTrue(eventRaised);
            }

            [TestCase]
            public async Task RaisesProjectSavedEvent()
            {
                var factory = Factory.Create().SetupDefault();
                var projectManager = factory.GetProjectManager();

                await projectManager.Load("dummyLocation");

                var eventRaised = false;
                projectManager.ProjectSaved += async (sender, e) => eventRaised = true;

                await projectManager.Save();

                Assert.IsTrue(eventRaised);
            }

            [TestCase(5)]
            public async Task DoesntRaiseProjectRefreshRequiredInParallelSave(int threadsCount)
            {
                var factory = Factory.Create().SetupDefault();

                var mockOfProjectManager = factory.Mock<ProjectManager>();

                var projectManager = mockOfProjectManager.Object;

                for (var i = 0; i < threadsCount; i++)
                {
                    await projectManager.Load(string.Format("project{0}", i));
                }

                var raisedProjectRefreshRequired = false;
                projectManager.ProjectRefreshRequired += (sender, args) => raisedProjectRefreshRequired = true;


                // Run test
                var tasks = projectManager.Projects.Select(proj => Task.Factory.StartNew(async () => await projectManager.Save(proj))).Cast<Task>().ToArray();
                Task.WaitAll(tasks);



                Assert.IsFalse(raisedProjectRefreshRequired);
            }
        }

        [TestFixture]
        public class TheCloseMethod
        {
            [TestCase]
            public async Task UpdatesProjectAfterClosingProject()
            {
                var factory = Factory.Create().SetupDefault();
                var projectManager = factory.GetProjectManager();

                await projectManager.Load("dummyLocation");

                Assert.IsNotNull(projectManager.ActiveProject);

                await projectManager.Close();

                Assert.IsNull(projectManager.ActiveProject);
            }

            [TestCase]
            public async Task UpdatesLocationAfterClosingProject()
            {
                var factory = Factory.Create().SetupDefault();
                var projectManager = factory.GetProjectManager();

                await projectManager.Load("dummyLocation");

                Assert.AreEqual("dummyLocation", projectManager.ActiveProject.Location);

                await projectManager.Close();

                Assert.AreEqual(null, projectManager.ActiveProject);
            }

            [TestCase]
            public async Task RaisesProjectClosingEvent()
            {
                var factory = Factory.Create().SetupDefault();
                var projectManager = factory.GetProjectManager();

                await projectManager.Load("dummyLocation");

                var eventRaised = false;
                projectManager.ProjectClosing += async (sender, e) => eventRaised = true;

                await projectManager.Close();

                Assert.IsTrue(eventRaised);
            }

            [TestCase]
            public async Task RaisesProjectClosedEvent()
            {
                var factory = Factory.Create().SetupDefault();
                var projectManager = factory.GetProjectManager();

                await projectManager.Load("dummyLocation");

                var eventRaised = false;
                projectManager.ProjectClosed += async (sender, e) => eventRaised = true;

                await projectManager.Close();

                Assert.IsTrue(eventRaised);
            }
        }
    }
}