namespace Orc.ProjectManagement.Test.Managers;

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
        public async Task RaisesProjectLoadingEventAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            var eventRaised = false;
            projectManager.ProjectLoadingAsync += async (sender, e) => eventRaised = true;

            await projectManager.LoadAsync("dummyLocation");

            Assert.That(eventRaised, Is.True);
        }

        [TestCase]
        public async Task RaisesProjectLoadingFailedEventAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            var eventRaised = false;
            projectManager.ProjectLoadingFailedAsync += async (sender, e) => eventRaised = true;

            await projectManager.LoadAsync("cannotload");

            Assert.That(eventRaised, Is.True);
        }

        [TestCase]
        public async Task RaisesProjectLoadingCanceledEventAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            var eventRaised = false;
            projectManager.ProjectLoadingAsync += async (sender, e) => e.Cancel = true;
            projectManager.ProjectLoadingCanceledAsync += async (sender, e) => eventRaised = true;

            await projectManager.LoadAsync("dummyLocation");

            Assert.That(eventRaised, Is.True);
        }

        [TestCase]
        public async Task DoesntRaiseProjectLoadedEventIfCanceledAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            var eventRaised = false;
            projectManager.ProjectLoadingAsync += async (sender, e) => e.Cancel = true;
            projectManager.ProjectLoadedAsync += async (sender, e) => eventRaised = true;

            await projectManager.LoadAsync("dummyLocation");

            Assert.That(eventRaised, Is.False);
        }

        [TestCase]
        public async Task RaisesProjectLoadedEventAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            var eventRaised = false;
            projectManager.ProjectLoadedAsync += async (sender, e) => eventRaised = true;

            await projectManager.LoadAsync("dummyLocation");

            Assert.That(eventRaised, Is.True);
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
        public async Task RaisesProjectLoadingEventAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            var eventRaised = false;
            projectManager.ProjectLoadingAsync += async (sender, e) => eventRaised = true;

            await projectManager.LoadInactiveAsync("dummyLocation");

            Assert.That(eventRaised, Is.True);
        }

        [TestCase]
        public async Task RaisesProjectLoadingFailedEventAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            var eventRaised = false;
            projectManager.ProjectLoadingFailedAsync += async (sender, e) => eventRaised = true;

            await projectManager.LoadInactiveAsync("cannotload");

            Assert.That(eventRaised, Is.True);
        }

        [TestCase]
        public async Task RaisesProjectLoadingCanceledEventAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            var eventRaised = false;
            projectManager.ProjectLoadingAsync += async (sender, e) => e.Cancel = true;
            projectManager.ProjectLoadingCanceledAsync += async (sender, e) => eventRaised = true;

            await projectManager.LoadInactiveAsync("dummyLocation");

            Assert.That(eventRaised, Is.True);
        }

        [TestCase]
        public async Task DoesntRaiseProjectLoadedEventIfCanceledAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            var eventRaised = false;
            projectManager.ProjectLoadingAsync += async (sender, e) => e.Cancel = true;
            projectManager.ProjectLoadedAsync += async (sender, e) => eventRaised = true;

            await projectManager.LoadInactiveAsync("dummyLocation");

            Assert.That(eventRaised, Is.False);
        }

        [TestCase]
        public async Task RaisesProjectLoadedEventAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            var eventRaised = false;
            projectManager.ProjectLoadedAsync += async (sender, e) => eventRaised = true;

            await projectManager.LoadInactiveAsync("dummyLocation");

            Assert.That(eventRaised, Is.True);
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
        public async Task DoesNothingWithoutProjectAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            Assert.That(projectManager.ActiveProject, Is.Null);

            await projectManager.RefreshAsync();

            Assert.That(projectManager.ActiveProject, Is.Null);
        }

        [TestCase]
        public async Task RaisesProjectActivatedEventAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            await projectManager.LoadAsync("dummyLocation");

            var eventRaised = false;
            projectManager.ProjectActivatedAsync += async (sender, e) => eventRaised = true;

            await projectManager.RefreshAsync();

            Assert.That(eventRaised, Is.True);
        }
    }

    [TestFixture]
    public class TheSaveMethod
    {
        [TestCase("myLocation")]
        public async Task UpdatesLocationAfterSavingProjectAsync(string newLocation)
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            await projectManager.LoadAsync("dummyLocation");

            Assert.That(projectManager.ActiveProject.Location, Is.EqualTo("dummyLocation"));

            await projectManager.SaveAsync(newLocation);

            Assert.That(projectManager.ActiveProject.Location, Is.EqualTo(newLocation));
        }

        [TestCase]
        public async Task RaisesProjectSavingEventAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            await projectManager.LoadAsync("dummyLocation");

            var eventRaised = false;
            projectManager.ProjectSavingAsync += async (sender, e) => eventRaised = true;

            await projectManager.SaveAsync();

            Assert.That(eventRaised, Is.True);
        }

        [TestCase]
        public async Task RaisesProjectSavedEventAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            await projectManager.LoadAsync("dummyLocation");

            var eventRaised = false;
            projectManager.ProjectSavedAsync += async (sender, e) => eventRaised = true;

            await projectManager.SaveAsync();

            Assert.That(eventRaised, Is.True);
        }
    }

    [TestFixture]
    public class TheCloseMethod
    {
        [TestCase]
        public async Task UpdatesProjectAfterClosingProjectAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            await projectManager.LoadAsync("dummyLocation");

            Assert.That(projectManager.ActiveProject, Is.Not.Null);

            await projectManager.CloseAsync();

            Assert.That(projectManager.ActiveProject, Is.Null);
        }

        [TestCase]
        public async Task UpdatesLocationAfterClosingProjectAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            await projectManager.LoadAsync("dummyLocation");

            Assert.That(projectManager.ActiveProject.Location, Is.EqualTo("dummyLocation"));

            await projectManager.CloseAsync();

            Assert.That(projectManager.ActiveProject, Is.EqualTo(null));
        }

        [TestCase]
        public async Task RaisesProjectClosingEventAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            await projectManager.LoadAsync("dummyLocation");

            var eventRaised = false;
            projectManager.ProjectClosingAsync += async (sender, e) => eventRaised = true;

            await projectManager.CloseAsync();

            Assert.That(eventRaised, Is.True);
        }

        [TestCase]
        public async Task RaisesProjectClosedEventAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            await projectManager.LoadAsync("dummyLocation");

            var eventRaised = false;
            projectManager.ProjectClosedAsync += async (sender, e) => eventRaised = true;

            await projectManager.CloseAsync();

            Assert.That(eventRaised, Is.True);
        }

        [TestCase(Reason = "ORCOMP-147")]
        public async Task DoesNotReloadClosedProjectAgainAsync()
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

            Assert.That(eventCount, Is.EqualTo(1));
            Assert.That(projects.Count, Is.EqualTo(1));
            Assert.That(projects[0], Is.Null);
        }
    }

    [TestFixture]
    public class TheSetActiveProjectMethod
    {
        [TestCase("dummyLocation")]
        public async Task UpdatesActiveProjectByValueFromParameterAsync(string newLocation)
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            var initialActiveProject = projectManager.ActiveProject;
            var newProject = factory.CreateProject(newLocation);

            await projectManager.SetActiveProjectAsync(newProject);

            await projectManager.LoadAsync(newLocation);
            var activeProject = projectManager.ActiveProject;

            Assert.That(activeProject?.Location, Is.Not.EqualTo(initialActiveProject?.Location));
            Assert.That(newProject.Location, Is.EqualTo(activeProject.Location));
        }

        [TestCase("dummyLocation")]
        public async Task RaiseProjectActivationEventAsync(string newLocation)
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            IProject projectFromEvent = null;

            projectManager.ProjectActivationAsync += async (sender, e) => projectFromEvent = e.NewProject;
            var newProject = factory.CreateProject(newLocation);

            await projectManager.LoadAsync(newLocation);
            await projectManager.SetActiveProjectAsync(newProject);

            Assert.That(projectFromEvent.Location, Is.EqualTo(newProject.Location));
        }

        [TestCase("dummyLocation")]
        public async Task RaiseProjectActivatedEventAsync(string newLocation)
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            IProject projectFromEvent = null;

            projectManager.ProjectActivatedAsync += async (sender, e) => projectFromEvent = e.NewProject;
            var newProject = factory.CreateProject(newLocation);

            await projectManager.LoadAsync(newLocation);
            await projectManager.SetActiveProjectAsync(newProject);

            await Task.Delay(50);

            Assert.That(projectFromEvent.Location, Is.EqualTo(newProject.Location));
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
