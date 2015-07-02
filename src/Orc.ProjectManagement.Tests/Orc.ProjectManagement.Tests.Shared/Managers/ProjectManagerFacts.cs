// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectManagerFacts.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement.Test.Managers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using Moq;
    using NUnit.Framework;
    using Tests;

    public class ProjectManagerFacts
    {
        [TestFixture]
        public class TheLoadMethod
        {
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
            public async Task RaisesProjectLoadingFailedEvent()
            {
                var factory = Factory.Create().SetupDefault();
                var projectManager = factory.GetProjectManager();

                var eventRaised = false;
                projectManager.ProjectLoadingFailed += async (sender, e) => eventRaised = true;

                await projectManager.Load("cannotload");

                Assert.IsTrue(eventRaised);
            }

            [TestCase]
            public async Task RaisesProjectLoadingCanceledEvent()
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
            public async Task DoesntRaiseProjectLoadedEventIfCanceled()
            {
                var factory = Factory.Create().SetupDefault();
                var projectManager = factory.GetProjectManager();

                var eventRaised = false;
                projectManager.ProjectLoading += async (sender, e) => e.Cancel = true;
                projectManager.ProjectLoaded += async (sender, e) => eventRaised = true;

                await projectManager.Load("dummyLocation");

                Assert.IsFalse(eventRaised);
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

            [TestCase]
            public async Task RaisesProjectLoadedEventBeforeCallsSetActiveMethod()
            {
                var factory = Factory.Create().SetupDefault();
                var mock = factory.MockProjectManager();
                var projectManager = mock.Object;

                IList<string> actionNames = new List<string>();

                Listener.ListenToProjectManager(factory, (name, args) => actionNames.Add(name));

                await projectManager.Load("dummyLocation");

                var projectLoadedIndex = actionNames.Single(x => string.Equals(x, Listener.ProjectManagerProjectLoaded));
                var setAciveIndex = actionNames.Single(x => string.Equals(x, Listener.ProjectManagerSetActiveProject));

                Assert.Less(projectLoadedIndex, setAciveIndex);
            }

            [TestCase]
            public async Task CallsSetActiveMethodWithLoadedProjectInParameter()
            {
                var factory = Factory.Create().SetupDefault();
                var mock = factory.MockProjectManager();
                var projectManager = mock.Object;

                IProject loadedProject = null;
                projectManager.ProjectLoaded += async (sender, e) => loadedProject = e.Project;

                await projectManager.Load("dummyLocation");

                mock.Verify(x => x.SetActiveProject(loadedProject), Times.Once);
            }
        }

        [TestFixture]
        public class TheLoadInactiveMethod
        {
            [TestCase]
            public async Task RaisesProjectLoadingEvent()
            {
                var factory = Factory.Create().SetupDefault();
                var projectManager = factory.GetProjectManager();

                var eventRaised = false;
                projectManager.ProjectLoading += async (sender, e) => eventRaised = true;

                await projectManager.LoadInactive("dummyLocation");

                Assert.IsTrue(eventRaised);
            }

            [TestCase]
            public async Task RaisesProjectLoadingFailedEvent()
            {
                var factory = Factory.Create().SetupDefault();
                var projectManager = factory.GetProjectManager();

                var eventRaised = false;
                projectManager.ProjectLoadingFailed += async (sender, e) => eventRaised = true;

                await projectManager.LoadInactive("cannotload");

                Assert.IsTrue(eventRaised);
            }

            [TestCase]
            public async Task RaisesProjectLoadingCanceledEvent()
            {
                var factory = Factory.Create().SetupDefault();
                var projectManager = factory.GetProjectManager();

                var eventRaised = false;
                projectManager.ProjectLoading += async (sender, e) => e.Cancel = true;
                projectManager.ProjectLoadingCanceled += async (sender, e) => eventRaised = true;

                await projectManager.LoadInactive("dummyLocation");

                Assert.IsTrue(eventRaised);
            }

            [TestCase]
            public async Task DoesntRaiseProjectLoadedEventIfCanceled()
            {
                var factory = Factory.Create().SetupDefault();
                var projectManager = factory.GetProjectManager();

                var eventRaised = false;
                projectManager.ProjectLoading += async (sender, e) => e.Cancel = true;
                projectManager.ProjectLoaded += async (sender, e) => eventRaised = true;

                await projectManager.LoadInactive("dummyLocation");

                Assert.IsFalse(eventRaised);
            }

            [TestCase]
            public async Task RaisesProjectLoadedEvent()
            {
                var factory = Factory.Create().SetupDefault();
                var projectManager = factory.GetProjectManager();

                var eventRaised = false;
                projectManager.ProjectLoaded += async (sender, e) => eventRaised = true;

                await projectManager.LoadInactive("dummyLocation");

                Assert.IsTrue(eventRaised);
            }

            [TestCase]
            public async Task DoesntCallSetActiveMethodWithLoadedProjectInParameter()
            {
                var factory = Factory.Create().SetupDefault();
                var mock = factory.MockProjectManager();
                var projectManager = mock.Object;

                await projectManager.LoadInactive("dummyLocation");

                mock.Verify(x => x.SetActiveProject(It.IsAny<IProject>()), Times.Never);
            }
        }

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

            [TestCase(Reason = "ORCOMP-147")]
            public async Task DoesNotReloadClosedProjectAgain()
            {
                var factory = Factory.Create().SetupDefault();
                var projectManager = factory.GetProjectManager();

                await projectManager.Load("dummyLocation");

                var eventCount = 0;
                var projects = new List<IProject>();
                projectManager.ProjectActivated += async (sender, e) =>
                {
                    eventCount++;
                    projects.Add(e.NewProject);
                };

                await projectManager.Close();

                Assert.AreEqual(1, eventCount);
                Assert.AreEqual(1, projects.Count);
                Assert.IsNull(projects[0]);
            }
        }

        [TestFixture]
        public class TheSetActiveProjectMethod
        {
            [TestCase("dummyLocation")]
            public async Task UpdatesActiveProjectByValueFromParameter(string newLocation)
            {
                var factory = Factory.Create().SetupDefault();
                var projectManager = factory.GetProjectManager();

                var initialActiveProject = projectManager.ActiveProject;
                var newProject = factory.CreateProject(newLocation);

                await projectManager.SetActiveProject(newProject);

                var activeProject = projectManager.ActiveProject;

                Assert.AreNotEqual(initialActiveProject, activeProject);
                Assert.AreEqual(activeProject, newProject);
            }

            [TestCase("dummyLocation")]
            public async Task RaiseProjectActivationEvent(string newLocation)
            {
                var factory = Factory.Create().SetupDefault();
                var projectManager = factory.GetProjectManager();

                IProject projectFromEvent = null;

                projectManager.ProjectActivation += async (sender, e) => projectFromEvent = e.NewProject;
                var newProject = factory.CreateProject(newLocation);

                await projectManager.SetActiveProject(newProject);

                Assert.AreEqual(newProject, projectFromEvent);
            }

            [TestCase("dummyLocation")]
            public async Task RaiseProjectActivatedEvent(string newLocation)
            {
                var factory = Factory.Create().SetupDefault();
                var projectManager = factory.GetProjectManager();

                IProject projectFromEvent = null;

                projectManager.ProjectActivated += async (sender, e) => projectFromEvent = e.NewProject;
                var newProject = factory.CreateProject(newLocation);

                await projectManager.SetActiveProject(newProject);

                Assert.AreEqual(newProject, projectFromEvent);
            }

            [TestCase("dummyLocation")]
            public async Task RaiseProjectActivatedEventAfterSettingActiveProject(string newLocation)
            {
                var factory = Factory.Create().SetupDefault();
                var mock = factory.MockProjectManager();
                var projectManager = mock.Object;

                IList<string> actionNames = new List<string>();

                Listener.ListenToProjectManager(factory, (name, args) => actionNames.Add(name));

                var newProject = factory.CreateProject(newLocation);

                await projectManager.SetActiveProject(newProject);

                var projectActivatedIndex = actionNames.Single(x => string.Equals(x, Listener.ProjectManagerProjectActivated));
                var setAciveIndex = actionNames.Single(x => string.Equals(x, Listener.ProjectManagerActiveProjectSet));

                Assert.Less(setAciveIndex, projectActivatedIndex);
            }
        }
    }
}