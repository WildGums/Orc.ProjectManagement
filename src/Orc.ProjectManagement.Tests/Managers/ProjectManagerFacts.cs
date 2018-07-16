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
                projectManager.ProjectLoadingAsync += async (sender, e) => eventRaised = true;

                await projectManager.LoadAsync("dummyLocation");

                Assert.IsTrue(eventRaised);
            }

            [TestCase]
            public async Task RaisesProjectLoadingFailedEvent()
            {
                var factory = Factory.Create().SetupDefault();
                var projectManager = factory.GetProjectManager();

                var eventRaised = false;
                projectManager.ProjectLoadingFailedAsync += async (sender, e) => eventRaised = true;

                await projectManager.LoadAsync("cannotload");

                Assert.IsTrue(eventRaised);
            }

            [TestCase]
            public async Task RaisesProjectLoadingCanceledEvent()
            {
                var factory = Factory.Create().SetupDefault();
                var projectManager = factory.GetProjectManager();

                var eventRaised = false;
                projectManager.ProjectLoadingAsync += async (sender, e) => e.Cancel = true;
                projectManager.ProjectLoadingCanceledAsync += async (sender, e) => eventRaised = true;

                await projectManager.LoadAsync("dummyLocation");

                Assert.IsTrue(eventRaised);
            }

            [TestCase]
            public async Task DoesntRaiseProjectLoadedEventIfCanceled()
            {
                var factory = Factory.Create().SetupDefault();
                var projectManager = factory.GetProjectManager();

                var eventRaised = false;
                projectManager.ProjectLoadingAsync += async (sender, e) => e.Cancel = true;
                projectManager.ProjectLoadedAsync += async (sender, e) => eventRaised = true;

                await projectManager.LoadAsync("dummyLocation");

                Assert.IsFalse(eventRaised);
            }

            [TestCase]
            public async Task RaisesProjectLoadedEvent()
            {
                var factory = Factory.Create().SetupDefault();
                var projectManager = factory.GetProjectManager();

                var eventRaised = false;
                projectManager.ProjectLoadedAsync += async (sender, e) => eventRaised = true;

                await projectManager.LoadAsync("dummyLocation");

                Assert.IsTrue(eventRaised);
            }

            //[TestCase]
            //public async Task RaisesProjectLoadedEventBeforeCallsSetActiveMethod()
            //{
            //    var factory = Factory.Create().SetupDefault(ProjectManagementType.MultipleDocuments);
            //    var mock = factory.MockProjectManager();
            //    var projectManager = mock.Object;

            //    IList<string> actionNames = new List<string>();

            //    Listener.ListenToProjectManager(factory, (name, args) => actionNames.Add(name));

            //    await projectManager.LoadAsync("dummyLocation");

            //    var projectLoadedIndex = actionNames.Single(x => string.Equals(x, Listener.ProjectManagerProjectLoaded));
            //    var setAciveIndex = actionNames.Single(x => string.Equals(x, Listener.ProjectManagerSetActiveProject));

            //    Assert.Less(projectLoadedIndex, setAciveIndex);
            //}

            //[TestCase]
            //public async Task CallsSetActiveMethodWithLoadedProjectInParameter()
            //{
            //    var factory = Factory.Create().SetupDefault();
            //    var mock = factory.MockProjectManager();
            //    var projectManager = mock.Object;

            //    IProject loadedProject = null;
            //    projectManager.ProjectLoaded += async (sender, e) => loadedProject = e.Project;

            //    await projectManager.Load("dummyLocation");

            //    mock.Verify(x => x.SetActiveProject(loadedProject), Times.Once);
            //}
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
                projectManager.ProjectLoadingAsync += async (sender, e) => eventRaised = true;

                await projectManager.LoadInactiveAsync("dummyLocation");

                Assert.IsTrue(eventRaised);
            }

            [TestCase]
            public async Task RaisesProjectLoadingFailedEvent()
            {
                var factory = Factory.Create().SetupDefault();
                var projectManager = factory.GetProjectManager();

                var eventRaised = false;
                projectManager.ProjectLoadingFailedAsync += async (sender, e) => eventRaised = true;

                await projectManager.LoadInactiveAsync("cannotload");

                Assert.IsTrue(eventRaised);
            }

            [TestCase]
            public async Task RaisesProjectLoadingCanceledEvent()
            {
                var factory = Factory.Create().SetupDefault();
                var projectManager = factory.GetProjectManager();

                var eventRaised = false;
                projectManager.ProjectLoadingAsync += async (sender, e) => e.Cancel = true;
                projectManager.ProjectLoadingCanceledAsync += async (sender, e) => eventRaised = true;

                await projectManager.LoadInactiveAsync("dummyLocation");

                Assert.IsTrue(eventRaised);
            }

            [TestCase]
            public async Task DoesntRaiseProjectLoadedEventIfCanceled()
            {
                var factory = Factory.Create().SetupDefault();
                var projectManager = factory.GetProjectManager();

                var eventRaised = false;
                projectManager.ProjectLoadingAsync += async (sender, e) => e.Cancel = true;
                projectManager.ProjectLoadedAsync += async (sender, e) => eventRaised = true;

                await projectManager.LoadInactiveAsync("dummyLocation");

                Assert.IsFalse(eventRaised);
            }

            [TestCase]
            public async Task RaisesProjectLoadedEvent()
            {
                var factory = Factory.Create().SetupDefault();
                var projectManager = factory.GetProjectManager();

                var eventRaised = false;
                projectManager.ProjectLoadedAsync += async (sender, e) => eventRaised = true;

                await projectManager.LoadInactiveAsync("dummyLocation");

                Assert.IsTrue(eventRaised);
            }

            //[TestCase]
            //public async Task DoesntCallSetActiveMethodWithLoadedProjectInParameter()
            //{
            //    var factory = Factory.Create().SetupDefault();
            //    var mock = factory.MockProjectManager();
            //    var projectManager = mock.Object;

            //    await projectManager.LoadInactive("dummyLocation");

            //    mock.Verify(x => x.SetActiveProject(It.IsAny<IProject>()), Times.Never);
            //}
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

                await projectManager.RefreshAsync();

                Assert.IsNull(projectManager.ActiveProject);
            }

            [TestCase]
            public async Task RaisesProjectActivatedEvent()
            {
                var factory = Factory.Create().SetupDefault();
                var projectManager = factory.GetProjectManager();

                await projectManager.LoadAsync("dummyLocation");

                var eventRaised = false;
                projectManager.ProjectActivatedAsync += async (sender, e) => eventRaised = true;

                await projectManager.RefreshAsync();

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

                await projectManager.LoadAsync("dummyLocation");

                Assert.AreEqual("dummyLocation", projectManager.ActiveProject.Location);

                await projectManager.SaveAsync(newLocation);

                Assert.AreEqual(newLocation, projectManager.ActiveProject.Location);
            }

            [TestCase]
            public async Task RaisesProjectSavingEvent()
            {
                var factory = Factory.Create().SetupDefault();
                var projectManager = factory.GetProjectManager();

                await projectManager.LoadAsync("dummyLocation");

                var eventRaised = false;
                projectManager.ProjectSavingAsync += async (sender, e) => eventRaised = true;

                await projectManager.SaveAsync();

                Assert.IsTrue(eventRaised);
            }

            [TestCase]
            public async Task RaisesProjectSavedEvent()
            {
                var factory = Factory.Create().SetupDefault();
                var projectManager = factory.GetProjectManager();

                await projectManager.LoadAsync("dummyLocation");

                var eventRaised = false;
                projectManager.ProjectSavedAsync += async (sender, e) => eventRaised = true;

                await projectManager.SaveAsync();

                Assert.IsTrue(eventRaised);
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

                await projectManager.LoadAsync("dummyLocation");

                Assert.IsNotNull(projectManager.ActiveProject);

                await projectManager.CloseAsync();

                Assert.IsNull(projectManager.ActiveProject);
            }

            [TestCase]
            public async Task UpdatesLocationAfterClosingProject()
            {
                var factory = Factory.Create().SetupDefault();
                var projectManager = factory.GetProjectManager();

                await projectManager.LoadAsync("dummyLocation");

                Assert.AreEqual("dummyLocation", projectManager.ActiveProject.Location);

                await projectManager.CloseAsync();

                Assert.AreEqual(null, projectManager.ActiveProject);
            }

            [TestCase]
            public async Task RaisesProjectClosingEvent()
            {
                var factory = Factory.Create().SetupDefault();
                var projectManager = factory.GetProjectManager();

                await projectManager.LoadAsync("dummyLocation");

                var eventRaised = false;
                projectManager.ProjectClosingAsync += async (sender, e) => eventRaised = true;

                await projectManager.CloseAsync();

                Assert.IsTrue(eventRaised);
            }

            [TestCase]
            public async Task RaisesProjectClosedEvent()
            {
                var factory = Factory.Create().SetupDefault();
                var projectManager = factory.GetProjectManager();

                await projectManager.LoadAsync("dummyLocation");

                var eventRaised = false;
                projectManager.ProjectClosedAsync += async (sender, e) => eventRaised = true;

                await projectManager.CloseAsync();

                Assert.IsTrue(eventRaised);
            }

            [TestCase(Reason = "ORCOMP-147")]
            public async Task DoesNotReloadClosedProjectAgain()
            {
                var factory = Factory.Create().SetupDefault();
                var projectManager = factory.GetProjectManager();

                await projectManager.LoadAsync("dummyLocation");

                var eventCount = 0;
                var projects = new List<IProject>();
                projectManager.ProjectActivatedAsync += async (sender, e) =>
                {
                    eventCount++;
                    projects.Add(e.NewProject);
                };

                await projectManager.CloseAsync();

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

                await projectManager.SetActiveProjectAsync(newProject);

                await projectManager.LoadAsync(newLocation);
                var activeProject = projectManager.ActiveProject;

                Assert.AreNotEqual(initialActiveProject?.Location, activeProject?.Location);
                Assert.AreEqual(activeProject.Location, newProject.Location);
            }

            [TestCase("dummyLocation")]
            public async Task RaiseProjectActivationEvent(string newLocation)
            {
                var factory = Factory.Create().SetupDefault();
                var projectManager = factory.GetProjectManager();

                IProject projectFromEvent = null;

                projectManager.ProjectActivationAsync += async (sender, e) => projectFromEvent = e.NewProject;
                var newProject = factory.CreateProject(newLocation);

                await projectManager.LoadAsync(newLocation);
                await projectManager.SetActiveProjectAsync(newProject);

                Assert.AreEqual(newProject.Location, projectFromEvent.Location);
            }

            [TestCase("dummyLocation")]
            public async Task RaiseProjectActivatedEvent(string newLocation)
            {
                var factory = Factory.Create().SetupDefault();
                var projectManager = factory.GetProjectManager();

                IProject projectFromEvent = null;

                projectManager.ProjectActivatedAsync += async (sender, e) => projectFromEvent = e.NewProject;
                var newProject = factory.CreateProject(newLocation);

                await projectManager.LoadAsync(newLocation);
                await projectManager.SetActiveProjectAsync(newProject);

                await Task.Delay(50);

                Assert.AreEqual(newProject.Location, projectFromEvent.Location);
            }

            //[TestCase("dummyLocation")]
            //public async Task RaiseProjectActivatedEventAfterSettingActiveProject(string newLocation)
            //{
            //    var factory = Factory.Create().SetupDefault();
            //    var mock = factory.MockProjectManager();
            //    var projectManager = mock.Object;

            //    IList<string> actionNames = new List<string>();

            //    Listener.ListenToProjectManager(factory, (name, args) => actionNames.Add(name));

            //    var newProject = factory.CreateProject(newLocation);

            //    await projectManager.SetActiveProject(newProject);

            //    var projectActivatedIndex = actionNames.Single(x => string.Equals(x, Listener.ProjectManagerProjectActivated));
            //    var setAciveIndex = actionNames.Single(x => string.Equals(x, Listener.ProjectManagerActiveProjectSet));

            //    Assert.Less(setAciveIndex, projectActivatedIndex);
            //}
        }
    }
}